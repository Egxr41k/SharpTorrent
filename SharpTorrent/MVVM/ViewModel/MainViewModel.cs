using MonoTorrent;
using SharpTorrent.MVVM.Model;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SharpTorrent.MVVM.ViewModel
{
    internal class MainViewModel : Base.ViewModel
    {
        public HomeViewModel HomeVM { get; set; }
        public TorrentsMenuViewModel TorrentsMenuVM { get; set; }

        public Base.Command? MaximizeCommand { get; set; }
        public Base.Command? MinimizeCommand { get; set; }
        public Base.Command? CloseCommand { get; set; }

        #region Icons
        public BitmapImage MinimizeIcon
        {
            get => minimizeIcon;
            set => Set(ref minimizeIcon, value);
        }
        private BitmapImage minimizeIcon;

        public BitmapImage MaximizeIcon
        {
            get => maximizeIcon;
            set => Set(ref maximizeIcon, value);
        }
        private BitmapImage maximizeIcon;

        public BitmapImage CloseIcon
        {
            get => closeIcon;
            set => Set(ref closeIcon, value);
        }
        private BitmapImage closeIcon;

        #endregion

        public static Window MainWindow
        {
            get => Application.Current.MainWindow;
            set => Application.Current.MainWindow = value;
        }
        private object currentView;
        public object CurrentView
        {
            get => currentView;
            set => Set(ref currentView, value);
        }
        //public static Base.ViewModel tempView
        //{
        //    set
        //    {
        //        CurrentView = value;
        //    }
        //}
        //public void SetView()
        //{
        //    CurrentView = tempView;
        //}

        public object MenuControl
        {
            get => menuControl;
            set => Set(ref menuControl, value);
        }
        private object menuControl;

        private void AppCommandsInit()
        {
            MinimizeCommand = new(o =>
            {
                MainWindow.WindowState = WindowState.Minimized;
            });

            MaximizeCommand = new(o =>
            {
                if (MainWindow.WindowState == WindowState.Normal)
                    MainWindow.WindowState = WindowState.Maximized;
                else MainWindow.WindowState = WindowState.Normal;
            });

            CloseCommand = new(o => MainWindow.Close());
        }
        private void IconsInit()
        {
            string IconsDirectory = MainModel.RESOURCEPATH + "\\Icons";
            string[] IconsFiles = Directory.GetFiles(IconsDirectory);
            
            closeIcon    = new BitmapImage(new Uri(IconsFiles[0]));
            maximizeIcon = new BitmapImage(new Uri(IconsFiles[1]));
            minimizeIcon = new BitmapImage(new Uri(IconsFiles[2]));
        }

        public MainViewModel()
        {
            MenuControl = new TorrentsMenuViewModel();
            CurrentView = new HomeViewModel();

            IconsInit();
            AppCommandsInit();
        }
    }
}
