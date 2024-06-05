using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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


        public static long eventType(this DbSet<FactType> facts, string EventName)
        {
            return facts.Where(f => f.Name == EventName).Select(f => f.FactTypeId).FirstOrDefault();
        }


        public static IEnumerable<(long startId, long endId)> GetIdRanges(this IEnumerable<Person> persons)
        {
            long startId = persons.First().PersonId;
            long endId = startId;

            foreach (var person in persons.Skip(1))
            {
                long cur = person.PersonId;
                if (cur - endId != 1)
                {
                    yield return (startId, endId);
                    startId = cur;
                }
                endId = cur;
            }
            yield return (startId, endId);
        }

        public static Group MakeGroup(this DB db, string name)
        {
            var grp = db.Tags.Where(g => g.TagType == 0);
            var tagValue = grp.Any() ? grp.Max(g => g.TagValue) +1 : 1000;

            Group group = new Group()
            {
                Name = name,
                TagValue = tagValue
            };

            db.Tags.Add(group);
            return group;
        }
    }
}
