using Geolocation;
using MathNet.Numerics.Distributions;
using Microsoft.EntityFrameworkCore;
using rmSharp;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Intrinsics.X86;

namespace rmtester
{
    internal static class Helpers
    {
        public static List<placeDTO> PlaceList => placeList;

        public static Person makeNew(this DbSet<Person> persons, int year, Sex sex, Place p)
        {
            var birthday = new DateTime(year, 1, 1).AddDays(rnd.Next(-300, 300));
            return persons.makeNew(birthday, sex, p);
        }
        public static Person makeNew(this DbSet<Person> persons, DateTime birthday, Sex sex, Place p)
        {
            var givens = sex == Sex.Male ? maleNames : femaleNames;
            var given = new Name
            {
                Given = givens.ElementAt(rnd.Next(0, givens.Count)),
                Surname = surnames.ElementAt(rnd.Next(0, surnames.Count))
            };
                       
            var deathday =  birthday.AddDays(Math.Min(105*365, personAgeDistribution.Sample()));
            
            var person = new Person();
            person.Names.Add(given);
            person.Sex = sex;
            person.Events.Add(new PersonEvent { EventType = 1, Date = new RMDate(birthday), Place = p });
            person.Events.Add(new PersonEvent { EventType = 2, Date = new RMDate(deathday) });
            persons.Add(person);

            Console.WriteLine($"{++cnt}-{person}");

            return person;
        }

        public static double Age(this Person person) => (person.getEventDate(2) - person.getEventDate(1)).TotalDays / 365.2425;

        public static Family? MakeFamilyFrom(this DB db, Person spouse1)
        {
            if (rnd.Next(0, 100) < 15) return null; // 15% of the persons stay single

            if (spouse1.Age() < 17) return null;  // lives too short to found a family

            // generate spouse
            var sp2birthPlace = db.Places.GetOrMakeNearby(spouse1.getEventPlace(1), 150);
            var sp2birthDay = spouse1.getEventDate(1).AddDays(365 * rnd.Next(-5, 5)); // birthday +/- 5 years
            var sp2sex = spouse1.Sex == Sex.Male ? Sex.Female : Sex.Male;
            Person spouse2 = db.Persons.makeNew(sp2birthDay, sp2sex, sp2birthPlace);

            if (spouse2.Age() < 17)
            {
                db.Persons.Remove(spouse2);
                return null;
            }

            // generate marriage
            var olderSpouse = spouse1.getEventDate(1) > spouse2.getEventDate(1) ? spouse1 : spouse2;
            var marriageDate = olderSpouse.getEventDate(1).AddDays(rnd.Next(365 * 18, 365 * 35));  // marriage 18-45 years after birth of younger spouse

            if (marriageDate > spouse1.getEventDate(2) || marriageDate > spouse2.getEventDate(2))  // already dead at proposed marriage date
            {
                db.Persons.Remove(spouse2);
                return null;
            }

            // generate family and add marriage event
            Family family = new(spouse1, spouse2);
            var marriagePlace = db.Places.GetOrMakeNearby(sp2birthPlace, 150);
            family.Events.Add(new FamilyEvent { EventType = 300, Date = new RMDate(marriageDate), Place = marriagePlace });
            db.Families.Add(family);

            return family;
        }
        public static void AddChildrenTo(this DB db, Family family)
        {
            if (rnd.Next(0, 100) < 10) return; // 10% of the couples will have no kids

            DateTime deathdayH = family.Husband.getEventDate(2);
            DateTime deathdayW = family.Wife.getEventDate(2);

            var marriageDate = family.getEventDate(300);
            var marriagePlace = family.getEventPlace(300)!;

            int nrOfChildren = (int) Math.Max(0, childDistribution.Sample()); 
            var childBirthday = marriageDate.AddDays(rnd.Next(-100, 3 * 365)); // first child born between 100 days before or 3 years after the marriage
            for (int i = 0; i < nrOfChildren; i++)
            {
                if (childBirthday >= deathdayH || childBirthday >= deathdayW) break;

                var sex = rnd.Next(2) == 0 ? Sex.Male : Sex.Female;
                var child = db.Persons.makeNew(childBirthday, sex, marriagePlace);
                child.Names.First().Surname = family.Husband.PrimaryName.Surname;
                family.AddChild(child);

                childBirthday = childBirthday.AddDays(rnd.Next(300, 3 * 365));
            }
        }

        public static PersonEvent getEvent(this Person person, long eventId) => person.Events.First(e => e.EventType == eventId);
        public static Place getEventPlace(this Person person, long eventId) => person.getEvent(eventId).Place!;
        public static DateTime getEventDate(this Person person, long eventId)
        {
            RMDate date = person.getEvent(eventId).Date;
            return new DateTime(date.year, date.month, date.day);
        }

        public static FamilyEvent getEvent(this Family family, long eventId) => family.Events.First(e => e.EventType == eventId);
        public static Place? getEventPlace(this Family family, long eventId) => family.getEvent(eventId).Place;
        public static DateTime getEventDate(this Family family, long eventId)
        {
            RMDate date = family.getEvent(eventId).Date;
            return new DateTime(date.year, date.month, date.day);
        }

        static public Place GetOrMakeNearby(this DbSet<Place> places, Place place, double maxDistance)
        {
            double sqrSide = maxDistance * 0.707106;

            var nearByPlaces =
                 PlaceList
                .Where(p => GeoCalculator.GetDistance(p.location, place.Location, distanceUnit: DistanceUnit.Kilometers) < sqrSide)
                .ToList();

            var nearByPlace = nearByPlaces[rnd.Next(0, nearByPlaces.Count)];  // randomly choose one

            return places.GetOrMakeNew(nearByPlace);
        }
        public static Place GetOrMakeNew(this DbSet<Place> places, placeDTO place)
        {
            Place? p = places.FirstOrDefault(p => p.Name == place.Name); // do we have an entry?
            if (p == null)                                                                     // if not, generate one                 
            {
                p = new Place
                {
                    PlaceType = 0,
                    Name = place.Name,
                    Reverse = place.Reverse,
                    LatLongExact = 1,
                    Latitude = (long)(place.lat * 10000000.0),
                    Longitude = (long)(place.lon * 10000000.0)
                };
                places.Add(p);
            }
            return p;
        }


        private static List<string> femaleNames = File.ReadLines("GivenFemales.txt").ToList();
        private static List<string> maleNames = File.ReadLines("GivenMales.txt").ToList();
        private static List<string> surnames = File.ReadAllLines("surnames.txt").ToList();
        private static List<placeDTO> placeList =
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

        private static Random rnd = new();

        static Normal childDistribution = new(4,6); // nr of children 4+/-6
        static Normal spouseAgeDifferenceDistribution = new(0, 5); // nr of children 4+/-6
        static Normal personAgeDistribution = new Normal(50*365, 15*365); // days;

        static int cnt = 0; 
    }
}








