using rmSharp;
using System;
using System.Linq;


/****************************************************************************** 
 * Uses the "US_Presidents.rmTree" database
 * 
 * This example prints the patrilinar line (all male ancestors) of George Washington
 * 
 ******************************************************************************/

namespace rmSharp.Examples
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DB.sqLiteFile = "../../../../example_databases/US_Presidents.rmTree";  // database file to be set only once 
        
            using (var db = new DB()) // connect to the database
            {   
                var person = db.Persons.FirstOrDefault(p => p.PrimaryName.Surname == "Washington" && p.PrimaryName.Given == "George");

                int g = 0; // tracks generations
                while (person != null)
                {
                    printPerson(person, g++);
                                       
                    var father = person                                 // find the biological father of person:
                      .ParentRelations                                  // = all entries in ChildTable where 'person' acts as child
                      .Where(cr => cr.RelFather == RelationShip.Birth)  // filter for those entries where the father is the biol. father 
                      .Select(pr => pr.Father)                          // use the "Father" property from the relation 
                      .FirstOrDefault();                                // take the first entry if any (should be max 1 of course)                                                                                          

                    person = father;                                    // use the found father for the next iteration, loop stops if no father found
                }
            }
        }

        static void printPerson(Person person, int generation)
        {
            var birthDate = person.Events.FirstOrDefault(e => e.FactType.Name == "Birth")?.Date;  // find birth- 
            var deathDate = person.Events.FirstOrDefault(e => e.FactType.Name == "Death")?.Date;  // and death dates from the corresponding events

            Console.WriteLine($"({generation})\t{person.PrimaryName} ({birthDate?.year} - {deathDate?.year})");
        }
    }

    /********************************************
     Prints:     
      
        (0)     Washington George(1732 - 1799)
        (1)     Washington Augustine(1693 - 1743)
        (2)     Washington Lawrence(1659 - 1697)
        (3)     Washington John(1633 - 1676)
        (4)     Washington Lawrence(1601 - 1652)
        (5)     Washington Lawrence(1568 - 1616)
        (6)     Washington Robert(1544 - 1620)
        (7)     Washington Lawrence(1500 - 1583)
        (8)     Washington John(1478 - 1528)
        (9)     Washington Robert(1455 - 1528)
        (10)    Washington Robert( - 1483)
        (11)    de Washington John(1385 - 1423)
        (12)    de Washington John( - 1408)
        (13)    de Washington Robert( - 1348)
        (14)    de Washington Robert( - 1324)
        (15)    de Washington William( - 1290)
        (16)    de Washington Walter(1212 - 1264)
        (17)    de Washington William( - 1239)
        (18)    de Hertburn William(1180 - )
        (19)    Dolfin Patric fitz( - )
        (20)    Uchtred Dolfin fitz( - )
        (21)    Maldred Uchtred fitz( - )
        (22)     Maldred( - )
        (23)     Maldred( - )
        (24)    Thane Crinan the( - 1045)

    ********************************************************/
}
