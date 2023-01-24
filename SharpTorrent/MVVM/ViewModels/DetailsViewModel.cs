﻿using System.Windows.Documents;

namespace SharpTorrent.MVVM.ViewModels;
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
    public string Text => SelectedModel?.Output.ToString() ??


        "//////////////////////////////////////////////////////////////////\r\n" +
        "\r\n" +
        "Hello.\r\n" +
        "Im am Egxr41k, Ukrainian teenager and .Net developer.\r\n" +
        "\r\n" +
        "Welcome to SharpTorrent - free and open source bit-torrent\r\n" +
        "client based on MonoTorrent library by Alan MacGovern:\r\n" +
        "https://github.com/alanmcgovern/monotorrent\r\n" +
        "\r\n" +
        "I hope that your user experiens of SharpTorrent usage won`t\r\n" +
        "be so bad,but if application crashing, or not working correctly,\r\n" +
        "please text me on telegram(username already has been written) or\r\n" +
        "email: egor2005krava@gmail.com, and describe problew that you find.\r\n" +
        "iam student so i havent many to paid QA engenier :) thanks.\r\n" +
        "\r\nCheak my another projects here:\r\n" +
        "https://github.com/Egxr41k\r\n" +
        "//////////////////////////////////////////////////////////////////";



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
        OnpropertyChanged(nameof(Text));
    }
}

