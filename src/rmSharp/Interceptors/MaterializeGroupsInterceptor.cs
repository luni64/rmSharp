using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace rmSharp
{
    public class MaterializeGroupsInterceptor : IMaterializationInterceptor
    {
        public object InitializedInstance(MaterializationInterceptionData materializationData, object instance)
        {
            if (instance is Group group)
            {
                List<long> personIds = [];
                foreach (var entry in group.Entries)
                {
                    for (long id = entry.StartId; id <= entry.EndId; id++)
                    {
                        personIds.Add(id);
                    }
                }

                if (materializationData.Context is rmContext db)
                {
                    group. Persons.AddRange(db.Persons.Where(p => MemberIds(group).Contains(p.PersonId)));
                }
            }
            return instance;
        }

        private IReadOnlyList<long> MemberIds(Group group)
        {
            List<long> ids = [];
            foreach (var entry in group.Entries)
            {
                for (long id = entry.StartId; id <= entry.EndId; id++)
                {
                    ids.Add(id);
                }
            }
            return ids;
        }
    }
}

