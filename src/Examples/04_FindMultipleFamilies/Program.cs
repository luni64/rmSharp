using System;
using System.Linq;


/****************************************************************************** 
 * Uses the "US_Presidents.rmTree" database
 * 
 * This example prints all persons with multiple families (e.g. married twice)
 * It prints the families those persons are part of, and the corresponding 
 * children.
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
                foreach (var person in db.Persons.Where(p => p.Families.Count > 1))  // persons who 'married' at least twice
                {
                    Console.WriteLine($"{person.PrimaryName} ({person.PersonId})");
                    foreach (var family in person.Families)
                    {
                        Console.WriteLine($"  Family: {family}");
                        foreach (var child in family.Children)
                        {
                            Console.WriteLine($"    Child: {child}");
                        }
                    }
                    Console.WriteLine();
                }
            }
        }
    }

    /********************************************
     Prints:           
        ...

        REAGAN Ronald Wilson (89)
          Family: REAGAN Ronald Wilson & ROBBINS Nancy_DAVIS (Anne Frances
            Child: REAGAN Patricia_Ann DAVIS (
            Child: REAGAN Ronald Prescott
          Family: REAGAN Ronald Wilson & FULKS Jane_WYMAN (Sarah Jane
            Child: REAGAN Maureen Elizabeth
            Child: REAGAN Michael Edward
            Child: REAGAN ? (Girl)

        LUCKETT Edith (113)
          Family: ROBBINS Kenneth & LUCKETT Edith
            Child: ROBBINS Nancy_DAVIS (Anne Frances
          Family: DAVIS Loyal & LUCKETT Edith

        DAVIS Loyal (118)
          Family: DAVIS Loyal & LUCKETT Edith
          Family: DAVIS Loyal &
            Child: DAVIS Richard
        ...

    ********************************************************/
}
