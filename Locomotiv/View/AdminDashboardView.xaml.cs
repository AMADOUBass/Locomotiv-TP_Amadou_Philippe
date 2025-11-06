using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using Locomotiv.ViewModel;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using static Block;
using System.Windows.Shapes;



namespace Locomotiv.View
{
    /// <summary>
    /// Logique d'interaction pour AdminDashboardViewModel.xaml
    /// </summary>
    public partial class AdminDashboardView : UserControl
    {
        private static readonly PointLatLng QuebecCenter = new PointLatLng(46.81750, -71.21376);
        public Station? SelectedStation { get; private set; }

        public AdminDashboardView()
        {
            InitializeComponent();

            //InitMap();

            Loaded += (s, e) =>
                {
                    if (DataContext is AdminDashboardViewModel vm)
                    {
                        InitMap();
                        LoadStationMarkers(vm.GetStations());
                        LoadTrainMarkers(vm.GetTrainsEnMouvement());
                        LoadBlockRoutes(vm.Blocks);

                        var conflits = vm.GetConflits();
                        AfficherConflitsSurCarte(conflits);
                    }
                }
            ;

        }

        private void InitMap()
        {

            GMaps.Instance.Mode = AccessMode.ServerAndCache;

            MapControl.MapProvider = GMapProviders.OpenStreetMap;

            MapControl.Position = QuebecCenter;

            MapControl.MinZoom = 5;
            MapControl.MaxZoom = 18;
            MapControl.Zoom = 13;


            MapControl.CanDragMap = true;
            MapControl.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
            MapControl.IgnoreMarkerOnMouseWheel = true;
            MapControl.ShowCenter = false;


            MapControl.Markers.Clear();

            //AddMarker(QuebecCenter, , Brushes.Black);

        }

        public void LoadStationMarkers(IEnumerable<Station> stations)
        {
            //MapControl.Markers.Clear();
            foreach (var station in stations)
            {
                if (station.Latitude == 0 && station.Longitude == 0)
                {
                    Console.WriteLine($"Coordonnées invalides pour : {station.Nom}");
                    continue;
                }

                var position = new PointLatLng(station.Latitude, station.Longitude);
                var marker = new GMapMarker(position)
                {
                    Shape = new Ellipse
                    {
                        Width = 12,
                        Height = 12,
                        Stroke = Brushes.Blue,
                        StrokeThickness = 2,
                        Fill = Brushes.LightBlue,
                        ToolTip = station.Nom + $" (Capacité: {station.CapaciteMaxTrains} trains)"
                    },
                    Offset = new Point(-6, -6),
                    Tag = station
                };
                marker.Shape.MouseLeftButtonUp += (s, e) =>
                {
                    if (DataContext is AdminDashboardViewModel vm)
                    {
                        vm.StationSelectionnee = marker.Tag as Station;
                        MapControl.Position = position;
                        MapControl.Zoom = 15;
                    }
                };
                MapControl.Markers.Add(marker);

            }
        }

