using GMap.NET;
using Locomotiv.Model.DAL;
using Locomotiv.Model.Interfaces;
using Locomotiv.Utils;
using Locomotiv.Utils.Commands;
using Locomotiv.Utils.Services;
using Locomotiv.Utils.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static User;

namespace Locomotiv.ViewModel
{
    public class AdminDashboardViewModel : BaseViewModel
    {
        public readonly ITrainDAL _trainDAL;
        private readonly IStationDAL _stationDAL;
        private readonly IDialogService _dialogService;
        private readonly IBlockDAL _blockDAL;
        public IEnumerable<Station> GetStations() => Stations;
        public IEnumerable<Train> GetTrainsEnMouvement() => TrainsEnMouvement;
        public ObservableCollection<Block> Blocks { get; set; } = new();

        private void LoadBlocks()
        {
            var blocks = _blockDAL.GetAllBlocks();
            Blocks = new ObservableCollection<Block>(blocks);
        }
        public ObservableCollection<Train> Trains { get; set; } = new ObservableCollection<Train>();
        public ObservableCollection<Station> Stations { get; set; } = new ObservableCollection<Station>();

        public ObservableCollection<Train> TrainsEnMouvement =>
            new ObservableCollection<Train>(Trains.Where(t => t.Etat == EtatTrain.EnTransit));

        public ObservableCollection<Train> ArriveesDeLaStationSelectionnee =>
            new ObservableCollection<Train>(Trains.Where(t => t.Station?.Id == StationSelectionnee?.Id && t.Etat == EtatTrain.EnGare));

        public ObservableCollection<Train> DepartsDeLaStationSelectionnee =>
            new ObservableCollection<Train>(Trains.Where(t => t.Station?.Id == StationSelectionnee?.Id && t.Etat == EtatTrain.EnAttente));






        public ObservableCollection<Train> TrainsDeLaStationSelectionnee =>
            new ObservableCollection<Train>(Trains.Where(t => t.Station?.Id == StationSelectionnee?.Id)
        );

        public ObservableCollection<string> ConflitsDeLaStationSelectionnee =>
                new ObservableCollection<string>(
                GetConflits()
                .Where(c => c.TrainA.Station?.Id == StationSelectionnee?.Id || c.TrainB.Station?.Id == StationSelectionnee?.Id)
                .Select(c => $"⚠️ {c.TrainA.Nom} et {c.TrainB.Nom} sur {c.BlockConflit.Nom}")
                );

        public ICommand AjouterTrainCommand { get; }
        public ICommand SupprimerTrainCommand { get; }

        private Train? _selectedTrain;
        public Train? SelectedTrain
        {
            get => _selectedTrain;
            set
            {
                _selectedTrain = value;
                OnPropertyChanged(nameof(SelectedTrain));
                OnPropertyChanged(nameof(Train));
            }
        }
        private Station? _stationSelectionnee;
        public Station? StationSelectionnee
        {
            get => _stationSelectionnee;
            set
            {
                _stationSelectionnee = value;
                OnPropertyChanged(nameof(StationSelectionnee));
                OnPropertyChanged(nameof(TrainsDeLaStationSelectionnee));
                OnPropertyChanged(nameof(ConflitsDeLaStationSelectionnee));
                //OnPropertyChanged(nameof(ArriveesDeLaStationSelectionnee));
                //OnPropertyChanged(nameof(DepartsDeLaStationSelectionnee));

            }
        }



        public AdminDashboardViewModel(ITrainDAL trainDAL, IDialogService dialogService,IStationDAL station,IBlockDAL block)
        {
            _trainDAL = trainDAL;
            _dialogService = dialogService;
            _stationDAL = station;
            _blockDAL = block;
            AjouterTrainCommand = new RelayCommand(AddTrain);
            SupprimerTrainCommand = new RelayCommand(DeleteTrain);
            LoadStations();
            LoadTrains();
            LoadBlocks();

        }
        private void LoadStations()
        {
            var stations = _stationDAL.GetAllStations();
            Stations = new ObservableCollection<Station>(stations);
        }

