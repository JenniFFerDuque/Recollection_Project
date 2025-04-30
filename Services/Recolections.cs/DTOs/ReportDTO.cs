using Data.Models;

namespace Services.Recolections.cs.DTOs
{
    public class ReportDTO
    {
        public required User User { get; set; }
        public required List<RecolectionDetail> Recolections { get; set; }
        public decimal TotalWeight { get; set; }
        public required Dictionary<string, int> RecolectionSummary { get; set; } // Tipo -> Cantidad
    }

    public class RecolectionDetail
    {
        public DateTime Date { get; set; }
        public required string MaterialType { get; set; }
        public decimal? Weight { get; set; }
    }
}
