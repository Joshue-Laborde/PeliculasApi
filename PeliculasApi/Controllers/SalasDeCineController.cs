using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PeliculasApi.DTOs;
using PeliculasApi.Entidades;

namespace PeliculasApi.Controllers
{
    [ApiController]
    [Route("api/SalasDeCine")]
    public class SalasDeCineController : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public SalasDeCineController(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            this.context = context;
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