        private void LoadTrains()
        {
            var trains = _trainDAL.GetAllTrains();
            Trains = new ObservableCollection<Train>(trains);
        }

        private void AddTrain()
        {
            if (_dialogService.ShowTrainDialog(out Train train))
            {
                _trainDAL.AddTrain(train);
                Trains.Add(train);
                _dialogService.ShowMessage($"Train '{train.Nom}' ajouté avec succès!", "Ajout de Train");
            }
        }

        private void DeleteTrain()
        {
            //if (SelectedTrain == null) return;

            //if (_dialogService.ShowConfirmation($"Voulez-vous vraiment supprimer '{SelectedTrain.Nom}' ?"))
            //{
            //    _trainDAL.DeleteTrain(SelectedTrain.Id);
            //    Trains.Remove(SelectedTrain);
            //    _dialogService.ShowMessage($"Train '{SelectedTrain.Nom}' supprimé avec succès!", "Suppression de Train");
            //}
        }
        private double DistanceKm(PointLatLng a, PointLatLng b)
        {
            const double R = 6371; // Rayon terrestre en km
            var dLat = (b.Lat - a.Lat) * Math.PI / 180;
            var dLon = (b.Lng - a.Lng) * Math.PI / 180;
            var lat1 = a.Lat * Math.PI / 180;
            var lat2 = b.Lat * Math.PI / 180;

            var aCalc = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                        Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            var c = 2 * Math.Atan2(Math.Sqrt(aCalc), Math.Sqrt(1 - aCalc));
            return R * c;
        }

        public List<(Train TrainA, Train TrainB, Block BlockConflit)> GetConflits()
        {
            var conflits = new List<(Train, Train, Block)>();

            var trainsEnTransit = Trains
                .Where(t => t.Etat == EtatTrain.EnTransit && t.BlockId != null)
                .ToList();

            var blocksOccupes = Blocks
                .Where(b => b.EstOccupe)
                .ToDictionary(b => b.Id, b => b);

            for (int i = 0; i < trainsEnTransit.Count; i++)
            {
                var trainA = trainsEnTransit[i];
                var blockA = blocksOccupes.GetValueOrDefault(trainA.BlockId!.Value);
                if (blockA == null) continue;

                for (int j = i + 1; j < trainsEnTransit.Count; j++)
                {
                    var trainB = trainsEnTransit[j];
                    var blockB = blocksOccupes.GetValueOrDefault(trainB.BlockId!.Value);
                    if (blockB == null) continue;

                    // ✅ Cas 1 : même block occupé par deux trains
                    if (trainA.BlockId == trainB.BlockId)
                    {
                        conflits.Add((trainA, trainB, blockA));
                        continue;
                    }

                    // ✅ Cas 2 : blocks différents mais trop proches
                    var centerA = new PointLatLng(
                        (blockA.LatitudeDepart + blockA.LatitudeArrivee) / 2,
                        (blockA.LongitudeDepart + blockA.LongitudeArrivee) / 2
                    );

                    var centerB = new PointLatLng(
                        (blockB.LatitudeDepart + blockB.LatitudeArrivee) / 2,
                        (blockB.LongitudeDepart + blockB.LongitudeArrivee) / 2
                    );

                    double distanceKm = DistanceKm(centerA, centerB);

                    if (distanceKm < 1.0)
                    {
                        conflits.Add((trainA, trainB, blockA));
                    }
                }
            }

            //_dialogService.ShowMessage($"Nombre de conflits détectés : {conflits.Count}", "Conflits de Trains");
            return conflits;
        }

        private ObservableCollection<string> _conflitsTextuels = new();
        public ObservableCollection<string> ConflitsTextuels
        {
            get => _conflitsTextuels;
            set
            {
                _conflitsTextuels = value;
                OnPropertyChanged(nameof(ConflitsTextuels));
            }
        }


    }
}