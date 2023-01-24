namespace SharpTorrent.MVVM.ViewModels
{
    internal class SharpTorrentViewModel : Base.ViewModel
    {
        //public HomeViewModel HomeVM { get; set; }
        public ListingViewModel ListingViewModel { get; set; }
        public DetailsViewModel DetailsViewModel { get; set; }

        //private object currentView;
        //public object CurrentView
        //{
        //    get => currentView;
        //    set => Set(ref currentView, value);
        //}
        public SharpTorrentViewModel(
            SharpTorrentStore sharpTorrentStore,
            SelectedModelStore selectedModelStore)
        {
            ListingViewModel = new ListingViewModel(sharpTorrentStore, selectedModelStore);
            DetailsViewModel = new DetailsViewModel(selectedModelStore);
            
            
            //HomeVM = new HomeViewModel();
            //CurrentView = HomeVM;
        }
    }
}
