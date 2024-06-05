using DelegateDecompiler;
using global::Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using rmSharp.Microsoft.EntityFrameworkCore.Query;
using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace rmSharp
{
    public static class DelegateDecompilerDbContextOptionsBuilderExtensions
    {
        public static DbContextOptionsBuilder AddDelegateDecompiler(this DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.AddInterceptors(new DelegateDecompilerQueryPreprocessor2());
    }

    namespace Microsoft.EntityFrameworkCore.Query
    {
        public class DelegateDecompilerQueryPreprocessor2 : IQueryExpressionInterceptor
        {
            Expression IQueryExpressionInterceptor.QueryCompilationStarting(Expression queryExpression, QueryExpressionEventData eventData)
                => DecompileExpressionVisitor.Decompile(queryExpression);
        }

        public interface IQueryPreprocessor
        {
            Expression Process(Expression query);
        }

        //public class DelegateDecompilerQueryPreprocessor : IQueryExpressionInterceptor
        //{
        //    public Expression Process(Expression query) => DecompileExpressionVisitor.Decompile(query);
        //}

    }

    public class DB : rmContext
    {
        public DB() : base(makeOptions())
        {
         
            // unfortuntatly the TagTable is a mess. -> We need to handle TagValue generation manually 
            ChangeTracker.Tracking += ChangeTracker_Tracking;

            var groups = Tags.Where(g => g.TagType == 0);
            curGroupId = groups.Any() ? groups.Max(g => g.TagValue) : 1000;  // get the current max value of TagValue from the tags table
        }

        private long curGroupId;  // current group id

        private void ChangeTracker_Tracking(object? sender, EntityTrackingEventArgs e)
        {
            if (e.Entry.Entity is Group group)
            {
                if (group.TagValue == 0)  // we need to calculate the TagValue (id) for new groups manually. The value is needed for tracking
                {                   
                    group.TagValue = ++curGroupId; 
                }
            }
        }        

        public static string sqLiteFile { get; set; } = string.Empty;

        static private DbContextOptions<rmContext> makeOptions()
        {
            var opt = new DbContextOptionsBuilder<rmContext>();

            opt
                .UseSqlite($"Data Source={sqLiteFile}")
                .AddDelegateDecompiler()
                .AddInterceptors(new SQLiteExtensionInterceptor())
                .AddInterceptors(new MaterializeGroupsInterceptor())
                .UseLazyLoadingProxies()
                .EnableSensitiveDataLogging(true);            

            return opt.Options;
        }

        public override int SaveChanges()
        {
            var saveTime = DateTime.Now;

            var changedEntries = ChangeTracker
                .Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
                .ToList();

            foreach (var entityEntry in changedEntries)
            {
                if (entityEntry.Metadata.FindProperty("ChangeDate") != null)
                {
                    entityEntry.Property("ChangeDate").CurrentValue = saveTime;
                }

                if (entityEntry.Entity is Group group)
                {
                    group.UpdateRanges();  //calculate the id ranges from the list of persons before saving. 
                }
            }
            return base.SaveChanges();
        }

        public IQueryable<Group> Groups => Tags.OfType<Group>();
    }
}

