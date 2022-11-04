using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpTorrent.MVVM.ViewModel.Base;
using Prism.Commands;
using Prism.Mvvm;


namespace DialogSample
{
    internal class MainViewModel : SharpTorrent.MVVM.ViewModel.Base.ViewModel
    {
        DialogService _dialogService = new DialogService();
        private Command _showDialog;

        public Command ShowDialog =>
            _showDialog ?? (_showDialog = new Command(o =>
            {
                _dialogService.ShowDialog();
            }));

        //void ExecuteShowDialog()
        //{
        //    _dialogService.ShowDialog();
        //}
    }
}
