using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPIPessoa.Application.Usuario;
using WebAPIPessoa.Repository;

namespace WebAPIPessoa.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly PessoaContext _context;

        public UsuarioController(PessoaContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult InserirUsuario([FromBody] UsuarioRequest request)
        {
            var usuarioService = new UsuarioService(_context);//a comunicação com o banco de dados esta dentro da Service
            var sucesso = usuarioService.InserirUsuario(request);

            if (sucesso == true)
            {
                return NoContent();
            }
            else
            {
                return BadRequest();
            }
        }
        
        [HttpGet]
        [Authorize]
        public IActionResult ObterUsuarios()//IActionResult é um método de ação retorne uma variedade de tipos de resultado, como uma exibição, um redirecionamento, um arquivo, um status de erro ou um objeto JSON.
        {
            var usuarioService = new UsuarioService(_context);
            var usuarios = usuarioService.ObterUsuarios();

            if(usuarios == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(usuarios);
            }
        }

        [HttpGet]
        [Route("{id}")]//para ele requisitar o ID do usuario da rota ObterUsuarios() gerada pelo Banco de dados
        [Authorize]
        public IActionResult ObterUsuario([FromRoute] int id)//ObterUsuario() pois ele vai obter informações especificas de apenas UM USUARIO, diferete do ObterUsuarios(), que vai obter informações de todos os usuarios que estiverem no banco de dados
        {
            var usuarioService = new UsuarioService(_context);
            var usuario = usuarioService.ObterUsuario(id);

            if (usuario == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(usuario);
            }
        }

        [HttpPut]
        [Route("{id}")] //recebe na rota o id do usuario que voce quer atualizar
        [Authorize]

        public IActionResult AtualizarUsuario([FromRoute]int id, [FromBody] UsuarioRequest request)//Ele precisa mandar qual o id, e as informações que estão no request(Nome, Usuario e senha)
        {
            var usuarioService = new UsuarioService(_context);
            var sucesso = usuarioService.AtualizarUsuario(id, request); // chamando o nosso metodo criado (AtualizarUsuario), que esta na UsuarioService

            if (sucesso == true) // Se a Service retornar verdadeiro, retorne um 204 (sucesso)
            {
                return NoContent();
            }
            else // Se não, retorne BadRequest( codigo 400)
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize]

        public IActionResult RemoverUsuario([FromRoute]int id)
        {
            var usuarioService = new UsuarioService(_context);//passando o banco de dados
            var sucesso = usuarioService.RemoverUsuario(id);//passando o metodo

            if (sucesso == true) // Se a Service retornar verdadeiro, retorne um 204 (sucesso)
            {
                return NoContent();
            }
            else // Se não, retorne BadRequest( codigo 400)
            {
                return BadRequest();
            }
        }
    }
}
