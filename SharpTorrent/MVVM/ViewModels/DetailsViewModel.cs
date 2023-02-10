﻿using System.Text;


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

