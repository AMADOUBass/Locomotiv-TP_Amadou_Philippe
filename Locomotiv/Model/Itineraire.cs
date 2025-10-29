using System.ComponentModel.DataAnnotations;


public class Itineraire
{
    [Key]
    public int Id { get; set; }

    public ICollection<Etape> Etapes { get; set; } = new List<Etape>();

    public int TrainId { get; set; } // Clé étrangère vers le train
    public Train Train { get; set; } // Navigation vers le train
}