using ApiBRD.Helpers;
using ApiBRD.Models.DTOs;
using ApiBRD.Models.Entities;
using ApiBRD.Repositories;
using ApiBRD.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ApiBRD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly Repository<Producto> repositoryProductos;
        private readonly Repository<Categoria> repositoryCategorias;
        private readonly IMapper mapper;
        private readonly IWebHostEnvironment webHostEnvironment;

        public LabsystePwaBrdContext context { get; }
        public IHubContext<CategoriaHub> HubContext { get; }


        public CategoriasController(Repository<Producto> repositoryProductos,Repository<Categoria> repository, LabsystePwaBrdContext context, IHubContext<CategoriaHub> hubContext, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            this.repositoryProductos = repositoryProductos;
            this.repositoryCategorias = repository;
            this.context = context;
            HubContext = hubContext;
            this.mapper = mapper;
            this.webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var categorias = repositoryCategorias.Context.Categoria.Include(x => x.Producto).ToList().Select(x => mapper.Map<CategoriaIncludeDTO>(x));
            return Ok(categorias);
        }


        [HttpPost]
        public async Task<IActionResult> AgregarCategoria(CategoriaImagenDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
            {
                return BadRequest("Ingrese el nombre de la categoria.");
            }

            Categoria d = new()
            {
                Nombre = dto.Nombre
            };

            repositoryCategorias.Insert(d);

            try
            {
                string path = Path.Combine(webHostEnvironment.WebRootPath, "categorias", d.Id.ToString());
                ImagenConverter.ConvertBase64ToImage(dto.ImagenBase64, path);
            }
            catch (Exception ex)
            {
                repositoryCategorias.Delete(d);
                return StatusCode(500, "Error al guardar la imagen: " + ex.Message);
            }

            await HubContext.Clients.All.SendAsync("NuevaCategoria", d);
            return Ok("La categoria fue agregada con exito.");
        }

        [HttpPut]
        public async Task<IActionResult> EditarCategoria(CategoriaImagenDTO dto)
        {
            var categoria = context.Categoria.Find(dto.Id);

            if (categoria == null)
            {
                return NotFound("La categoria que intenta editar no fue encontrada.");
            }
            categoria.Nombre = dto.Nombre;

            if (dto.ImagenBase64 != null)
            {
                try
                {
                    string path = Path.Combine(webHostEnvironment.WebRootPath, "categorias", dto.Id.ToString());
                    ImagenConverter.ConvertBase64ToImage(dto.ImagenBase64, path);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Error al subir nueva imagen:" + ex.Message);
                }
            }


            context.Update(categoria);
            int total = context.SaveChanges();

            if (total > 0)
            {
                await HubContext.Clients.All.SendAsync("CategoriaEditada", new
                {
                    categoria.Id,
                    categoria.Nombre,
                });
            }

            return Ok("La categoria fue editada con exito");
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarCategoria(int id)
        {
            var categoriaExistente = context.Categoria.Find(id);
            if (categoriaExistente == null)
            {
                return NotFound("No se pudo encontrar la categoria que desea eliminar.");
            }

            var data = repositoryProductos.GetAll().Where(x => x.IdCategoria == id).ToList();
            foreach (var item in data)
            {
                repositoryProductos.Delete(item);
            }

            context.Remove(categoriaExistente);
            context.SaveChanges();

            await HubContext.Clients.All.SendAsync("CategoriaEliminada", categoriaExistente.Id);

            return Ok("La categoria fue eliminada con exito.");
        }
    }
}