        private Brush GetTrainColor(EtatTrain etat) => etat switch
        {
            EtatTrain.EnGare => Brushes.Blue,
            EtatTrain.EnTransit => Brushes.Orange,
            EtatTrain.EnAttente => Brushes.Gray,
            EtatTrain.HorsService => Brushes.Black,
            EtatTrain.Programme => Brushes.Purple,
            _ => Brushes.LightGray
        };
        public void LoadTrainMarkers(IEnumerable<Train> trains)
        {
            foreach (var train in trains)
            {
                PointLatLng position;

                if (train.Etat == EtatTrain.EnTransit && train.Block != null)
                {
                    // Position au milieu du block
                    var lat = (train.Block.LatitudeDepart + train.Block.LatitudeArrivee) / 2;
                    var lng = (train.Block.LongitudeDepart + train.Block.LongitudeArrivee) / 2;
                    position = new PointLatLng(lat, lng);
                }
                else if (train.Station != null)
                {
                    position = new PointLatLng(train.Station.Latitude, train.Station.Longitude);
                }
                else continue;

                var marker = new GMapMarker(position)
                {
                    Shape = new Ellipse
                    {
                        Width = 10,
                        Height = 10,
                        Stroke = Brushes.Black,
                        StrokeThickness = 2,
                        Fill = GetTrainColor(train.Etat),
                        ToolTip = $"Train: {train.Nom}\nÉtat: {train.Etat}"
                    },
                    Offset = new Point(-5, -5)
                };

                MapControl.Markers.Add(marker);
            }
        }
        private Brush GetBlockColor(SignalType signal) => signal switch
        {
            SignalType.Vert => Brushes.Green,
            SignalType.Rouge => Brushes.Red,
            SignalType.Jaune => Brushes.Yellow,
            _ => Brushes.Gray
        };
        private double GetAngleBetweenPoints(PointLatLng start, PointLatLng end)
        {
            double deltaX = end.Lng - start.Lng;
            double deltaY = end.Lat - start.Lat;
            double angleRad = Math.Atan2(deltaY, deltaX);
            return angleRad * (180 / Math.PI); // convertit en degrés
        }
        public void LoadBlockRoutes(IEnumerable<Block> blocks)
        {
            foreach (var block in blocks)
            {
                var start = new PointLatLng(block.LatitudeDepart, block.LongitudeDepart);
                var end = new PointLatLng(block.LatitudeArrivee, block.LongitudeArrivee);

                // Crée la route visuelle
                var route = new GMapRoute(new List<PointLatLng> { start, end })
                {
                    Shape = new Path
                    {
                        Stroke = GetBlockColor(block.Signal),
                        StrokeThickness = block.EstOccupe ? 20 : 15,
                        Fill = GetBlockColor(block.Signal),
                        Opacity = 0.6,
                        ToolTip = $"🛤️ Block: {block.Nom}\n🚦 Signal: {block.Signal}\n📍 Occupé: {(block.EstOccupe ? "Oui" : "Non")}",
                        IsHitTestVisible = true
                    }
                };

                // Applique les paramètres du tooltip pour qu’il reste visible
                ToolTipService.SetInitialShowDelay(route.Shape, 0);
                ToolTipService.SetShowDuration(route.Shape, 5000);
                ToolTipService.SetPlacement(route.Shape, PlacementMode.Mouse);

                MapControl.Markers.Add(route);

                // Ajoute une flèche directionnelle à l’arrivée
                var directionMarker = new GMapMarker(end)
                {
                    Shape = new Path
                    {
                        Data = System.Windows.Media.Geometry.Parse("M 0,0 L 10,5 L 0,10 Z"), // triangle
                        Fill = Brushes.Black,
                        Width = 10,
                        Height = 10,
                        ToolTip = $"→ {block.Nom}",
                        RenderTransform = new RotateTransform(GetAngleBetweenPoints(start, end)),
                        RenderTransformOrigin = new Point(0.5, 0.5)
                    },
                    Offset = new Point(-5, -5)
                };

                MapControl.Markers.Add(directionMarker);
            }
        }
        public void AfficherConflitsSurCarte(List<(Train TrainA, Train TrainB, Block BlockConflit)> conflits)
        {
            if (conflits == null || conflits.Count == 0) return;

            foreach (var (trainA, trainB, block) in conflits)
            {
                var start = new PointLatLng(block.LatitudeDepart, block.LongitudeDepart);
                var end = new PointLatLng(block.LatitudeArrivee, block.LongitudeArrivee);

                var startLocal = MapControl.FromLatLngToLocal(start);
                var endLocal = MapControl.FromLatLngToLocal(end);

                var strokeBrush = new SolidColorBrush(Colors.Red);

                var line = new Line
                {
                    X1 = startLocal.X,
                    Y1 = startLocal.Y,
                    X2 = endLocal.X,
                    Y2 = endLocal.Y,
                    StrokeThickness = 8,
                    Stroke = strokeBrush,
                    Fill = Brushes.Transparent,
                    ToolTip = $"⚠️ Conflit entre {trainA.Nom} et {trainB.Nom}\nBlock: {block.Nom}",
                    IsHitTestVisible = true
                };

                // Tooltip stable
                ToolTipService.SetInitialShowDelay(line, 0);
                ToolTipService.SetShowDuration(line, 5000);
                ToolTipService.SetPlacement(line, PlacementMode.Mouse);

                // Glow effect
                line.Effect = new DropShadowEffect
                {
                    Color = Colors.Red,
                    BlurRadius = 10,
                    ShadowDepth = 0,
                    Opacity = 0.8
                };

                // Hover interaction
                line.MouseEnter += (s, e) =>
                {
                    line.StrokeThickness = 10;
                    line.Effect = new DropShadowEffect { Color = Colors.Yellow, BlurRadius = 15 };
                };
                line.MouseLeave += (s, e) =>
                {
                    line.StrokeThickness = 8;
                    line.Effect = new DropShadowEffect { Color = Colors.Red, BlurRadius = 10 };
                };

                var canvas = new Canvas();
                canvas.Children.Add(line);

                // Animation rouge ↔ orange
                canvas.Loaded += (s, e) =>
                {
                    var colorAnimation = new ColorAnimation
                    {
                        From = Colors.Red,
                        To = Colors.Orange,
                        Duration = TimeSpan.FromSeconds(0.5),
                        AutoReverse = true,
                        RepeatBehavior = RepeatBehavior.Forever
                    };

                    var storyboard = new Storyboard();
                    storyboard.Children.Add(colorAnimation);
                    Storyboard.SetTarget(colorAnimation, strokeBrush);
                    Storyboard.SetTargetProperty(colorAnimation, new PropertyPath(SolidColorBrush.ColorProperty));
                    storyboard.Begin();
                };

                // Marqueur principal
                var conflictMarker = new GMapMarker(start)
                {
                    Shape = canvas,
                    Offset = new Point(0, 0)
                };
                MapControl.Markers.Add(conflictMarker);

                // 🚨 Icône d’alerte à l’arrivée
                var alertMarker = new GMapMarker(end)
                {
                    Shape = new TextBlock
                    {
                        Text = "🚨",
                        FontSize = 24,
                        Foreground = Brushes.Red,
                        ToolTip = $"Conflit détecté sur {block.Nom}"
                    },
                    Offset = new Point(-12, -12)
                };
                MapControl.Markers.Add(alertMarker);

                // 🏷️ Label avec noms des trains
                var labelMarker = new GMapMarker(start)
                {
                    Shape = new TextBlock
                    {
                        Text = $"{trainA.Nom} ↔ {trainB.Nom}",
                        FontSize = 12,
                        Foreground = Brushes.White,
                        Background = Brushes.DarkRed,
                        Padding = new Thickness(2),
                        ToolTip = $"Conflit sur {block.Nom}"
                    },
                    Offset = new Point(-30, -30)
                };
                MapControl.Markers.Add(labelMarker);
            }

            // Zoom uniquement si un seul conflit
            if (conflits.Count == 1)
            {
                var first = conflits.First().BlockConflit;
                MapControl.Position = new PointLatLng(first.LatitudeDepart, first.LongitudeDepart);
                MapControl.Zoom = 16;
            }
        }
        private void AfficherConflits_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is AdminDashboardViewModel vm)
            {
                var conflits = vm.GetConflits();
                AfficherConflitsSurCarte(conflits);
            }
        }
        private void RafraichirConflits_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is AdminDashboardViewModel vm)
            {
                vm.ConflitsTextuels = new ObservableCollection<string>(
                    vm.GetConflits().Select(c => $"⚠️ {c.TrainA.Nom} et {c.TrainB.Nom} sur {c.BlockConflit.Nom}")
                );
            }
        }
        private void AdminDashboardView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is AdminDashboardViewModel vm)
            {
                var conflits = vm.GetConflits();
                AfficherConflitsSurCarte(conflits); // ✅ méthode locale dans le .xaml.cs
            }
        }
        //public void AfficherConflitsSurCarte(List<(Train TrainA, Train TrainB, Block BlockConflit)> conflits)
        //{
        //    if (conflits == null || conflits.Count == 0) return;

        //    foreach (var (trainA, trainB, block) in conflits)
        //    {
        //        var start = new PointLatLng(block.LatitudeDepart, block.LongitudeDepart);
        //        var end = new PointLatLng(block.LatitudeArrivee, block.LongitudeArrivee);

        //        var startLocal = MapControl.FromLatLngToLocal(start);
        //        var endLocal = MapControl.FromLatLngToLocal(end);

        //        var strokeBrush = new SolidColorBrush(Colors.Red);

        //        var line = new Line
        //        {
        //            X1 = startLocal.X,
        //            Y1 = startLocal.Y,
        //            X2 = endLocal.X,
        //            Y2 = endLocal.Y,
        //            StrokeThickness = 8,
        //            Stroke = strokeBrush,
        //            Fill = Brushes.Transparent,
        //            ToolTip = $"⚠️ Conflit entre {trainA.Nom} et {trainB.Nom}\nBlock: {block.Nom}",
        //            IsHitTestVisible = true
        //        };

        //        // Tooltip stable
        //        ToolTipService.SetInitialShowDelay(line, 0);
        //        ToolTipService.SetShowDuration(line, 5000);
        //        ToolTipService.SetPlacement(line, PlacementMode.Mouse);

        //        // Glow effect
        //        line.Effect = new DropShadowEffect
        //        {
        //            Color = Colors.Red,
        //            BlurRadius = 10,
        //            ShadowDepth = 0,
        //            Opacity = 0.8
        //        };

        //        // Hover interaction
        //        line.MouseEnter += (s, e) =>
        //        {
        //            line.StrokeThickness = 10;
        //            line.Effect = new DropShadowEffect { Color = Colors.Yellow, BlurRadius = 15 };
        //        };
        //        line.MouseLeave += (s, e) =>
        //        {
        //            line.StrokeThickness = 8;
        //            line.Effect = new DropShadowEffect { Color = Colors.Red, BlurRadius = 10 };
        //        };

        //        var canvas = new Canvas();
        //        canvas.Children.Add(line);

        //        // Animation rouge ↔ orange
        //        canvas.Loaded += (s, e) =>
        //        {
        //            var colorAnimation = new ColorAnimation
        //            {
        //                From = Colors.Red,
        //                To = Colors.Orange,
        //                Duration = TimeSpan.FromSeconds(0.5),
        //                AutoReverse = true,
        //                RepeatBehavior = RepeatBehavior.Forever
        //            };

        //            var storyboard = new Storyboard();
        //            storyboard.Children.Add(colorAnimation);
        //            Storyboard.SetTarget(colorAnimation, strokeBrush);
        //            Storyboard.SetTargetProperty(colorAnimation, new PropertyPath(SolidColorBrush.ColorProperty));
        //            storyboard.Begin();
        //        };

        //        // Marqueur principal
        //        var conflictMarker = new GMapMarker(start)
        //        {
        //            Shape = canvas,
        //            Offset = new Point(0, 0)
        //        };
        //        MapControl.Markers.Add(conflictMarker);

        //        // 🚨 Icône d’alerte à l’arrivée
        //        var alertMarker = new GMapMarker(end)
        //        {
        //            Shape = new TextBlock
        //            {
        //                Text = "🚨",
        //                FontSize = 24,
        //                Foreground = Brushes.Red,
        //                ToolTip = $"Conflit détecté sur {block.Nom}"
        //            },
        //            Offset = new Point(-12, -12)
        //        };
        //        MapControl.Markers.Add(alertMarker);

        //        // 🏷️ Label avec noms des trains
        //        var labelMarker = new GMapMarker(start)
        //        {
        //            Shape = new TextBlock
        //            {
        //                Text = $"{trainA.Nom} ↔ {trainB.Nom}",
        //                FontSize = 12,
        //                Foreground = Brushes.White,
        //                Background = Brushes.DarkRed,
        //                Padding = new Thickness(2),
        //                ToolTip = $"Conflit sur {block.Nom}"
        //            },
        //            Offset = new Point(-30, -30)
        //        };
        //        MapControl.Markers.Add(labelMarker);
        //    }

        //    // Zoom uniquement si un seul conflit
        //    if (conflits.Count == 1)
        //    {
        //        var first = conflits.First().BlockConflit;
        //        MapControl.Position = new PointLatLng(first.LatitudeDepart, first.LongitudeDepart);
        //        MapControl.Zoom = 16;
        //    }
        //}
        //private void AfficherConflits_Click(object sender, RoutedEventArgs e)
        //{
        //    if (DataContext is AdminDashboardViewModel vm)
        //    {
        //        var conflits = vm.GetConflits();
        //        AfficherConflitsSurCarte(conflits);
        //    }
        //}
        //private void RafraichirConflits_Click(object sender, RoutedEventArgs e)
        //{
        //    if (DataContext is AdminDashboardViewModel vm)
        //    {
        //        vm.ConflitsTextuels = new ObservableCollection<string>(
        //            vm.GetConflits().Select(c => $"⚠️ {c.TrainA.Nom} et {c.TrainB.Nom} sur {c.BlockConflit.Nom}")
        //        );
        //    }
        //}
        //private void AdminDashboardView_Loaded(object sender, RoutedEventArgs e)
        //{
        //    if (DataContext is AdminDashboardViewModel vm)
        //    {
        //        var conflits = vm.GetConflits();
        //        AfficherConflitsSurCarte(conflits); // ✅ méthode locale dans le .xaml.cs
        //    }
        //}
    }
}


