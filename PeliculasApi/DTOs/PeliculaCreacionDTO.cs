using PeliculasApi.Validation;
using System.ComponentModel.DataAnnotations;

namespace PeliculasApi.DTOs
{
    public class PeliculaCreacionDTO : PeliculaPatchDTO
    {

        [PesoArchivoValidacion(pesoMaximoEnMegaBytes: 4)]
        [TipoArchivoValidacion(grupoTipoArchivo: GrupoTipoArchivo.Imagen)]
        public IFormFile Poster { get; set; }

        public List<int> GenerosIds { get; set; }
    }
}
