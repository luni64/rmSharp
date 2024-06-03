using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace rmSharp
{
    public static partial class Extensions
    {
        // The _uid field contains a standard (version 4) UUID (16 bytes long), followed by two checksum bytes.
        // The 18 bytes of the combination are stored as a hex string in the _uid field. The checksum algorithm
        // used by PAF can be found at https://archive.fhiso.org/BetterGEDCOM/files/GEDCOMUniqueIdentifiers.pdf.
        // Other sources show more "complicated" algorithms. However, they result in the same checksum.
        // It appears that the algorithm used is Fletcher16: https://de.wikipedia.org/wiki/Fletcher%E2%80%99s_Checksum
        public static string toGedUid(this Guid guid)
        {
            var uidBytes = guid.ToByteArray(); // GUID.ToByteArray() produces a shuffled byte sequence which must be deshuffled first.
            Array.Reverse(uidBytes, 0, 4);     // For the deshuffling algorithm see: https://stackoverflow.com/q/9195551
            Array.Reverse(uidBytes, 4, 2);
            Array.Reverse(uidBytes, 6, 2);

            var checksum = new byte[2];
            foreach (var uuidByte in uidBytes)
            {
                checksum[0] += uuidByte;
                checksum[1] += checksum[0];
            }

            return // return bytes as hex string
                String.Concat(Array.ConvertAll(uidBytes, c => c.ToString("X2"))) +
                String.Concat(Array.ConvertAll(checksum, c => c.ToString("X2")));
        }


        //public static Place GetOrMake(this DbSet<Place> places, string name, string abbreviation)
        //{
        //    Place? place = places.FirstOrDefault(p => p.Name == name && p.Abbrev == abbreviation);
        //    if (place == null)
        //    {
        //        place = new Place
        //        {
        //            PlaceType = 0,
        //            Name = name,
        //            Abbrev = abbreviation
        //        };
        //        places.Add(place);
        //    }
        //    return place;
        //}

        public static long eventType(this DbSet<FactType> facts, string EventName)
        {
            return facts.Where(f => f.Name == EventName).Select(f => f.FactTypeId).FirstOrDefault();
        }

    }
}
