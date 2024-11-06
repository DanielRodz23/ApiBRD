using ApiBRD.Helpers;
using ApiBRD.Hubs;
using ApiBRD.Models.DTOs;
using ApiBRD.Models.Entities;
using ApiBRD.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ApiBRD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriasController(Repository<Producto> repositoryProductos, Repository<Categoria> repository, LabsystePwaBrdContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment) : ControllerBase
    {
        private readonly Repository<Producto> repositoryProductos = repositoryProductos;
        private readonly Repository<Categoria> repositoryCategorias = repository;
        private readonly IMapper mapper = mapper;
        private readonly IWebHostEnvironment webHostEnvironment = webHostEnvironment;

        public LabsystePwaBrdContext Context { get; } = context;

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            var categorias = repositoryCategorias.Context.Categoria.Include(x => x.Producto).ToList().Select(mapper.Map<CategoriaIncludeDTO>);
            return Ok(categorias);
        }
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var cat = repositoryCategorias.GetById(id);
            if (cat == null) return NotFound();
            return Ok(mapper.Map<CategoriaDTO>(cat));
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
            LastUpdateManager.UpdateCategorias();
            //await HubContext.Clients.All.SendAsync("NuevaCategoria", mapper.Map<CategoriaDTO>(d));
            return Ok("La categoria fue agregada con exito.");
        }

        [HttpPut]
        public async Task<IActionResult> EditarCategoria(CategoriaImagenDTO dto)
        {
            var categoria = Context.Categoria.Find(dto.Id);

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


            Context.Update(categoria);
            Context.SaveChanges();

            //if (total > 0)
            //{
            //    await HubContext.Clients.All.SendAsync("CategoriaEditada", mapper.Map<CategoriaDTO>(categoria));
            //}
            LastUpdateManager.UpdateCategorias();
            return Ok("La categoria fue editada con exito");
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarCategoria(int id)
        {
            var categoriaExistente = Context.Categoria.Find(id);
            if (categoriaExistente == null)
            {
                return NotFound("No se pudo encontrar la categoria que desea eliminar.");
            }

            var data = repositoryProductos.GetAll().Where(x => x.IdCategoria == id).ToList();
            foreach (var item in data)
            {
                repositoryProductos.Delete(item);
            }

            Context.Remove(categoriaExistente);
            Context.SaveChanges();

            //await HubContext.Clients.All.SendAsync("CategoriaEliminada", categoriaExistente.Id);
            LastUpdateManager.UpdateCategorias();
            return Ok("La categoria fue eliminada con exito.");
        }
    }
}
