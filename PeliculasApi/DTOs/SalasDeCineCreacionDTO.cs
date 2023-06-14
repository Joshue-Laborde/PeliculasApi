using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace PeliculasApi.DTOs
{
    public class SalasDeCineCreacionDTO
    {
        [Required]
        [StringLength(120)]
        public string Nombre { get; set; }
    }
}
