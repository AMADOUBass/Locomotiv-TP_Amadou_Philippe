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
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;


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
            InitMap();
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

            AddMarker(QuebecCenter, Brushes.Gold, Brushes.Black);

        }
        private void AddMarker(PointLatLng position, Brush fillColor, Brush strokeColor)
        {
            GMapMarker marker = new(position)
            {
                Shape = new Ellipse
                {
                    Width = 12,
                    Height = 12,
                    Stroke = strokeColor,
                    StrokeThickness = 2,
                    Fill = fillColor,
                    ToolTip = "Centre de Québec"
                },
                Offset = new Point(-6, -6)
            };
            MapControl.Markers.Add(marker);
        }

        public void LoadStationMarkers(IEnumerable<Station> stations)
        {
            MapControl.Markers.Clear();
            foreach (var station in stations)
            {
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
                        ToolTip = station.Nom
                    },
                    Offset = new Point(-6, -6)
                    };
                marker.Shape.MouseLeftButtonUp += (s, e) => OnStationSelected(station);
                MapControl.Markers.Add(marker);

            }
        }
        private void OnStationSelected(Station selectedStation)
        {
            SelectedStation = selectedStation;
            
        }


    }
}
