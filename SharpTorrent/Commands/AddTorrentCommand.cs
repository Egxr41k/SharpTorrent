using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SharpTorrent.Commands;

internal class AddTorrentCommand : Base.AsyncCommand
{
    public override Task ExucuteTask(object parameter)
    {
        OpenFileDialog ofd = new()
        {
            Filter = "Torrent Files(*.torrent)|*.torrent"
        };

        if (ofd.ShowDialog() == true)
        {
            string DownloadFile = ofd.FileName;

            string dirName = DownloadFile.Split('\\').Last().Split('.').First();
            string dirPath = Path.GetDirectoryName(ofd.FileName) ??
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string SaveDirectory = Path.Combine(dirPath + "\\" + dirName);

            if (!Directory.Exists(SaveDirectory))
                Directory.CreateDirectory(SaveDirectory);

            try
            {
                StartDownload(DownloadFile, SaveDirectory);
            }
            catch (Exception) { }
        }
        return Task.CompletedTask;
    }

    private async void StartDownload(string file, string directory)
    {
        //______________________________
        //
        //MUST BE EDITING BEFORE TESTING
        //______________________________

        await MainModel.DownloadAsync(file, directory);
        await MainModel.Manager.StartAsync();
    }
}
