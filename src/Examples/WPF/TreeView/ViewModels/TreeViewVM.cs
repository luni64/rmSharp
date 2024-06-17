using rmSharp;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Diagnostics.Eventing.Reader;
using System.Collections.ObjectModel;
using Task = System.Threading.Tasks.Task;


namespace ViewModels
{
    public class TreeItem
    {
        public List<TreeItem> Nodes { get; } = [];
    }
    public class FamilyVM : TreeItem
    {
        public PersonVM Husband { get; }
        public PersonVM Wife { get; }

        public FamilyVM(rmSharp.Family family)
        {
            //db.Entry(family).Reference(f => f.Husbands).Load();
            //db.Entry(family).Reference(f => f.Wife).Load();
            // db.Entry(family).Collection(f=>f.ChildInfos).Load();

            //db.Entry(family).Collection(f => f.ChildInfos).Query().Include(f => f.Child).Load();

            this.family = family;
            Husband = new PersonVM(family.Husband);
            Wife = new PersonVM(family.Wife);
            foreach (var child in family.Children)
            {
                if (child.Families.Count > 0)
                {
                    Nodes.AddRange(child.Families.Select(f => new FamilyVM(f)));
                }
                else
                {
                    Nodes.Add(new PersonVM(child));
                }

            }
        }

        private Family family;
        public override string ToString() => family.ToString();
    }

    public class PersonVM : TreeItem
    {
        public string Name { get; }
        public PersonVM(Person p)
        {
            this.person = p;
            if (person != null)
            {
                string name = person.PrimaryName.ToString();
                string bday = person.Events.Where(e => e.FactType.Name == "Birth").FirstOrDefault()?.Date.year.ToString() ?? "?";
                string dDay = person.Events.Where(e => e.FactType.Name == "Death").FirstOrDefault()?.Date.year.ToString() ?? "?";

                Name = $"{name} ({bday}-{dDay})";
            }
            else Name = "UNKN";


        }

        Person person;

        public override string ToString() => person?.ToString() ?? "";
    }

    public class TreeViewVM : BaseViewModel
    {       
        public ObservableCollection<FamilyVM>? Families { get; private set; }

        public async Task LoadAsync(string name)
        {
            Families = null;
            OnPropertyChanged("Families");

            await Task.Run(() =>
            {
                using (var db = new DB())
                {
                    var names = db.Names
                    .Where(n => n.IsPrimary && n.Surname == name)
                    .Select(n => n.OwnerId);
                    
                    var f = db.Families
                    .Where(f => names.Contains(f.HusbandId) || names.Contains(f.WifeId))
                    .Select(f => new FamilyVM(f));

                    Families = new ObservableCollection<FamilyVM>(f.ToList());
                    OnPropertyChanged("Families");
                }
            });            
        }
     

    }
}