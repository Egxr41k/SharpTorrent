using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SharpTorrent.MVVM.View
{
    /// <summary>
    /// Interaction logic for TorrentView.xaml
    /// </summary>
    public partial class TorrentView : UserControl
    {
        public TorrentView()
        {
            InitializeComponent();
            var dc = DataContext;
        }
    }
}
