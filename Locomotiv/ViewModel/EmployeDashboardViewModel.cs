using Locomotiv.Model.Interfaces;
using Locomotiv.Utils;
using Locomotiv.Utils.Services.Interfaces;

public class EmployeDashboardViewModel : BaseViewModel
{
    private readonly IStationDAL _stationDAL;
    private readonly IUserSessionService _userSessionService;

    private Station _stationAssignee;
    public Station StationAssignee
    {
        get => _stationAssignee;
        set => SetProperty(ref _stationAssignee, value);
    }

    public EmployeDashboardViewModel(IStationDAL stationDAL, IUserSessionService userSessionService)
    {
        _stationDAL = stationDAL;
        _userSessionService = userSessionService;

        _userSessionService.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(_userSessionService.ConnectedUser))
            {
                ChargerStationAssignee();
            }
        };

        ChargerStationAssignee();
    }

    private void ChargerStationAssignee()
    {
        var user = _userSessionService.ConnectedUser;

        if (user?.StationId == null)
        {
            StationAssignee = new Station { Nom = "Aucune station assignée" };
            return;
        }

        StationAssignee = _stationDAL.GetStationById(user.StationId.Value)
                         ?? new Station { Nom = "Station introuvable" };
    }
}