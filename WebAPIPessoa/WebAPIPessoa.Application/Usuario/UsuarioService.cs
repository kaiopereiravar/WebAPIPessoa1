using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebAPIPessoa.Repository;
using WebAPIPessoa.Repository.Models;

namespace WebAPIPessoa.Application.Usuario
{
    public class UsuarioService
    {

        private readonly PessoaContext _context;
        public UsuarioService(PessoaContext context)
        {
            _context = context;
        }
        public bool InserirUsuario(UsuarioRequest request)
        {
            try // tente fazer o que esta dentro das chaves
            {
                var usuarioExiste = _context.Usuarios.FirstOrDefault(x => x.email == request.email);
                if (usuarioExiste != null)
                {
                    return false;
                }
               
                var usuario = new TabUsuario() // TabUsuario é a representação da minha tabela do banco de dados (pois o nome dela é TabUsuario)
                {
                    nome = request.Nome,
                    usuario = request.Usuario,
                    senha = request.Senha,
                    email = request.email
                };

                _context.Usuarios.Add(usuario); // esta adicionando no banco de dados
                _context.SaveChanges(); // efetiva a transação

                return true;               
            }

            catch (Exception ex) // caso ocorra algum erro nas chaves acima, execute o que esta aqui
            {
                return false;
            }

        }

        public List<TabUsuario> ObterUsuarios() // List (TabUsuario) significa que eu vou trazer uma lista da tabela, ou seja, vou retornar VARIOS USUARIOS
        {
            try
            {
                var usuarios = _context.Usuarios.ToList(); // _context é o meu banco de dados, essa linha significa => Vá no meu banco de dados, na tabela Usuarios e pegue tudo que esta la e me retorne (ToList())
                return usuarios;
            }

            catch (Exception ex)
            {
                return null;
            }
        }   

        public TabUsuario ObterUsuario(int id)
        {
            try
            {
                var usuario = _context.Usuarios.FirstOrDefault(x => x.id == id); // Essa linha de comando significa para ele ir no banco de dados, na tabela Usuarios(_context.Usuarios), selecionar o primeiro objeto Usuario do banco de dados(FirstOrDefaul) que tenha a variavel igual, ao o ID do usuario precisa ser igual ao ID que voce recebeu como parametro na linha 46 (x => x.id == id)
                return usuario;
            }

            catch (Exception ex)
            {
                return null;
            }

            // A diferença entre try e catch, e if e else, é que:
            // Try e catch é para TRATAMENTO DE ERROS, e
            // if e else é para TOMADA DE DECISÕES.
        }

        public bool AtualizarUsuario(int id, UsuarioRequest request)
        {
            try //tratamento de erros
            {
                var usuarioDb = _context.Usuarios.FirstOrDefault(x => x.id == id);//aqui ele esta fazendo o mesmo que "Select * from TabUsuario where id = ()" no banco de dados
                if(usuarioDb == null) // Ver se o id é valido, se nao for, retornar "False"
                    return false;

                usuarioDb.nome = request.Nome; //Se o id for valido, passar essas informações para o banco de dados
                usuarioDb.senha = request.Senha;
                usuarioDb.usuario = request.Usuario;
                usuarioDb.email = request.email;

                _context.Usuarios.Update(usuarioDb);
                _context.SaveChanges();//salvar a transação

                return true;
                
            }
            catch(Exception ex)//tratamento de erros
            {
                return false;
            }
         } 

        public bool RemoverUsuario(int id)
        {
            try
            {
                var usuarioDb = _context.Usuarios.FirstOrDefault(x => x.id == id); //Esse metodo é para ver se o ID é válido
                if (usuarioDb == null) // Ver se o id é valido, se nao for, retornar "False"
                    return false;

                //No caso do id for valido, ele não vai entrar no if

                _context.Usuarios.Remove(usuarioDb); //Vá no banco de dados, na tabela Usuarios, e remova o usuario(com o id dele)
                _context.SaveChanges();

                return true;
            }
            catch( Exception ex )
            {
                return false;
            }
        }
    }
}
