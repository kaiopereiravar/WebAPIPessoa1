﻿using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using WebAPIPessoa.Application.Eventos;
using WebAPIPessoa.Application.Eventos.Models;
using WebAPIPessoa.Repository;
using WebAPIPessoa.Repository.Models;

namespace WebAPIPessoa.Application.Autenticacao
{
    public class AutenticacaoService : IAutenticacaoService
    {
        private readonly PessoaContext _context;//readonly significa que é somente de leitura( ninguem pode alterar o _context)
        private readonly IRabbitMQProducer _rabbitMQProducer; // Referencia da interface

        public AutenticacaoService(PessoaContext context, IRabbitMQProducer rabbitMQProducer)//metodo construtor(ou seja, sempre que alguem instanciar, chamar esse metodo, tem que passar um contexto( que é a referencia ao banco de dados)
        {
            _context = context;//passando o contexto, ele coloca o contexto dentro da variavel _context
            _rabbitMQProducer = rabbitMQProducer;
        }

        public bool EsqueciSenha(string email)
        {
            try
            {
                var UsuarioExiste = _context.Usuarios.FirstOrDefault(x => x.email == email);
                if (UsuarioExiste == null)//Se não existir esse email no banco de dados, na tabela usuarios, cancele o andamento dessa função
                {
                    return false;
                }

                var esqueciSenhaModel = new EsqueciSenhaModel()//Se existir esse email, passe o "Email", defina que é uma "recuperação de senha", e passe a senha esquecida
                {
                    Email = email,
                    Assunto = "Recuperação de senha",
                    Texto = $"Você solicitou a recuperação de senha. Sua senha é: <b>{UsuarioExiste.senha}</b>" //A melhor forma de concatenar
                };

                _rabbitMQProducer.EnviarMensagem(esqueciSenhaModel, "Var.Notificacao.Email", "Var.Notificacao", "Var.Notificacao");//Esse é o nome da fila e da eschange la no rabbit
                
                return true;
            }

            catch ( Exception ex)//Tudo que for excessao ira cair aqui
            {
                return false;
            }
        }

        public AutenticacaoResponse Autenticar(AutenticacaoRequest request)
        {
            var usuario = _context.Usuarios.FirstOrDefault(x => x.usuario == request.UserName && x.senha == request.Password);
            if (usuario != null)
            {
                var tokenString = GerarTokenJwt(usuario);
                var resposta = new AutenticacaoResponse()
                {
                    token = tokenString,
                    UsuarioId = usuario.id
                };

                return resposta;
            }

            else
            {
                return null;
            }
        }

        private string GerarTokenJwt(TabUsuario usuario)
        {
            var issuer = "var"; //Quem esta emitindo o token
            var audience = "var"; //Pra quem vai liberar o acesso
            var key = "8fab334a-5e6f-4d7c-8d8c-c35241836bd6"; //é uma chave secreta (atraves do gerador de gui on-line)
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));//pegou a "key", transformou em um bite que é oq o SymetricSecurityKey espera , e colocou dentro da securityKey
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);//Pega o securityKey, e fala que é uma credencial de identificação, e diz que é do modelo de criptografia "HmacSha256"

            var claims = new[]
            {
                new System.Security.Claims.Claim("usuarioId",usuario.id.ToString()) // na hora de gerar o token, guarda aqui que quem gerou o token foi esse "usuarioId", e o id do usuario, foi o que buscou no banco (usuario.id.ToString())
                // Um Claim contém uma informação específica sobre o usuário, como o nome, email, permissões de acesso, dentre outras. Essas informações podem ser utilizadas para verificar se o usuário tem as permissões necessárias para acessar determinado recurso ou funcionalidade do sistema.
            };

            var token = new JwtSecurityToken(issuer: issuer, claims: claims, audience: audience, expires: DateTime.Now.AddMinutes(60), signingCredentials: credentials); // Essa é a linha onde passamos parametros para passar o token
            var tokenHandler = new JwtSecurityTokenHandler();
            var stringToken = tokenHandler.WriteToken(token);
            return stringToken;

        }
    }
}
