using rmSharp;
using System.ComponentModel;
using System.Windows.Data;
using Task = System.Threading.Tasks.Task;

namespace ViewModels
{
    public class PVM : BaseViewModel
    {
        public string Name => $"{Surname} {Given} ({bd}-{dd})";
        public string? Surname { get;  set; }
        public string? Given { get;  set; }
        public RMDate? bd { get;  set; }
        public RMDate? dd { get;  set; }



        //public PVM(long ID, string Surname, string given, RMDate? birthDay, RMDate? deathDay)
        //{
        //    this.Surname = Surname;
        //    this.Given = given;
        //    this.bd = birthDay;
        //    this.dd = deathDay;
        //}
    }

    public class PersonListVM : BaseViewModel
    {
        private List<PVM>? _persons;


        public PersonListVM()
        {
            using (var db = new DB())
            {
                var _persons = db.Persons.Select(p => new PVM
                {
                    // p.PersonId 
                    Surname = p.PrimaryName.Surname,
                    Given = p.PrimaryName.Given,
                    bd = p.Events.First(e => e.FactType.Name == "Birth").Date,
                    dd = p.Events.First(e => e.FactType.Name == "Death").Date
                }).ToList();

                Persons = CollectionViewSource.GetDefaultView(_persons);
                Persons.Filter = p => ((PVM)p).Name.ToUpper().Contains(_filter.ToUpper());
            }
        }


        private string _filter = "";
        public string Filter
        {
            get => _filter;
            set
            {
                SetProperty(ref _filter, value);
                Persons.Refresh();
            }
        }

        private PVM? _selectedPVM = null;
        public PVM? SelectedPVM
        {
            get => _selectedPVM;
            set => SetProperty(ref _selectedPVM, value);
        }

        public ICollectionView Persons { get; }

        public async Task LoadAsync(string name)
        {
            await Task.Run(() =>
            {

            });
        }
    }
}

