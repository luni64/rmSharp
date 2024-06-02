using Microsoft.EntityFrameworkCore.Sqlite.Query.Internal;
using Newtonsoft.Json;
using rmSharp;
using System.Diagnostics;
using System.Globalization;
using Task = rmSharp.Task;

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
                    "nrOfGenerations":    7,                  
                },
                {
                    "startYear":          1580,
                    "startCity":          "San Francisco",  // choose any place in the US listed in the us_cities.txt file 
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

                    var startPlace = placeList.FirstOrDefault(p => p.city == startCity && p.state == startState);
                    if (startPlace != null)
                    {
                        Place birthPlace = db.Places.GetOrMakeNew(startPlace);
                        Person rootPerson = db.Persons.makeNew(startYear, Sex.Male, birthPlace);
                        makeTree(db, rootPerson, maxGen);
                    }
                }
                db.SaveChanges();
            }
        }

        static Family? makeFamily(DB db, Person parent)
        {
            Person husband, wife;
            Place spouseBirthplace = db.GetNearbyPlace(parent.Events.First(e => e.EventType == 1).Place!, 150);
            DateTime spouseBirthday = parent.getEventDate(1).AddDays(rnd.Next(-5 * 365, 5*365));

            if (parent.Sex == Sex.Female)
            {
                wife = parent;
                husband = db.Persons.makeNew(spouseBirthday, Sex.Male, spouseBirthplace);
            }
            else
            {
                husband = parent;
                wife = db.Persons.makeNew(spouseBirthday, Sex.Female, spouseBirthplace);
            }

            var family = new Family(husband, wife);

            DateTime birthDayH = husband.getEventDate(1);
            DateTime deathDayH = husband.getEventDate(2);
            DateTime birtDayW = wife.getEventDate(1);
            DateTime deathDayW = wife.getEventDate(2);


            var mDate = birtDayW.AddDays(rnd.Next(365 * 20, 365 * 40));
            if (mDate < deathDayH && mDate < deathDayW)
            {
                var marriagePlace = db.GetNearbyPlace(spouseBirthplace, 100);
                family.Events.Add(new FamilyEvent { EventType = 300, Date = new RMDate(mDate), Place = marriagePlace });

                DateTime childBirthday = mDate.AddDays(365 + rnd.Next(-1, 3 * 365));
                for (int i = 0; i < rnd.Next(0, 10); i++)
                {
                    if (childBirthday < deathDayH && childBirthday < deathDayW)
                    {
                        var sex = rnd.Next(2) == 0 ? Sex.Male : Sex.Female;
                        var child = db.Persons.makeNew(childBirthday, sex, spouseBirthplace);
                        child.Names.First().Surname = husband.Names.First().Surname;
                        family.AddChild(child);
                    }
                    childBirthday.AddDays(rnd.Next(340, 3 * 365));
                }
                db.Families.Add(family);

                return family;
            }
            return null;
        }

        static Family? makeTree(DB db, Person parent, int maxGen, int generation = 0)
        {            
            if (generation++ > maxGen) return null;

            var family = makeFamily(db, parent);
            if (family == null) return null;

            foreach (var child in family.Children)
            {
                makeTree(db, child, maxGen, generation);
            }
            return family;

        }

        static Random rnd = new();

        static List<placeDTO> placeList =
             File.ReadAllLines("us_cities.txt")
            .Skip(1)
            .Select(l => l.Split(','))
            .Select(p => new placeDTO()
            {
                state = p[2],
                city = p[3].Trim('"'),
                county = p[4].Trim('"'),
                lat = double.Parse(p[5], CultureInfo.InvariantCulture),
                lon = double.Parse(p[6], CultureInfo.InvariantCulture)
            })
            .ToList();
    }
}








