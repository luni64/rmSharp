using Geolocation;
using Microsoft.EntityFrameworkCore;
using rmSharp;
using System.Globalization;

namespace rmtester
{
    internal static class Helpers
    {
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

            var deathday = birthday.AddDays(rnd.Next(35 * 356, 90 * 356));

            var person = new Person();
            person.Names.Add(given);
            person.Sex = sex;
            person.Events.Add(new PersonEvent { EventType = 1, Date = new RMDate(birthday), Place = p });
            person.Events.Add(new PersonEvent { EventType = 2, Date = new RMDate(deathday) });

            persons.Add(person);

            Console.WriteLine(person);

            return person;
        }

        public static DateTime getEventDate(this Person person, long eventId)
        {
            var ev = person.Events.First(e => e.EventType == eventId);
            RMDate date = ev.Date;
            return new DateTime(date.year, date.month, date.day);
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

        private static Random rnd = new();


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


        static public Place GetNearbyPlace(this DB db, Place place, double maxDistance)
        {
            double sqrSide = maxDistance * 0.707106;

            var nearByPlaces =
                 placeList
                .Where(p => GeoCalculator.GetDistance(p.location, place.Location, distanceUnit: DistanceUnit.Kilometers) < sqrSide)
                .ToList();

            var nearByPlace = nearByPlaces[rnd.Next(0, nearByPlaces.Count)];  // randomly choose one

            return db.Places.GetOrMakeNew(nearByPlace);

        }


    }
}








