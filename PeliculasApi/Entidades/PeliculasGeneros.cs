namespace PeliculasApi.Entidades
{
    public class PeliculasGeneros
    {
        public int GeneroId { get; set; }
        public int PeliculasId { get; set; }
        public Genero Genero { get; set; }
        public Pelicula pelicula { get; set; }
        
    }
}
