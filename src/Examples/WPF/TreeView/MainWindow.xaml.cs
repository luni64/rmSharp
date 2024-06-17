using Microsoft.EntityFrameworkCore;
//using rmSharp;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ViewModels;

namespace TreeView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // private readonly DB db =new DB();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //   db.Persons.Load();
        }


        private void MainWinClosing(object sender, CancelEventArgs e)
        {
            if (DataContext is MainVM vm)
            {
                // vm.OnWindowClosing(sender, e);
            }
        }

        TreeWin tw;

        private async void NigglClick(object sender, RoutedEventArgs e)
        {
            if (tw == null)
            {
                tw = new TreeWin();
                tw.Show();
            }

            if (tw.DataContext is TreeViewVM vm)
            {
                await vm.LoadAsync("Niggl");
            }

        }

        private async void OberhauserClick(object sender, RoutedEventArgs e)
        {
            if (tw == null)
                tw = new TreeWin();
            tw.Show();

            if (tw.DataContext is TreeViewVM vm)
            {
                Cursor = Cursors.Wait;
                await vm.LoadAsync("Oberhauser");
                Cursor = Cursors.Arrow;
            }

        }

        private async void Person_Click(object sender, RoutedEventArgs e)
        {

            var view = new PersonView();
            view.Show();



        }
    }
}