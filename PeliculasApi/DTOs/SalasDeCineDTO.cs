using System.Globalization;

namespace PeliculasApi.DTOs
{
    public class SalasDeCineDTO
    {
        public int id { get; set; }
        public string Nombre { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set;}
    }
}
