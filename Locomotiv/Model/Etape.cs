using System.ComponentModel.DataAnnotations;

public class Etape
{
    [Key]
    public int Id { get; set; }

    public string Lieu { get; set; } // Nom de la station ou point d’intérêt

    public DateTime HeureArrivee { get; set; }

    public DateTime HeureDepart { get; set; }

    public int Ordre { get; set; } // Position dans l’itinéraire

    public int ItineraireId { get; set; } // Clé étrangère vers l’itinéraire
    public Itineraire Itineraire { get; set; }
}

