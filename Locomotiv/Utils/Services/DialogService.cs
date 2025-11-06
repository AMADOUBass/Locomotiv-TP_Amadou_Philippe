using Locomotiv.Utils.Services.Interfaces;
using Locomotiv.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Locomotiv.Utils.Services
{
    public class DialogService : IDialogService
    {
        public void ShowMessage(string message, string title = "Info")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }
        public bool ShowTrainDialog(out Train train)
        {
            var dialog = new TrainFormDialog();
            var result = dialog.ShowDialog();
            if (result == true)
            {
                train = dialog.Train;
                return true;
            }
            train = null;
            return false;
        }
        public bool ShowPlanifierItineraireDialog(out Itineraire itineraire)
        {
            //var dialog = new ItineraireFormDialog();
            //var result = dialog.ShowDialog();
            //if (result == true)
            //{
            //    itineraire = dialog.Itineraire;
            //    return true;
            //}
            itineraire = null;
            return false;
        }
    }

}
