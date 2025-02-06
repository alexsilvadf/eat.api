using Entities.Entidades;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using WebApi.Models;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [AllowAnonymous]
        [Produces("application/json")]
        [HttpPost("/api/AdicionarUsuario")]
        public async Task<IActionResult> AdicionarUsuario([FromBody] Login login)
        {
            if (string.IsNullOrEmpty(login.Email) || (string.IsNullOrEmpty(login.Senha)) || (string.IsNullOrEmpty(login.Cpf))){
                return Ok("Falta alguns dados");
            }

            var user = new ApplicationUser
            {
                Email = login.Email,
                UserName = login.Email,
                Cpf = login.Cpf
            };

            var result = await _userManager.CreateAsync(user, login.Senha);

            if (result.Errors.Any())
            {
                return Ok(result.Errors);
            }

            //Geração de confirmação caso precise

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            //retorno do email

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

            var response_Retorn = await _userManager.ConfirmEmailAsync(user, code);

            if (response_Retorn.Succeeded)
            {
                return Ok("Usuário Adicionado");
            }
            else
            {
                return Ok("Erro ao confirmar cadastro do usuário");
            }
        }
    }
}
