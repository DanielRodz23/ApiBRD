using ApiBRD.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ApiBRD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        public ProductosController(LabsystePwaBrdContext context)
        {
            Context = context;
        }

        public LabsystePwaBrdContext Context { get; }
    }
}
