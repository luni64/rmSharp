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
            DB.sqLiteFile = "../../../../example_databases/Royal92-Famous+European+Royalty+Gedcom.rmTree";  // database file to be set only once 

            using (var db = new DB())
            {

                Person? root = db
                   .Persons
                   .Where(p => p.PrimaryName.Given == "Goronwy")
                   .FirstOrDefault();

                if (root != null)
                {
                    printDescendants(root);
                }
            }
        }

        private static void printDescendants(Person person, int level = 0) //recursively print descendants
        {
            var birthDate = person.Events.FirstOrDefault(e => e.FactType.Name == "Birth")?.Date;  // find birth- 
            var deathDate = person.Events.FirstOrDefault(e => e.FactType.Name == "Death")?.Date;  // and death dates from the corresponding events

            Console.WriteLine($"{new string(' ', level * 2)}(G{level})-{person} ({birthDate?.year} - {deathDate?.year})");
            
            level += 1;
            foreach (var child in person.Children)
            {
                printDescendants(child, level);
            }
        }
    }

    /********************************************
     Prints:           
        ...

        (G22)-Mountbatten Cecilie of_Greece (1911 - 1937)
            (G23)- Louis (1931 - 1937)
            (G23)- Alexander (1933 - 1937)
            (G23)- Joanna (1936 - 1939)
        (G22)- Sophia (1914 - )
            (G23)- Eight_children ( - )
        (G22)-Mountbatten Philip (1921 - )
            (G23)-Windsor Charles Philip Arthur (1948 - )
            (G24)-Windsor William Arthur Philip (1982 - )
            (G24)-Windsor Henry Charles Albert (1984 - )
            (G23)-Windsor Anne Elizabeth Alice (1950 - )
            (G24)-Phillips Peter Mark Andrew (1977 - )
            (G24)-Phillips Zara Anne Elizabeth (1981 - )
            (G23)-Windsor Andrew Albert Christian (1960 - )
            (G24)-Windsor Beatrice Elizabeth Mary (1988 - )
            (G24)-Windsor Eugenie Victoria Helena (1990 - )
            (G23)-Windsor Edward Anthony Richard (1964 - )
        (G21)-Oldenburg Christopher (1888 - 1940)
        (G22)-Oldenburg Michael (1939 - )
        (G21)- Olga ( - )
    (G20)- Dagmar "Marie" of_Denmark (1847 - 1928)
        (G21)-Romanov Nicholas_II Alexandrovich (1868 - 1918)
        (G22)-Romanov Olga Nicholovna (1895 - 1918)
        (G22)- Tatiana Nicholovna (1897 - 1918)
        (G22)-Romanov Maria Nicholovna (1899 - 1918)
        (G22)-Romanov Anastasia Nicholovna (1901 - 1918)
        (G22)-Romanov Alexis Nicolaievich (1904 - 1918)
        (G21)-Romanov Alexander Alexandrovich (1869 - 1870)
        (G21)-Romanov George Alexandrovich (1871 - 1899)
        (G21)-Romanov Xenia (1875 - 1960)
        (G22)- Irina (1895 - )
        (G22)- Andrew (1897 - )
        (G22)- Theodore (1898 - )
        (G22)- Nikita (1900 - )
        (G22)- Dimitri (1901 - )
        (G22)- Rostislav (1902 - )
        (G22)- Vassily (1907 - )
        (G21)-Romanov Michael "Mischa" Alexandrovich (1878 - 1918)       
        ...

    ********************************************************/
}
