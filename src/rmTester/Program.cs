using Geolocation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite.Query.Internal;
using Newtonsoft.Json;
using rmSharp;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Diagnostics;
using Task = rmSharp.Task;

namespace rmtester
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DB.sqLiteFile = "empty.rmtree";

            using (var db = new DB())
            {
                var Pete = new Person("Smith", "Peter", Sex.Male);
                var Paula = new Person("Miller", "Paula", Sex.Female);
                var Tom = new Person("Smith", "Thomas", Sex.Male);
                var Mary = new Person("Jefferson", "Mary", Sex.Male);


                var theSmiths = new Family(Pete, Paula);
                theSmiths.AddChild(Tom);

                db.AddRange(Pete, Paula, Tom, Mary, theSmiths); // register the new entities in the database context
                db.SaveChanges();                               // we need to persist everything in the database to
                                                                // generate the person id's which are required for working with groups


                Group aGroup = new("Smith Group 2");
                aGroup.Persons.AddRange([Pete, Paula, Mary]);

                Group anotherGroup = new("testgroup");
                anotherGroup.Persons.Add(Tom);

                db.AddRange(aGroup, anotherGroup);
                
                
                db.SaveChanges();

                Group group = db.Groups.First();

                








                //var xx = db.GetGroupMembers(y);

                //var list = Enumerable.Range(1, 1)
                //    .Concat(Enumerable.Range(500, 5))
                //    .Concat(Enumerable.Range(600, 3))
                //    .Concat(Enumerable.Range(505, 1))
                //    .ToList().ConvertAll(e => (long)e);

                //var e = db.Persons.Where(p => list.Contains(p.PersonId));


                //var g = e.GetIdRanges();

                //foreach (var item in g)
                //{
                //    Trace.WriteLine($"{item.startId} {item.endId}");
                //}

                //Trace.WriteLine(g.ToQueryString());
            }
        }

        private static void Db_SavingChanges(object? sender, SavingChangesEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}






