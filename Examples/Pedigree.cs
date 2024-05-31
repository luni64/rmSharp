using RMDatabase;
using RMDatabase.Models;

namespace Example
{
    class Pedigree
    {
        public void Execute(string Surname, string Given)
        {
            using (var db = new DB())
            {
                // query the first person with the correct Surname and Given name
                var rootPerson = db.Persons
                    .Where(p => p.Names.Any(n => n.Surname == Surname && n.Given == Given && n.IsPrimary))
                    .FirstOrDefault();

                var root = new SimplePerson(db, rootPerson);
                Console.BufferHeight = 500;
                Console.BufferWidth = 150;
                Console.WindowHeight = Console.LargestWindowHeight;
                root.Print(1, 20);
            }
        }
    }


    class SimplePerson
    {
        public SimplePerson? Husband { get; set; }
        public SimplePerson? Wife { get; set; }
        public String Name { get; } = "";
        public static int maxGeneration = 0;

        public SimplePerson(DB db, Person? child, int generation = 0)
        {
            if (child == null) return;
            if (generation > 2) return;

            Generation = generation;
            maxGeneration = Math.Max(maxGeneration, Generation);

            var pn = child.PrimaryName;
            Name = $"{pn?.Surname} {pn?.Given} - {child.PersonId}";

            foreach (var family in child.Families)
            {
                var relation = getRelation(db, child, family);  // query the parents relationship of this family from the childTable

                if (family.Husband != null && relation.father == RelationShip.Birth) // we have found the father with birth relationship
                {
                    this.Husband = new SimplePerson(db, family.Husband, Generation + 1);
                }

                if (family.Wife != null && relation.mother == RelationShip.Birth) // we have found the father with birth relationship
                {
                    this.Wife = new SimplePerson(db, family.Wife);
                }
            }
        }

        public void Print(int posX, int posY)
        {
            //Console.SetCursorPosition(posX, posY);
            //Console.WriteLine(Name);
            //Father?.Print(posX + 5, posY + 1+ (maxGeneration-Generation));
            //Mother?.Print(posX + 5, posY - 1 + (maxGeneration - Generation));
        }


        private int Generation { get; }
        private (RelationShip father, RelationShip mother) getRelation(DB db, Person child, Family family)
        {
            var childTableEntry = db.ChildTable.Single(e => e.ChildId == child.PersonId && e.Family == family);
            return (childTableEntry.RelFather, childTableEntry.RelMother);
        }
    }






}






