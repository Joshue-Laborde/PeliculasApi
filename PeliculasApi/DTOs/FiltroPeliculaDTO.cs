namespace PeliculasApi.DTOs
{
    public class FiltroPeliculaDTO
    {
        public int pagina { get; set; } = 1;
        public int CantidadRegistroPorPagina { get; set; } = 10;
        public PaginacionDTO Paginacion
        {
            get { return new PaginacionDTO() { Pagina = pagina, CantidadRegistrosPorPagina = CantidadRegistroPorPagina, }; }
        }
        public string Titulo { get; set; }
        public int GeneroId { get; set; }
        public bool EnCines { get; set; }
        public bool ProximosEstrenos { get; set; }
    }
}
