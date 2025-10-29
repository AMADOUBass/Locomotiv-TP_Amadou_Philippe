using Locomotiv.Utils.Services;
using Microsoft.EntityFrameworkCore;
using Seismoscope.Utils;
using System.IO;

public class ApplicationDbContext : DbContext
{

    public DbSet<User> Users { get; set; }
    public DbSet<Station> Stations { get; set; }



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


    public void SeedData()
    {
        if (!Users.Any())
        {
            var (adminHash, adminSalt) = PassWordHelper.HashPassword("adminpass");
            var (employeHash, employeSalt) = PassWordHelper.HashPassword("employepass");

           
            var station = new Station
            {
                Nom = "Gare de Québec",
                Localisation = "Vieux-Québec",
                Latitude = 46.8139,
                Longitude = -71.2082,
                CapaciteMaxTrains = 5
            };
            Stations.Add(station); 

            Users.AddRange(
                new User
                {
                    Prenom = "Admin",
                    Nom = "Admin",
                    Username = "admin",
                    PasswordHash = adminHash,
                    PasswordSalt = adminSalt,
                    Role = User.UserRole.Admin
                },
                new User
                {
                    Prenom = "Employe",
                    Nom = "employe",
                    Username = "employe",
                    PasswordHash = employeHash,
                    PasswordSalt = employeSalt,
                    Role = User.UserRole.Employe,
                    Station = station 
                }
            );

            SaveChanges();
        }
    }

}
