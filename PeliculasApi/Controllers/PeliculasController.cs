﻿using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasApi.DTOs;
using PeliculasApi.Entidades;
using PeliculasApi.Helpers;
using PeliculasApi.Servicios;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace PeliculasApi.Controllers
{
    [ApiController]
    [Route("api/peliculas")]
    public class PeliculasController : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly ILogger<PeliculasController> logger;
        private readonly string contenedor = "peliculas";

        public PeliculasController(ApplicationDbContext context, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos, ILogger<PeliculasController> logger) : base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PeliculasIndexDTO>> Get()
        {
            var top = 5;
            var hoy = DateTime.Today;

            var proximosEstrenos = await context.Peliculas
                                            .Where(x => x.FechaEstreno > hoy)
                                            .OrderBy(x => x.FechaEstreno)
                                            .Take(top)
                                            .ToListAsync();

            var enCines = await context.Peliculas
                                            .Where(x => x.EnCines)
                                            .Take(top)
                                            .ToListAsync();

            var result = new PeliculasIndexDTO();
            result.FuturosEstrenos = mapper.Map<List<PeliculaDTO>>(proximosEstrenos);
            result.EnCines = mapper.Map<List<PeliculaDTO>>(enCines);
            return result;
        }

        [HttpGet("filtro")]
        public async Task<ActionResult<List<PeliculaDTO>>> Filtrar([FromQuery] FiltroPeliculaDTO filtroPeliculaDTO)
        {
            var peliculasQueryable = context .Peliculas.AsQueryable();

            if (!string.IsNullOrEmpty(filtroPeliculaDTO.Titulo))
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.Titulo.Contains(filtroPeliculaDTO.Titulo));
            }

            if (filtroPeliculaDTO.EnCines)
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.EnCines);
            }

            if (filtroPeliculaDTO.ProximosEstrenos)
            {
                var hoy = DateTime.Today;
                peliculasQueryable = peliculasQueryable.Where(x => x.FechaEstreno > hoy);
            }

            if(filtroPeliculaDTO.GeneroId != 0)
            {
                peliculasQueryable = peliculasQueryable
                                        .Where(x => x.PeliculasGeneros.Select(y => y.GeneroId)
                                        .Contains(filtroPeliculaDTO.GeneroId));
            }

            if (!string.IsNullOrEmpty(filtroPeliculaDTO.CampoOrdenar))
            {
                var tipoOrden = filtroPeliculaDTO.OrdenAscendente ? "ascending" : "descending";
                try
                {
                    peliculasQueryable = peliculasQueryable.OrderBy($"{filtroPeliculaDTO.CampoOrdenar} {tipoOrden}");
                }
                catch(Exception ex)
                {
                    logger.LogError(ex.Message, ex);
                }
            }

            await HttpContext.InsertarParametrosPaginacion(peliculasQueryable, filtroPeliculaDTO.CantidadRegistroPorPagina);

            var peliculas = await peliculasQueryable.Paginar(filtroPeliculaDTO.Paginacion).ToListAsync();

            return mapper.Map<List<PeliculaDTO>>(peliculas);
        }


        [HttpGet("{id}", Name = "obtenerPelicula")]
        public async Task<ActionResult<PeliculasDetallesDTO>> GetId(int id)
        {
            var pelicula = await context.Peliculas.Include(x=> x.PeliculasActores).ThenInclude(x=> x.Actor)
                                                    .Include(x=> x.PeliculasGeneros).ThenInclude(x=>x.Genero)
                                                    .FirstOrDefaultAsync(x => x.Id == id);
            
            if (pelicula == null)
                return NotFound();

            pelicula.PeliculasActores = pelicula.PeliculasActores.OrderBy(x => x.Orden).ToList();

            return mapper.Map<PeliculasDetallesDTO>(pelicula);
        }

        [HttpGet, Route("EnCine")]
        public async Task<ActionResult<List<PeliculaDTO>>> PeliculasEnCine()
        {
            var peliculas = await context.Peliculas.Where(x => x.EnCines).ToListAsync();

            if(peliculas == null)
                return NotFound();

            return mapper.Map<List<PeliculaDTO>>(peliculas);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] PeliculaCreacionDTO peliculaCreacionDTO)
        {
            var pelicula = mapper.Map<Pelicula>(peliculaCreacionDTO);

            if(peliculaCreacionDTO.Poster != null)
            {
                using (var memorystream = new MemoryStream())
                {
                    await peliculaCreacionDTO.Poster.CopyToAsync(memorystream);
                    var contenido = memorystream.ToArray();
                    var extension = Path.GetExtension(peliculaCreacionDTO.Poster.FileName);
                    pelicula.Poster = await almacenadorArchivos.GuardarArchivo(contenido, extension, contenedor, peliculaCreacionDTO.Poster.ContentType);
                }
            }
            AsignarOrdernActores(pelicula);
            context.Add(pelicula);
            await context.SaveChangesAsync();
            var result = mapper.Map<PeliculaDTO>(pelicula);
            return new CreatedAtRouteResult("obtenerPelicula", new { id = pelicula.Id }, result);

        }

        private void AsignarOrdernActores(Pelicula pelicula)
        {
            if(pelicula.PeliculasActores != null)
            {
                for(int i = 0; i < pelicula.PeliculasActores.Count; i++)
                {
                    pelicula.PeliculasActores[i].Orden = i;
                }
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromForm] PeliculaCreacionDTO peliculaCreacionDTO)
        {
            var peliculaDB = await context.Peliculas
                                                    .Include(x=> x.PeliculasActores)
                                                    .Include(x=> x.PeliculasGeneros)
                                                    .FirstOrDefaultAsync(x => x.Id == id);
            if (peliculaDB == null)
                return NotFound();

            peliculaDB = mapper.Map(peliculaCreacionDTO, peliculaDB);

            if (peliculaCreacionDTO.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await peliculaCreacionDTO.Poster.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(peliculaCreacionDTO.Poster.FileName);
                    peliculaDB.Poster = await almacenadorArchivos.EditarArchivo(contenido, extension, contenedor, peliculaDB.Poster, peliculaCreacionDTO.Poster.ContentType);

                }
            }
            AsignarOrdernActores(peliculaDB);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<PeliculaPatchDTO> patchDocument)
        {
            /*if (patchDocument is null)
                return BadRequest();

            var pelicula = await context.Peliculas.FirstOrDefaultAsync(x => x.Id == id);

            if (pelicula == null)
                return NotFound();

            var result = mapper.Map<PeliculaPatchDTO>(pelicula);

            patchDocument.ApplyTo(result, ModelState);

            var esValido = TryValidateModel(result);
            if (!esValido) return BadRequest(ModelState);

            mapper.Map(result, pelicula);

            await context.SaveChangesAsync();

            return NoContent();*/

            return await Patch<Pelicula, PeliculaPatchDTO>(id, patchDocument);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            /*var existe = await context.Peliculas.AnyAsync(x => x.Id == id);
            if (!existe) { return NotFound(); }

            context.Remove(new Pelicula() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();*/

            return await Delete<Pelicula>(id);
        }
    }
}
