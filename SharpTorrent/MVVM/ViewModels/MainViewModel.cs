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

        

        public MainViewModel()
        {
            MenuControl = new TorrentsMenuViewModel();
            CurrentView = new HomeViewModel();
        }
    }
}
