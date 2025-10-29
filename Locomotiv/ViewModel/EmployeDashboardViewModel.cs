using Locomotiv.Utils;
using Locomotiv.Utils.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locomotiv.ViewModel
{
    public class EmployeDashboardViewModel : BaseViewModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserSessionService _session;

        public Station? AssignedStation { get; private set; }

        //public ObservableCollection<Train> Trains => new(AssignedStation?.Trains ?? new());
        //public ObservableCollection<Voie> Voies => new(AssignedStation?.Voies ?? new());
        //public ObservableCollection<Signal> Signaux => new(AssignedStation?.Signaux ?? new());

        public EmployeDashboardViewModel(ApplicationDbContext context, IUserSessionService session)
        {
            _context = context;
            _session = session;
            //LoadAssignedStation();
        }

        //private void LoadAssignedStation()
        //{
        //    var user = _session.ConnectedUser;
        //    if (user?.StationId != null)
        //    {
        //        AssignedStation = _context.Stations
        //            .Include(s => s.Trains)
        //            .Include(s => s.Voies)
        //            .Include(s => s.Signaux)
        //            .FirstOrDefault(s => s.Id == user.StationId);
        //    }
        //}
    }
}
