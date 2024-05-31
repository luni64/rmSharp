using Microsoft.EntityFrameworkCore.Diagnostics;
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

            using (var db = new DB())
            {
                foreach (var item in db.Families)
                {
                    if(item.Children.Count > 0)
                    {
                    Console.WriteLine(item);

                    }
                }

                //foreach (var person in db.Persons.ToList().Where(p => p.Families.ToList().Count() > 1))
                //{
                //    Console.WriteLine($"{person.PrimaryName} ({person.PersonId})");
                //    foreach (var family in person.Families)
                //    {
                //        Console.WriteLine($"  F: {family}");
                //        foreach(var  child in family.ChildInfos)
                //        {
                //            Console.WriteLine($"    C: {child.Child}");
                //        }
                //    }
                //    Console.WriteLine();
                //}
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
