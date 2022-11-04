using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogSample
{
    public interface IDialogService
    {
        void ShowDialog(string name);
    }
    internal class DialogService : IDialogService
    {
        public void ShowDialog(string name)
        {
            var dialog = new DialogWindow();

            var type = Type.GetType($"DialogSample.{name}");

            dialog.Content = Activator.CreateInstance(type);

            dialog.ShowDialog();
        }
    }
}
