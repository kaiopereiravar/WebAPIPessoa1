using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAPIPessoa.Application.Pessoa;
using WebAPIPessoa.Repository;

namespace WebAPIPessoa.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PessoaController : ControllerBase
    {
        private readonly PessoaContext _context; // passamento do banco de dados
        public PessoaController(PessoaContext context) // quem for usar o banco de dados precisa passar o contexto
        {
            _context = context; // dentro dessa variavel ficam disponiveis as ações do banco de dados
        }

        /// <summary>
        /// Rota responsavel por realizar processamento de dados de uma pessoa - Calcula idade, imc, inss,e faz conversão de saldo para dolar
        /// </summary>
        /// <returns>Retorna os dados processados da pessoa</returns>
        /// <response code="200">Retorna os dados processados com sucesso</response> 
        /// <response code="400">Erro de validação</response>

        [HttpPost]
        [Authorize] // significa que para o usuario uar essa rota de "ProcessarInformaçõesPessoa" ele precisa de autorização
        public PessoaResponse ProcessarInformacoesPessoa([FromBody] PessoaRequest request)
        {
            var identidade = (ClaimsIdentity)HttpContext.User.Identity; // pegue o usuario que esta autenticado(o usuario do token), e coloque isso dentro da variavel "identidade"
            var usuarioId = identidade.FindFirst("usuarioId").Value; // encontre o usuario ID e guarde na variável // FindFirst = procure o primeiro // esse "usuarioId" pode ser encontrado no site "JWT.IO" quando vc coloca o token gerado do usuario tambem

            var pessoaService = new PessoaService(_context);
            var pessoaResponse = pessoaService.ProcessarInformacoesPessoa(request, Convert.ToInt32(usuarioId)); // passa para a service e, formato de int(numero)

            return pessoaResponse;
        }

        [HttpGet]
        [Authorize]
        public List<PessoaHistoricoResponse> ObterHistoricoPessoas()
        {
            var pessoaService = new PessoaService(_context);
            var pessoas = pessoaService.ObterHistoricoPessoas();

            return pessoas;
        }

        [HttpGet]
        [Authorize]
        [Route("{id}")]
        public PessoaHistoricoResponse ObterHistoricoPessoa([FromRoute]int id)
        {
            var pessoaService = new PessoaService(_context);
            var pessoa = pessoaService.ObterHistoricoPessoa(id);

            return pessoa;
        }

        [HttpDelete]
        [Authorize]
        [Route("{id}")]
        public IActionResult removerPessoa([FromRoute] int id)
        {
            var pessoaService = new PessoaService(_context);
            var sucesso = pessoaService.RemoverPessoa(id);

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
