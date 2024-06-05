
/*********************************************************************************** 
 * Uses an empty database which will be regenerated at compiletime 
 * 
 * This example creates a few simple individuals, and combines them to a family.
 * It then generates tree groups and assigns the genreated individuals to those groups.
 * 
 * To verify, open the generated database with RootsMagic (the generated database 
 * can be found at "...\07_WorkingWithGroups\bin\Debug\net8.0\empty.rmtree")
 * 
 ******************************************************************************/

using static System.Console;

namespace rmSharp.Examples
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DB.sqLiteFile = "empty.rmTree";

            using (var db = new DB())
            {
                var Pete = new Person("Smith", "Peter", Sex.Male);
                var Paula = new Person("Miller", "Paula", Sex.Female);
                var Tom1 = new Person("Smith", "Thomas", Sex.Male);
                var Mary = new Person("Jefferson", "Mary", Sex.Male);
                var Tom2 = new Person("Baker", "Tom", Sex.Male);

                var theSmiths = new Family(Pete, Paula);
                theSmiths.AddChild(Tom1);

                db.Persons.AddRange(Pete, Paula, Tom1, Mary, Tom2); // Register the new entities in the database context
                db.Families.Add(theSmiths);
                db.SaveChanges();                                   // Groups work with PersonIds. To generate the Ids, we need to persist
                                                                    // the new entities. 

                Group aGroup = new("The Smith Group");              // generate a new group 
                aGroup.Persons.AddRange([Pete, Paula, Tom1]);       // and add the Smith family members to it

                Group anotherGroup = new("All Toms");               // make another group 
                anotherGroup.Persons.AddRange([Tom1, Tom2]);        // and add the Toms to it

                Group yetAnotherGroup = new("Lonely Mary");         // make yet another group 
                yetAnotherGroup.Persons.Add(Mary);               

                db.AddRange(aGroup, anotherGroup, yetAnotherGroup); // register the groups in the db context
                db.SaveChanges();                                   // and write everything into the database                

                                
                foreach (var group in db.Groups)                    // readout the generated groups and display its members
                {
                    WriteLine(group.Name);
                    foreach (var person in group.Persons)
                    {
                        WriteLine($"   -{person.PrimaryName} ({person.PersonId})");
                    }
                    WriteLine();
                }

                /* Prints:
                    The Smith Group
                        -Smith Peter (1)
                        -Miller Paula (2)
                        -Smith Thomas (3)

                    All Toms
                        -Smith Thomas (3)
                        -Baker Tom (5)

                    Lonely Mary
                        -Jefferson Mary (4)
                 */
            }
        }
    }
}
