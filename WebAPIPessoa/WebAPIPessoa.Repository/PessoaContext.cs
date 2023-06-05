using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using WebAPIPessoa.Repository.Models;

namespace WebAPIPessoa.Repository
{
    public class PessoaContext : DbContext //a pessoaContext esta erdando do Dbcontext( tudo que tem no DbContext, tambem esta disponivel no PessoaContext)
    {
        public PessoaContext(DbContextOptions<PessoaContext> options) : base (options) { }//é um metodo construtor, ou seja, assim que alguem instanciar essa classe PessoaContext, quem for usar precisa me passar uma configuração, que seria no caso, o endereço do Banco de dados  
    
        public DbSet<TabUsuario> Usuarios { get; set; }//codigo indicando que voce tem uma tabela chamada TabUsuario

        public DbSet<TabPessoa> Pessoas { get; set; }//codigo indicando que voce tem uma tabela chamada TabPessoa

        protected override void OnModelCreating(ModelBuilder modelBuilder)//metodo protegido para sobrescrever(parte azul), quando o model for criado(parte amarela)
        {
            modelBuilder.Entity<TabUsuario>().ToTable("tabUsuario");//quando a aplicação estiver subindo, essa linha de codigo que faz o link
            modelBuilder.Entity<TabPessoa>().ToTable("TabPessoa");
        }

    }



    //CASO VOCE TIVESSE QUE CRIAR MAIS UMA TABELA SERIA MAIS FACIL
    //VOCE TERIA QUE:
    //
    // *Criar mais uma classe na Model com a representação da sua tabela em C#
    // *Adiciona mais uma linha igual a da linha 13 (public DbSet< "Com o nome da sua classe aqui" > "Voce define" { get; set; })
    // *e duplicar a linha 17 ( modelBuilder.Entity< "Nome da Classe aqui" >().ToTable("Nome da tabela aqui entre aspas");)
}
