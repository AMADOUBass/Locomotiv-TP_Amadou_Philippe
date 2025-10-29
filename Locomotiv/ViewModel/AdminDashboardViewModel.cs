using Locomotiv.Utils;
using Locomotiv.Utils.Commands;
using Locomotiv.Utils.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Locomotiv.ViewModel
{
    public class AdminDashboardViewModel : BaseViewModel
    {
        private readonly ApplicationDbContext _context;
        private readonly INavigationService _navigation;

        public ObservableCollection<Station> Stations { get; }
        public ObservableCollection<User> Users { get; }

        // 📅 Arrivées et départs liés à la station sélectionnée
        public ObservableCollection<Train> Arrivees { get; } = new();
        public ObservableCollection<Train> Departs { get; } = new();

        // 🚆 Trains en mouvement (à afficher sur la carte)
        public ObservableCollection<Train> TrainsEnMouvement { get; } = new();

        public ICommand NavigateToStationManagerCommand { get; }
        public ICommand NavigateToUserManagerCommand { get; }


        // 🗺️ Station sélectionnée sur la carte
        private Station _selectedStation;
        public Station SelectedStation
        {
            get => _selectedStation;
            set
            {
                if (_selectedStation != value)
                {
                    _selectedStation = value;
                    OnPropertyChanged(nameof(SelectedStation));
                    //LoadStationDetails(_selectedStation);
                }
            }
        }
       

        public AdminDashboardViewModel(ApplicationDbContext context, INavigationService navigation)
        {
            _context = context;
            _navigation = navigation;

            //// Chargement initial des données
            //Stations = new ObservableCollection<Station>(_context.Stations.ToList());
            //Users = new ObservableCollection<User>(_context.Users.ToList());

            //TrainsEnMouvement = new ObservableCollection<Train>(
            //    _context.Trains.Where(t => t.EstEnTransit).ToList()
            //);

            //NavigateToStationManagerCommand = new RelayCommand(() =>
            //    _navigation.NavigateTo<StationManagerViewModel>());

            //NavigateToUserManagerCommand = new RelayCommand(() =>
            //    _navigation.NavigateTo<UserManagerViewModel>());

        }
        // 🔍 Méthode appelée quand une station est sélectionnée
        //private void LoadStationDetails(Station station)
        //{
        //    Arrivees.Clear();
        //    Departs.Clear();

        //    if (station?.Trains != null)
        //    {
        //        foreach (var train in station.Trains)
        //        {
        //            if (train.ArriveePrevue.HasValue)
        //                Arrivees.Add(train);
        //            if (train.DepartPrevu.HasValue)
        //                Departs.Add(train);
        //        }
        //    }
        //}

        // 📡 Méthode pour exposer les stations à la carte
        public IEnumerable<Station> GetStationsForMap() => Stations;
    }
}

