using Microsoft.EntityFrameworkCore;
using rmSharp;
using System.Linq;
using static System.Console;

/****************************************************************************** 
 * 
 * This example extracts all primary names from the "US_Presidents.rmTree" database
 * and prints their "Surname" and "Given" properties
 * 
 ******************************************************************************/


namespace rmSharp.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            DB.sqLiteFile = "../../../../example_databases/US_Presidents.rmTree";  // database file

            using (var db = new DB())
            {
                var primaryNames = db.Names
                    .Where(n => n.NameType == NameTypes.Primary)  // get all primary names from the database...
                    .OrderBy(n => n.Surname);                     // ... and order them by surname


                foreach (var name in primaryNames)
                {
                    WriteLine(name.Surname + " " + name.Given);
                }

                WriteLine(primaryNames.ToQueryString());
            }
        }
    }
}

