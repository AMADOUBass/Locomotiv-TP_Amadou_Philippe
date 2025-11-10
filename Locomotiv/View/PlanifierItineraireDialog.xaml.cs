using Locomotiv.Model;
using Locomotiv.Model.Interfaces;
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
using System.Windows.Shapes;

namespace Locomotiv.View
{
    /// <summary>
    /// Logique d'interaction pour PlanifierItineraireDialog.xaml
    /// </summary>
    public partial class PlanifierItineraireDialog : Window
    {
        public Train TrainSélectionné { get; private set; }
        public List<PointArret> ArretsSélectionnés { get; private set; }

        public PlanifierItineraireDialog(List<Train> trains, List<PointArret> pointsArret)
        {
            InitializeComponent();
            cmbTrain.ItemsSource = trains;
            lstArrets.ItemsSource = pointsArret;
        }

        private void BtnValider_Click(object sender, RoutedEventArgs e)
        {
            if (cmbTrain.SelectedItem is not Train train || lstArrets.SelectedItems.Count == 0)
            {
                MessageBox.Show("Veuillez sélectionner un train et au moins un arrêt.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (chkRespecteRegles.IsChecked != true)
            {
                MessageBox.Show("Les règles de sécurité doivent être respectées.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            TrainSélectionné = train;
            ArretsSélectionnés = lstArrets.SelectedItems.Cast<PointArret>().ToList();

            DialogResult = true;
            Close();
        }

        private void BtnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }


    }
}
