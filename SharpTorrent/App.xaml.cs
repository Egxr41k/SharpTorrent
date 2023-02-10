global using SharpTorrent.MVVM.ViewModels;
global using SharpTorrent.Commands;
global using SharpTorrent.MVVM.Models;
global using SharpTorrent.Stores;

using System.Windows;
using System.Threading;

namespace SharpTorrent
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static CancellationTokenSource cancellation;

        private readonly SelectedModelStore _selectedModelStore;
        private readonly SharpTorrentStore _sharpTorrentStore;
        private readonly SharpTorrentViewModel _sharpTorrentViewModel;

        public App()
        {
            cancellation = new CancellationTokenSource();
            _sharpTorrentStore = new SharpTorrentStore();
            _selectedModelStore = new SelectedModelStore
                (_sharpTorrentStore);

            _sharpTorrentViewModel = new SharpTorrentViewModel
                (_sharpTorrentStore, _selectedModelStore);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel(_sharpTorrentViewModel)
            };
            MainWindow.Show();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            foreach(var torrent in
                _sharpTorrentViewModel.ListingViewModel.ActiveTorrents)
            {
                torrent.SharpTorrentModel.Manager?.StopAsync();
                cancellation.Cancel();
            }
            
            base.OnExit(e);

        }
    }
}
