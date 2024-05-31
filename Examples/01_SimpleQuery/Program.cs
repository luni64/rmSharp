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
    internal class Program
    {
        static void Main(string[] args)
        {
            DB.sqLiteFile = "../../../../example_databases/US_Presidents.rmTree";  // database file to be set only once 

            using (var db = new DB())
            {
                var query = db.Names                        // query the Names table
                    .Where(n => n.IsPrimary == true)        // filter for primary names
                    .Select(n=> new { n.Surname, n.Given}); // we only need surname and given name here

                WriteLine(query.ToQueryString() + "\n\n");  // This prints the auto-generated SQL which was sent to the database
                
                foreach (var entry in query)
                {
                    WriteLine(entry.Surname + " " + entry.Given);
                }
            }          
        }
    }
}

