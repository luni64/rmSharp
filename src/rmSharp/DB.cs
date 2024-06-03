using DelegateDecompiler;
using global::Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using rmSharp.Microsoft.EntityFrameworkCore.Query;
using System;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

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

    public class SQLiteExtensionInterceptor : IDbConnectionInterceptor
    {
        public DbConnection ConnectionCreated(ConnectionCreatedEventData eventData, DbConnection result)
        {
            var sqliteConnection = (SqliteConnection)result;
            sqliteConnection.EnableExtensions();
            sqliteConnection.LoadExtension(@"unifuzz64.dll");
            return sqliteConnection;
        }
    }

    public class DB : rmContext
    {
        public DB() : base(makeOptions()) { }

        public static string sqLiteFile { get; set; } = string.Empty;

        static private DbContextOptions<rmContext> makeOptions()
        {
            var opt = new DbContextOptionsBuilder<rmContext>();

            opt
                .AddDelegateDecompiler()
                .AddInterceptors(new SQLiteExtensionInterceptor())
                .UseSqlite($"Data Source={sqLiteFile}")
                .UseLazyLoadingProxies();
            ;

            return opt.Options;
        }

        public override int SaveChanges()
        {
            var changedEntriesCopy = ChangeTracker.Entries()
                        .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
                        .ToList();
            var saveTime = DateTime.Now;

            foreach (var entityEntry in changedEntriesCopy)
            {
                if (entityEntry.Metadata.FindProperty("ChangeDate") != null)
                {
                    entityEntry.Property("ChangeDate").CurrentValue = saveTime;
                }
                else Trace.WriteLine("change date error");
            }
            return base.SaveChanges();
        }



    }
}
