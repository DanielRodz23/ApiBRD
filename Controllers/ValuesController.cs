using ApiBRD.Models.DTOs;
using ApiBRD.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiBRD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly LabsystePwaBrdContext context;

        public ValuesController(LabsystePwaBrdContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(context.Producto.ToList());
        }

        [HttpPost]
        public async Task<IActionResult> Post(CategoriaDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
            {
                return BadRequest();
            }

            Categoria d = new()
            {
                Id = 0,
                Nombre = dto.Nombre
            };
            context.Add(d);
            context.SaveChanges();

            

        }
    
    }
}
