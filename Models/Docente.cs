namespace MiWebAPI.Models
{
    public class Docente
    {
        public int IdDocente { get; set; }
        public string? NombreCompleto { get; set; }
        public string? Correo { get; set; }
        public string? Tipo { get; set; }
        public decimal? HorasContratadas { get; set; }
        public string? Disponibilidad { get; set; }
        public string? Observaciones { get; set; }
    }
}