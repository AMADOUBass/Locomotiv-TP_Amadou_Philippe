using Locomotiv.Utils.Services;
using Microsoft.EntityFrameworkCore;
using Locomotiv.Utils;
using System.ComponentModel.DataAnnotations;
using System.IO;
using static Block;

public class ApplicationDbContext : DbContext
{

    public DbSet<User> Users { get; set; }
    public DbSet<Station> Station { get; set; }
    public DbSet<Train> Train { get; set; }
    public DbSet<Itineraire> Itineraire { get; set; }
    public DbSet<Signal> Signau { get; set; }
    public DbSet<Voie> Voie { get; set; }
    public DbSet<Etape> Etape { get; set; }

    public DbSet<Block> Blocks { get; set; }





    protected override void OnConfiguring(
       DbContextOptionsBuilder optionsBuilder)
    {
        // Définir le chemin absolu pour la base de données dans le répertoire AppData
        var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Locomotiv", "Locomotiv.db");
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath));
        var connectionString = $"Data Source={dbPath}";

        // Configurer le DbContext pour utiliser la chaîne de connexion
        optionsBuilder.UseSqlite(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }


    public void SeedData(bool force = false)
    {
        if (force || !Users.Any())
        {
            Console.WriteLine("🔁 Exécution du seed...");

            var (adminHash, adminSalt) = PassWordHelper.HashPassword("adminpass");
            var (employeHash, employeSalt) = PassWordHelper.HashPassword("employepass");

            // Station
            var station = new Station
            {
                Nom = "Gare de Québec",
                Localisation = "Vieux-Québec",
                Latitude = 46.8139,
                Longitude = -71.2082,
                CapaciteMaxTrains = 5
            };
            Station.Add(station);
            // Autres stations
            var gareCn = new Station
            {
                Nom = "Gare CN",
                Localisation = "Saint-Roch",
                Latitude = 46.8142,
                Longitude = -71.2105,
                CapaciteMaxTrains = 4
            };
            Station.Add(gareCn);

            var gareGatineau = new Station
            {
                Nom = "Gare Québec-Gatineau",
                Localisation = "Vanier",
                Latitude = 46.8300,
                Longitude = -71.2400,
                CapaciteMaxTrains = 3
            };
            Station.Add(gareGatineau);

            // Points d’intérêt logistiques et connexions
            var pointsInteret = new List<Station>
{
                new() { Nom = "Port de Québec", Localisation = "Vieux-Port", Latitude = 46.8219, Longitude = -71.2083, CapaciteMaxTrains = 2 },
                new() { Nom = "Baie de Beauport", Localisation = "Zone maritime", Latitude = 46.8458, Longitude = -71.1986, CapaciteMaxTrains = 2 },
                new() { Nom = "Centre de distribution", Localisation = "Industriel", Latitude = 46.8250, Longitude = -71.2450, CapaciteMaxTrains = 2 },
                new() { Nom = "Vers Gatineau", Localisation = "Connexion interrégionale", Latitude = 46.8400, Longitude = -71.2600, CapaciteMaxTrains = 0 },
                new() { Nom = "Vers Charlevoix", Localisation = "Connexion touristique", Latitude = 46.8500, Longitude = -71.1800, CapaciteMaxTrains = 0 },
                new() { Nom = "Vers la rive-sud", Localisation = "Connexion sud", Latitude = 46.8100, Longitude = -71.2300, CapaciteMaxTrains = 0 },
                new() { Nom = "Vers le nord", Localisation = "Connexion nord", Latitude = 46.8600, Longitude = -71.2200, CapaciteMaxTrains = 0 }
};
            Station.AddRange(pointsInteret);
            SaveChanges();
            SaveChanges();

            // Utilisateurs
            Users.AddRange(new List<User>
        {
            new ()
            {
                Prenom = "Admin",
                Nom = "Admin",
                Username = "admin",
                PasswordHash = adminHash,
                PasswordSalt = adminSalt,
                Role = User.UserRole.Admin
            },
            new ()
            {
                Prenom = "Employe",
                Nom = "Employe",
                Username = "employe",
                PasswordHash = employeHash,
                PasswordSalt = employeSalt,
                Role = User.UserRole.Employe,
                StationId = station.Id
            }
        });
            SaveChanges();

            // Trains
            var trainA = new Train
            {
                Nom = "Train A",
                Etat = EtatTrain.EnTransit,
                Capacite = 100,
                StationId = station.Id
            };
            Train.Add(trainA);
            SaveChanges();

            var trainB = new Train
            {
                Nom = "Train B",
                Etat = EtatTrain.EnTransit,
                Capacite = 80,
                StationId = station.Id
            };
            Train.Add(trainB);
            SaveChanges();

            // Blocks
            var blockConflit = new Block
            {
                Nom = "Gare du Palais → Gare CN",
                LatitudeDepart = 46.817400,
                LongitudeDepart = -71.213900,
                LatitudeArrivee = 46.814186,
                LongitudeArrivee = -71.210507,
                Signal = SignalType.Vert,
                EstOccupe = true,
                TrainId = trainA.Id
            };

            var blocksToSeed = new List<Block>
        {
            blockConflit,
            new ()
            {
                Nom = "Gare CN → Vers Gatineau",
                LatitudeDepart = 46.814186,
                LongitudeDepart = -71.210507,
                LatitudeArrivee = 46.819000,
                LongitudeArrivee = -71.250000,
                Signal = SignalType.Jaune,
                EstOccupe = false
            },
            new ()
            {
                Nom = "Gare du Palais → Baie de Beauport",
                LatitudeDepart = 46.817400,
                LongitudeDepart = -71.213900,
                LatitudeArrivee = 46.845833,
                LongitudeArrivee = -71.198611,
                Signal = SignalType.Rouge,
                EstOccupe = false
            },
            new ()
            {
                Nom = "Gare CN → Port de Québec",
                LatitudeDepart = 46.814186,
                LongitudeDepart = -71.210507,
                LatitudeArrivee = 46.821957,
                LongitudeArrivee = -71.208304,
                Signal = SignalType.Vert,
                EstOccupe = false
            },
            new ()
            {
                Nom = "Gare Québec-Gatineau → Vers le nord",
                LatitudeDepart = 46.817500,
                LongitudeDepart = -71.213700,
                LatitudeArrivee = 46.830000,
                LongitudeArrivee = -71.220000,
                Signal = SignalType.Vert,
                EstOccupe = false
            }
        };
            Blocks.AddRange(blocksToSeed);
            SaveChanges();

            // Itinéraire pour Train A
            var itineraireA = new Itineraire
            {
                TrainId = trainA.Id,
                Nom = "Québec → Gatineau",
                DateDepart = DateTime.Today.AddHours(8),
                DateArrivee = DateTime.Today.AddHours(12)
            };
            Itineraire.Add(itineraireA);
            SaveChanges();

            // Étapes pour Train A
            var etapesA = new List<Etape>
        {
            new()
            {
                ItineraireId = itineraireA.Id,
                TrainId = trainA.Id,
                Ordre = 1,
                BlockId = blockConflit.Id,
                Lieu = "Gare du Palais",
                HeureArrivee = DateTime.Today.AddHours(8),
                HeureDepart = DateTime.Today.AddHours(8).AddMinutes(5)
            },
            new()
            {
                ItineraireId = itineraireA.Id,
                TrainId = trainA.Id,
                Ordre = 2,
                BlockId = blocksToSeed[1].Id,
                Lieu = "Gare CN",
                HeureArrivee = DateTime.Today.AddHours(10),
                HeureDepart = DateTime.Today.AddHours(10).AddMinutes(5)
            },
            new()
            {
                ItineraireId = itineraireA.Id,
                TrainId = trainA.Id,
                Ordre = 3,
                BlockId = blocksToSeed[4].Id,
                Lieu = "Vers le nord",
                HeureArrivee = DateTime.Today.AddHours(12),
                HeureDepart = DateTime.Today.AddHours(12).AddMinutes(5)
            }
        };
            Etape.AddRange(etapesA);
            SaveChanges();

            // Étape conflictuelle pour Train B sur le même block
            var etapeConflit = new Etape
            {
                ItineraireId = itineraireA.Id,
                TrainId = trainB.Id,
                Ordre = 4,
                BlockId = blockConflit.Id,
                Lieu = "Gare du Palais",
                HeureArrivee = DateTime.Today.AddHours(8),
                HeureDepart = DateTime.Today.AddHours(8).AddMinutes(5)
            };
            Etape.Add(etapeConflit);
            SaveChanges();

            // ✅ Affecter le block aux trains pour que GetConflits() fonctionne
            trainA.BlockId = blockConflit.Id;
            trainB.BlockId = blockConflit.Id;
            Train.Update(trainA);
            Train.Update(trainB);
            SaveChanges();

            Console.WriteLine("✅ Données initiales insérées avec conflit simulé.");
        }
    }
}
