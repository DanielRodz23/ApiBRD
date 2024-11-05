using System.Security.Principal;
using ApiBRD.Helpers;
using ApiBRD.Models.DTOs;
using ApiBRD.Models.Entities;
using ApiBRD.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiBRD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly Repository<Usuario> repositoryUsuario;
        private readonly JwtTokerGenerator tokenGenerator;
        public AuthenticationController(Repository<Usuario> repositoryUsuario, JwtTokerGenerator tokenGenerator)
        {
            this.repositoryUsuario = repositoryUsuario;
            this.tokenGenerator = tokenGenerator;
        }
        [HttpPost]
        public async Task<IActionResult> Post(LoginDto dto)
        {
            var user = repositoryUsuario.GetAll().FirstOrDefault(x => x.Username == dto.Username && x.Password == Encriptacion.StringToSHA512(dto.Password));
            if (user == null) return BadRequest("");
            var token = tokenGenerator.GetToken(user.Username ?? "", "Admin", user.Id, []);

            return Ok(token);

        }
    }
}
