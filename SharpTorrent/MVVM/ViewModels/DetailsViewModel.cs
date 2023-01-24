namespace SharpTorrent.MVVM.ViewModels
internal class DetailsViewModel : Base.ViewModel
{
    //public TextRange Text
    //{
    //    get => text;
    //    set => Set(ref text, value);
    //}
    //private TextRange text;

    //public bool IsActive { get; set; }

    //public string TorrentName
    //{
    //    get => torrentName;
    //    set => Set(ref torrentName, value);
    //}
    //private string torrentName = "1";
    //public string TorrentPath { get; set; }

    //public int ProgressBarValue
    //{
    //    get => progrssBarValue;
    //    set => Set(ref progrssBarValue, value);
    //}
    //private int progrssBarValue;

    public string TorrentName => SelectedModel?.TorrentName ?? "Unknown";
    public string Id => SelectedModel?.Id.ToString() ?? "Unknown";

    public bool HasSelectedModel => SelectedModel != null;


    private readonly SelectedModelStore _selectedModelStore;
    private SharpTorrentModel SelectedModel => _selectedModelStore.SelectedModel;

    public DetailsViewModel(SelectedModelStore selectedModelStore)
    {
        
        _selectedModelStore = selectedModelStore;
        _selectedModelStore.SelectedModelChanged += _selectedModelStore_SelectedModelChanged;
    }

    protected override void Dispose()
    {
        _selectedModelStore.SelectedModelChanged -= _selectedModelStore_SelectedModelChanged;
        base.Dispose();
    }
    private void _selectedModelStore_SelectedModelChanged()
    {
        OnpropertyChanged(nameof(HasSelectedModel));
        OnpropertyChanged(nameof(TorrentName));
        OnpropertyChanged(nameof(Id));
    }
}

