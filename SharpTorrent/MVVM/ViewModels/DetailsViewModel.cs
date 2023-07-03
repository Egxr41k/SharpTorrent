using System.Text;


namespace SharpTorrent.MVVM.ViewModels;
internal class DetailsViewModel : Base.ViewModel
{
    public string Text
    {
        get => text;
        set => Set(ref text, value);
    }
    private string text =
            "//////////////////////////////////////////////////////////////////\r\n" +
            "\r\n" +
            "Hello.\r\n" +
            "I am Egxr41k, a Ukrainian teenager and .Net developer.\r\n" +
            "\r\n" +
            "Welcome to SharpTorrent - free and open source bit-torrent\r\n" +
            "client, based on MonoTorrent - library by Alan MacGovern:\r\n" +
            "https://github.com/alanmcgovern/monotorrent\r\n" +
            "\r\n" +
            "I hope that your user experience of SharpTorrent usage won`t\r\n" +
            "be as bad, but if the application crashes or doesn't work correctly,\r\n" +
            "Please, contact me on telegram(@Egxr41k) or via\r\n" +
            "email: egor2005krava@gmail.com, and report any problem you stumble upon.\r\n" +
            "I`m a student so I don't have enough to pay the QA engineer :) thanks.\r\n" +
            "\r\nCheck my other projects here:\r\n" +
            "https://github.com/Egxr41k\r\n" +
            "\r\n" +
            "//////////////////////////////////////////////////////////////////";

    public MonoTorrent.Client.TorrentManager? Manager => SelectedModel.Manager;

    private readonly SelectedModelStore _selectedModelStore;
    private SharpTorrentModel SelectedModel => _selectedModelStore.SelectedModel;

    public DetailsViewModel(SelectedModelStore selectedModelStore)
    {
        _selectedModelStore = selectedModelStore;
        _selectedModelStore.SelectedModelChanged += 
            _selectedModelStore_SelectedModelChanged;
    }

    protected override void Dispose()
    {
        _selectedModelStore.SelectedModelChanged -= 
            _selectedModelStore_SelectedModelChanged;
        base.Dispose();
    }

    private async void _selectedModelStore_SelectedModelChanged()
    {       
        if (Manager != null)
        {
            if (Manager.State == MonoTorrent.Client.TorrentState.Stopped)
                await Manager.StartAsync();

            if(SelectedModel.Task == null)
            {
                SelectedModel.Task = new (() =>
                {
                    StringBuilder output = new();

                    while (Manager.Progress != 100.00)
                    {
                        Text = _selectedModelStore.
                            SelectedModel.GetCurrentInfo(output).Result;
                        if (Manager == null) break;
                    }
                }, App.cancellation.Token);

                SelectedModel.Task.Start();
            }
        }
        else Text = "No active downloads here";
    }
}

