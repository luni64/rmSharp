using Microsoft.EntityFrameworkCore;
using RMDatabase;
using RMDatabase.Models;
using System.Globalization;

namespace Example
{
    class AddPerson
    {
        public void Execute()
        {
            using (var db = new DB())
            {
                var Pete = new Person("Miller", "Peter", Sex.Male);
                var Mary = new Person("Taylor", "Mary", Sex.Female);                
                var Thomas = new Person("Miller", "Thomas", Sex.Male);
                var Michael = new Person("Taylor", "Michael", Sex.Male);
                
                var family = new Family(Pete, Mary);
                family.AddChild(Thomas);
                var ci = family.AddChild(Michael,relFather: RelationShip.Step);

                var x = Pete.Families;

             

                

                
                               
                
                db.Persons.Add(Pete);
                db.Persons.Add(Mary);
                db.Persons.Add(Thomas);
                db.Families.Add(family);

                //foreach (var child in Pete.Children)
                //{
                //    foreach (var relation in child.ParentRelations)
                //    {
                //        if (relation.Father == Pete && relation.RelFather == RelationShip.Birth)
                //        {
                //            Console.WriteLine(child);
                //        }
                //    }
                //}

                var children = Mary.Children.Where(c => c.ParentRelations.Any(r => r.Mother == Mary && r.RelMother == RelationShip.Birth)).ToList();
                var stepChildren = Mary.Children.Where(c => c.ParentRelations.Any(r => r.Mother == Mary && r.RelMother == RelationShip.Step)).ToList();




                db.SaveChanges();
            }
        }
    }
}






