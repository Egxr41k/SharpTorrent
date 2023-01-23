using SharpTorrent.MVVM.View;
using SharpTorrent.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SharpTorrent
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //private readonly SelectedListAppModelStore _selectedListAppModelStore;
        //private readonly ModalNavigationStore _modalNavigationStore;
        //private readonly ListAppStore _listAppStore;

        public App()
        {
            //_listAppStore = new ListAppStore();
            //_modalNavigationStore = new ModalNavigationStore();
            //_selectedListAppModelStore = new SelectedListAppModelStore(_listAppStore);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel()
            };
            MainWindow.Show();

            base.OnStartup(e);
        }
    }
}
