using System.Threading.Tasks;
using System.Windows.Input;

namespace SharpTorrent.MVVM.ViewModels;

internal class ListingItemViewModel : Base.ViewModel
{
    public SharpTorrentModel SharpTorrentModel { get; private set; }

    public string TorrentName => SharpTorrentModel.TorrentName;

    public int ProgressBarValue
    {
        get => progrssBarValue;
        set => Set(ref progrssBarValue, value);
    }
    private int progrssBarValue;

    public int PercentComplete =>
     (int)SharpTorrentModel.Manager?.Progress;


    //public ICommand EditCommand { get; set; }
    //public ICommand DeleteCommand { get; set; }

    public ListingItemViewModel(SharpTorrentModel model, SharpTorrentStore sharpTorrentStore)
    {
        SharpTorrentModel = model;
        new Task(() =>
        {
            while (PercentComplete != 100.00)
                ProgressBarValue = PercentComplete;
        }, App.cancellation.Token)
            .Start();
    }
    

    internal void Update(SharpTorrentModel model)
    {
        SharpTorrentModel = model;

        OnpropertyChanged(nameof(SharpTorrentModel));
    }
}
