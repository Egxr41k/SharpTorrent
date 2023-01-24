namespace SharpTorrent.MVVM.ViewModels;

internal class MainViewModel : Base.ViewModel
{
    public SharpTorrentViewModel SharpTorrentViewModel { get; set; }
    public MainViewModel(SharpTorrentViewModel sharpTorrentViewModel)
    {
        SharpTorrentViewModel = sharpTorrentViewModel;
    }
}
