using rmSharp;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Diagnostics.Eventing.Reader;


namespace ViewModels
{
   public class pVM:BaseViewModel
    {
        
    }


    public class MainVM : BaseViewModel
    {
       // TreeViewVM treeViewVM;

        public MainVM()
        {
            DB.sqLiteFile = "oberhauser-niggl.rmTree";  // database file

          //  var p = new PersonListVM();

        }

       
    }
}