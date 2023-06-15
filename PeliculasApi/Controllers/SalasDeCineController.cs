using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using PeliculasApi.DTOs;
using PeliculasApi.Entidades;

namespace PeliculasApi.Controllers
{
    [ApiController]
    [Route("api/SalasDeCine")]
    public class SalasDeCineController : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly GeometryFactory geometryFactory;
        private readonly IMapper mapper;

        public SalasDeCineController(ApplicationDbContext context, GeometryFactory geometryFactory ,IMapper mapper) : base(context, mapper)
        {
            this.context = context;
            this.geometryFactory = geometryFactory;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<SalasDeCineDTO>>> Get()
        {
            return await Get<SalaDeCine, SalasDeCineDTO>();
        }

        [HttpGet("{id:int}",Name = "obtenerSalaDeCine")]
        public async Task<ActionResult<SalasDeCineDTO>> Get(int id)
        {
            return await Get<SalaDeCine, SalasDeCineDTO>(id);
        }

        [HttpGet("Cercanos")]
        public async Task<ActionResult<List<SalaDeCineCercanoDTO>>> Cercanos([FromQuery] SalaDeCineCercanoFiltroDTO filtro)
        {
            var ubicacionUsuario = geometryFactory.CreatePoint(new Coordinate(filtro.Longitud, filtro.Latitud));

            var salasDeCine = await context.SalaDeCine
                                            .OrderBy(x => x.Ubicacion.Distance(ubicacionUsuario))
                                            .Where(x => x.Ubicacion.IsWithinDistance(ubicacionUsuario, filtro.DistanciaEnKms * 1000))
                                            .Select(x => new SalaDeCineCercanoDTO
                                            {
                                                Id = x.Id,
                                                Nombre = x.Nombre,
                                                Latitud = x.Ubicacion.Y,
                                                Longitud = x.Ubicacion.X,
                                                DistanciaEnMetros = Math.Round(x.Ubicacion.Distance(ubicacionUsuario))
                                            }).ToListAsync();
            return salasDeCine;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] SalasDeCineCreacionDTO salasDeCineCreacionDTO)
        {
            return await Post<SalasDeCineCreacionDTO, SalaDeCine, SalasDeCineDTO>(salasDeCineCreacionDTO, "obtenerSalaDeCine");
        }

        [HttpPut]
        public async Task<ActionResult> Put(int id, [FromBody] SalasDeCineCreacionDTO salasDeCineCreacionDTO)
        {
            return await Put<SalasDeCineCreacionDTO, SalaDeCine>(id, salasDeCineCreacionDTO);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<SalaDeCine>(id);
        }
    }
}
