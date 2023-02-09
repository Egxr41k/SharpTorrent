using MonoTorrent;
using MonoTorrent.Client;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;


namespace SharpTorrent.MVVM.ViewModels;
internal class DetailsViewModel : Base.ViewModel
{
    //public static Task? CurrentTask;
    //public static Thread? CurrentThread;

    #region updatable property
    public string Text
    {
        get => text;
        set => Set(ref text, value);
    }
    private string text =
            "//////////////////////////////////////////////////////////////////\r\n" +
            "\r\n" +
            "Hello.\r\n" +
            "Im am Egxr41k, Ukrainian teenager and .Net developer.\r\n" +
            "\r\n" +
            "Welcome to SharpTorrent - free and open source bit-torrent\r\n" +
            "client, based on MonoTorrent - library by Alan MacGovern:\r\n" +
            "https://github.com/alanmcgovern/monotorrent\r\n" +
            "\r\n" +
            "I hope that your user experiens of SharpTorrent usage won`t\r\n" +
            "be so bad, but if application crashing, or not working correctly,\r\n" +
            "please, text me on telegram(username already has been written) or\r\n" +
            "email: egor2005krava@gmail.com, and describe problew that you find.\r\n" +
            "I`m student so i havent many to paid QA engenier :) thanks.\r\n" +
            "\r\nCheak my another projects here:\r\n" +
            "https://github.com/Egxr41k\r\n" +
            "\r\n" +
            "//////////////////////////////////////////////////////////////////";

    public int ProgressBarValue
    {
        get => progrssBarValue;
        set => Set(ref progrssBarValue, value);
    }
    private int progrssBarValue;

    //make updatable property
    public string EventListnerOutput
    {
        get
        {
            string output = "";
            foreach (string str in SelectedModel.Last10Messages)
            {
                output += str + "\n";
            }
            return output;
        }
    }
    #endregion

    public string TorrentName => SelectedModel?.TorrentName ?? "Unknown";
    public string Id => SelectedModel?.Id.ToString() ?? "Unknown";
    public bool HasSelectedModel => SelectedModel != null;
    public TorrentManager Manager => SelectedModel.Manager;

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
        OnpropertyChanged(nameof(HasSelectedModel));
        OnpropertyChanged(nameof(TorrentName));
        OnpropertyChanged(nameof(Id));
        OnpropertyChanged(nameof(Manager));


        //if (CurrentTask != null) App.cancellation.Cancel();
        

        if (Manager != null)
        {
            if (Manager.State == TorrentState.Stopped)
                await Manager.StartAsync();

            if(SelectedModel.Task == null)
            {
                SelectedModel.Task = new (() =>
                {
                    StringBuilder output = new();

                    while (Manager?.Progress != 100.00 && Manager != null)
                    {
                        Text = _selectedModelStore.
                            SelectedModel.GetCurrentInfo(output).Result;
                    }
                }, App.cancellation.Token
                );
                SelectedModel.Task.Start();
            }
        }
        else Text = "No active downloads here";
    }
}

