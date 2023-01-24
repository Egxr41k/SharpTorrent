global using SharpTorrent.MVVM.ViewModels;
global using SharpTorrent.Commands;
global using SharpTorrent.MVVM.Models;
global using SharpTorrent.Stores;

using System.Windows;

namespace SharpTorrent
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly SelectedModelStore _selectedModelStore;
        //private readonly ModalNavigationStore _modalNavigationStore;
        private readonly SharpTorrentStore _sharpTorrentStore;

        public App()
        {
            _sharpTorrentStore = new SharpTorrentStore();
            //_modalNavigationStore = new ModalNavigationStore();
            _selectedModelStore = new SelectedModelStore(_sharpTorrentStore);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var sharpTorrentViewModel =
                new SharpTorrentViewModel(_sharpTorrentStore,
                _selectedModelStore);

            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(sharpTorrentViewModel)
            };
            MainWindow.Show();

            base.OnStartup(e);
        }
    }
}
