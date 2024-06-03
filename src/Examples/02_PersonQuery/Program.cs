using rmSharp;
using System.Linq;
using static System.Console;

/****************************************************************************** 
 * Uses the "US_Presidents.rmTree" database
 * 
 * This example prints all male persons with surname "Jefferson" and their birthdays
 * Additionally, for each Jefferson, it prints children, thier name and their sex 
 * 
 ******************************************************************************/


namespace rmSharp.Examples
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DB.sqLiteFile = "../../../../example_databases/US_Presidents.rmTree";  // database file to be set only once 

            using (var db = new DB())
            {
                var jeffersons = db.Persons.Where(p => p.PrimaryName.Surname == "Jefferson" && p.Sex == Sex.Male);  // get all male Jeffersons from the database                                                                                      

                foreach (var person in jeffersons)
                {
                    Write(person.PrimaryName);

                    var birthEvent = person
                        .Events                                 // query the events belonging to this person
                        .Where(e => e.FactType.Name == "Birth") // filter for birth events
                        .SingleOrDefault();                     // get the event or null if none - or more than one which should not happen of course)

                    if (birthEvent != null)                     // if we have a birth event we print the date
                    {
                        Write($" (*{birthEvent.Date})");
                    }
                    WriteLine();

                    foreach (var child in person.Children)      // print primary name and sex of all children                              
                    {
                        WriteLine($"  - {child.PrimaryName} ({child.Sex})");
                    }
                }
            }
        }
    }
}

