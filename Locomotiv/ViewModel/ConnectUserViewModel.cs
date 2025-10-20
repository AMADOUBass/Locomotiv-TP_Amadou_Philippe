using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Locomotiv.Model;
using Locomotiv.Model.DAL;
using Locomotiv.Model.Interfaces;
using Locomotiv.Utils;
using Locomotiv.Utils.Commands;
using Locomotiv.Utils.Services.Interfaces;
using Locomotiv.Utils.Services;

namespace Locomotiv.ViewModel
{
    class ConnectUserViewModel : BaseViewModel
    {
        private readonly IUserDAL _userDAL;
        private INavigationService _navigationService;
        private IUserSessionService _userSessionService;

        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged(nameof(Username));
                    ValidateProperty(nameof(Username), value);
                }
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged(nameof(Password));
                    ValidateProperty(nameof(Password), value);
                }
            }
        }

        // Constructeur pour initialiser les services et la commande de connexion
        public ConnectUserViewModel(IUserDAL userDAL, INavigationService navigationService, IUserSessionService userSessionService)
        {
            _userDAL = userDAL;
            _navigationService = navigationService;
            _userSessionService = userSessionService;
            ConnectCommand = new RelayCommand(Connect, CanConnect);
        }

        public ICommand ConnectCommand { get; set; }

        // Méthode pour gérer la connexion de l'utilisateur
        private void Connect()
        {
            User? user = _userDAL.FindByUsernameAndPassword(Username, Password);
            if (user != null)
            {
                _userSessionService.ConnectedUser = user;
                _navigationService.NavigateTo<HomeViewModel>();
            }
            else
            {
                AddError(nameof(Password), "Utilisateur ou mot de passe invalide.");
                OnPropertyChanged(nameof(ErrorMessages));
            }
        }

        // Vérifie si la commande de connexion peut être exécutée
        private bool CanConnect()
        {
            bool allRequiredFieldsAreEntered = Username.NotEmpty() && Password.NotEmpty();
            return !HasErrors && allRequiredFieldsAreEntered;
        }

        // Valide les propriétés Username et Password
        private void ValidateProperty(string propertyName, string value)
        {
            ClearErrors(propertyName);
            switch (propertyName)
            {
                case nameof(Username):
                    if (value.Empty())
                    {
                        AddError(propertyName, "Le nom d'utilisateur est requis.");
                    }
                    else if (value.Length < 2)
                    {
                        AddError(propertyName, "Le nom d'utilisateur doit contenir au moins 2 caractères.");
                    }
                    break;
                case nameof(Password):
                    if (value.Empty())
                    {
                        AddError(propertyName, "Le mot de passe est requis.");
                    }
                    break;
            }
            OnPropertyChanged(nameof(ErrorMessages));
        }
    }
}
