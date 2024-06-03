using Newtonsoft.Json;
using rmSharp;

namespace rmtester
{
    internal class Program
    {
        static List<dynamic> settings = JsonConvert.DeserializeObject<List<dynamic>>(
            """
            [
                {
                    "startYear":          1630,
                    "startCity":          "Cleveland",  // choose any place in the US listed in the us_cities.txt file 
                    "startState":         "Ohio",
                    "nrOfGenerations":    8,                  
                },
                {
                    "startYear":          1580,
                    "startCity":          "San Francisco",  
                    "startState":         "California",
                    "nrOfGenerations":    3,                  
                },
            ]            
            """) ?? []; // generate empty list if settings string is malformed


        static void Main(string[] args)
        {
            DB.sqLiteFile = "empty.rmtree";

            using (var db = new DB())
            {
                foreach (var setting in settings)
                {
                    int startYear = setting["startYear"];
                    string startCity = setting["startCity"];
                    string startState = setting["startState"];
                    int maxGen = setting["nrOfGenerations"];

                    var startPlace = Helpers.PlaceList.FirstOrDefault(p => p.city == startCity && p.state == startState);
                    if (startPlace != null)
                    {
                        Place birthPlace = db.Places.GetOrMakeNew(startPlace);
                        Person rootPerson = db.Persons.makeNew(startYear, Sex.Male, birthPlace);
                        makeTreeRecursively(db, rootPerson, maxGen);
                    }
                    else Console.WriteLine("Start Place not found");
                }
                db.SaveChanges();
            }
        }


        static void makeTreeRecursively(DB db, Person person, int maxGen, int generation = 0)
        {
            if (generation++ > maxGen) return;

            var family = db.MakeFamilyFrom(person); 
            if (family != null)
            {
                db.AddChildrenTo(family);
                foreach (var child in family.Children)
                {
                    makeTreeRecursively(db, child, maxGen, generation);
                }
            }
        }
    }
}