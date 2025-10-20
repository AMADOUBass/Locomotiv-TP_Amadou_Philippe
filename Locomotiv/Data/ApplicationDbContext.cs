using Microsoft.EntityFrameworkCore;
using Locomotiv.Utils.Services;
using System.IO;

public class ApplicationDbContext : DbContext
{
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

    public DbSet<User> Users { get; set; }

    public void SeedData()
    {
        if (!Users.Any())
        {
            Users.AddRange(
                new User { Prenom = "John", Nom = "Doe", Username = "johndoe", Password = "password123" },
                new User { Prenom = "Jane", Nom = "Doe", Username = "janedoe", Password = "password123" }
            );
            SaveChanges();
        }
    }
}
