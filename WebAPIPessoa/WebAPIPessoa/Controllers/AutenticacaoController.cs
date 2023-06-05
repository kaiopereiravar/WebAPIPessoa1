using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using WebAPIPessoa.Application.Autenticacao;
using WebAPIPessoa.Application.Eventos;
using WebAPIPessoa.Repository;

namespace WebAPIPessoa.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AutenticacaoController : ControllerBase
    {
        private readonly IAutenticacaoService _autenticacaoService;

        public AutenticacaoController(IAutenticacaoService autenticacaoService)
        {
            _autenticacaoService = autenticacaoService;
        }

        [HttpPost]
        public IActionResult Login([FromBody] AutenticacaoRequest request)
        {
            var resposta = _autenticacaoService.Autenticar(request);

            if (resposta == null) 
            {
                return Unauthorized();
            }

            else
            {
                return Ok(resposta);
            }

        }

        [HttpPost]
        [Route("esqueciSenha")]
        public IActionResult Esquecisenha([FromBody] EsqueciSenhaRequest request)
        {
            var resposta = _autenticacaoService.EsqueciSenha(request.Email);

            if (resposta )
                return NoContent();

            else
                return BadRequest();
        }

    }
}
