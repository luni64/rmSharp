using System;
using System.Linq;


/*********************************************************************************** 
 * Uses an empty database which will be copied to the output folder at compiletime 
 * 
 * This example creates a few simple individuals, combines them to a family
 * and saves them to the database. 
 *
 * Be careful when writing data to a real database. Best to work on a 
 * copy until everything is tested. 
 * It is always a good idea to rebuild the indizes (Tools/Database Tools) from 
 * within RootMagic. This avoids possible issues with the proprietary 
 * collation algorithmus of RootsMagic.
 ******************************************************************************/

namespace rmSharp.Examples
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DB.sqLiteFile = "empty.rmTree";  // the database file is stored in the output folder: "06_AddPerson\bin\Debug\net8.0\empty.rmTree"

            using (var db = new DB())
            {
                var Pete = new Person("Miller", "Peter", Sex.Male);    
                var Mary = new Person("Taylor", "Mary", Sex.Female);
                var Thomas = new Person("Miller", "Thomas", Sex.Male);
                var Michael = new Person("Taylor", "Michael", Sex.Male);
                db.Persons.AddRange([Pete,Mary,Thomas,Michael]);   // add the new individuals to the Persons table             

                var family = new Family(Pete, Mary);
                family.AddChild(Thomas);                                 
                family.AddChild(Michael, relFather: RelationShip.Step);  // add a step child
                db.Families.Add(family);                                 // add the family to the Families table

                db.SaveChanges(); // writes all changes to the database, you'll find it here "06_AddPerson\bin\Debug\net8.0\empty.rmTree"
            }
        }
    }
}
