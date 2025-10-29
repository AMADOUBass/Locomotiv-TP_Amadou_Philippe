//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Seismoscope.Data.DtabaseSeeder
//{
//   public class DatabaseSeeder
//{
//    private readonly ApplicationDbContext _context;

//    public DatabaseSeeder(ApplicationDbContext context)
//    {
//        _context = context;
//    }

//    public void Seed()
//    {
//        SeedUsers();
//        SeedStationsAndTrains();
//        _context.SaveChanges();
//    }

//    private void SeedUsers()
//    {
//        if (_context.Users.Any()) return;

//        _context.Users.AddRange(
//            new User { Prenom = "Alice", Nom = "Dubois", Username = "admin", Password = PasswordHelper.HashPassword("admin123"), Role = UserRole.Admin },
//            new User { Prenom = "Marc", Nom = "Lavoie", Username = "employe1", Password = PasswordHelper.HashPassword("pass123"), Role = UserRole.User }
//        );
//    }

//    private void SeedStationsAndTrains()
//    {
//        if (_context.Stations.Any()) return;

//        var garePalais = new Station
//        {
//            Nom = "Gare du Palais",
//            Localisation = "Vieux-Québec",
//            CapaciteMaxTrains = 3,
//            Voies = new List<Voie>
//            {
//                new Voie { Nom = "Quai A" },
//                new Voie { Nom = "Quai B" }
//            },
//            Signaux = new List<Signal>
//            {
//                new Signal { Type = TypeSignal.Arret, EstActif = true },
//                new Signal { Type = TypeSignal.Passage, EstActif = true }
//            }
//        };

//        var steFoy = new Station
//        {
//            Nom = "Sainte-Foy",
//            Localisation = "Rive Ouest",
//            CapaciteMaxTrains = 2,
//            Voies = new List<Voie>
//            {
//                new Voie { Nom = "Quai C" }
//            },
//            Signaux = new List<Signal>
//            {
//                new Signal { Type = TypeSignal.Danger, EstActif = false }
//            }
//        };

//        var train1 = new Train
//        {
//            Nom = "Express Québec",
//            Etat = EtatTrain.EnGare,
//            Capacite = 100,
//            Station = garePalais
//        };

//        var train2 = new Train
//        {
//            Nom = "Navette Sainte-Foy",
//            Etat = EtatTrain.EnTransit,
//            Capacite = 80,
//            Station = steFoy
//        };

//        var itineraire = new Itineraire
//        {
//            Train = train1,
//            Etapes = new List<Etape>
//            {
//                new Etape { Lieu = "Gare du Palais", HeureArrivee = DateTime.Now, HeureDepart = DateTime.Now.AddMinutes(10), Ordre = 1 },
//                new Etape { Lieu = "Sainte-Foy", HeureArrivee = DateTime.Now.AddMinutes(30), HeureDepart = DateTime.Now.AddMinutes(35), Ordre = 2 }
//            }
//        };

//        _context.Stations.AddRange(garePalais, steFoy);
//        _context.Trains.AddRange(train1, train2);
//        _context.Itineraires.Add(itineraire);
//    }
//}
//}
