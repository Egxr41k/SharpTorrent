using MonoTorrent;
using MonoTorrent.Client;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace SharpTorrent.MVVM.Model
{
    static class MainModel
    {
        public static string RESOURCEPATH =
            Path.GetFullPath(
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    @"..\..\..\Resources"));

        public static Top10Listener Listener = new(10);
        public static ClientEngine Engine = new();
        public static StringBuilder Output = new(1024);
        //public static TorrentManager Manager;


        public static async Task<TorrentManager> DownloadAsync(
            string TorrentPath, string DownloadPath)

        {
            TorrentManager Manager;
            try
            {
                var settingsBuilder = new TorrentSettingsBuilder();
                Manager = await Engine.AddAsync(
                    await Torrent.LoadAsync(
                        TorrentPath ?? "#"),
                    DownloadPath ?? "#",
                    new TorrentSettingsBuilder()
                        .ToSettings());
            }
            catch (Exception e) { Console.WriteLine("Couldn't decode file. " + e.Message); }

            Manager = Engine.Torrents[0];

            #region events
            Manager.PeersFound += delegate (object? sender, PeersAddedEventArgs e)
            {
                lock (Listener)
                    Listener.WriteLine($"Found {e.NewPeers} new peers and {e.ExistingPeers} existing peers");
            };

            Manager.PeerConnected += (o, e) =>
            {
                lock (Listener)
                    Listener.WriteLine($"Connection succeeded: {e.Peer.Uri}");
            };

            Manager.ConnectionAttemptFailed += (o, e) =>
            {
                lock (Listener)
                    Listener.WriteLine(
                        $"Connection failed: {e.Peer.ConnectionUri} - {e.Reason}");
            };

            Manager.PieceHashed += delegate (object? o, PieceHashedEventArgs e)
            {
                lock (Listener)
                    Listener.WriteLine($"Piece Hashed: {e.PieceIndex} - {(e.HashPassed ? "Pass" : "Fail")}");
            };

            Manager.TorrentStateChanged += delegate (object? o, TorrentStateChangedEventArgs e)
            {
                lock (Listener)
                    Listener.WriteLine($"OldState: {e.OldState} NewState: {e.NewState}");
            };

            Manager.TrackerManager.AnnounceComplete += (sender, e) =>
            {
                Listener.WriteLine($"{e.Successful}: {e.Tracker}");
            };

            #endregion

            await Manager.StartAsync();
            return Manager;
        }
    }
}
