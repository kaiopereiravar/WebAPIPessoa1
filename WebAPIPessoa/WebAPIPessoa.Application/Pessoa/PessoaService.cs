using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebAPIPessoa.Application.Cache;
using WebAPIPessoa.Repository;
using WebAPIPessoa.Repository.Models;

namespace WebAPIPessoa.Application.Pessoa
{
    public class PessoaService
    {
        private readonly PessoaContext _context; // passamento do banco de dados
        private readonly ICacheService _cacheService;
        public PessoaService(PessoaContext context, ICacheService cacheService) // quem for usar o banco de dados precisa passar o contexto
        {
            _context = context; // dentro dessa variavel ficam disponiveis as ações do banco de dados
            _cacheService = cacheService;
        }

        public bool RemoverPessoa(int id)
        {
            try
            {
                var usuarioDb = _context.Pessoas.FirstOrDefault(x => x.id == id);
                if (usuarioDb == null) // Ver se o id é valido, se nao for, retornar "False"
                    return false;

                _context.Pessoas.Remove(usuarioDb); //Vá no banco de dados, na tabela Usuarios, e remova o usuario(com o id dele)
                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }        

        }

        public PessoaHistoricoResponse ObterHistoricoPessoa(int id)
        {
            var chave = $"pessoa_{id}";
            var cachePessoa = _cacheService.Get<PessoaHistoricoResponse>(chave);
            if (cachePessoa != null )
            {
                return cachePessoa;
            }

            var pessoaDb = _context.Pessoas.FirstOrDefault(x => x.id == id);
            if (pessoaDb == null)
                return null;


            var pessoa = new PessoaHistoricoResponse()
            {
                Aliquota = Convert.ToDouble(pessoaDb.aliquota),
                altura = pessoaDb.altura,
                Classificacao = pessoaDb.Classificacao,
                DataNascimento = pessoaDb.dataNascimento,
                Id = pessoaDb.id,
                Idade = pessoaDb.idade,
                idUsuario = pessoaDb.idUsuario,
                imc = pessoaDb.imc,
                Inss = Convert.ToDouble(pessoaDb.inss),
                Nome = pessoaDb.nome,
                Peso = pessoaDb.Peso,
                Salario = Convert.ToDouble(pessoaDb.Salário),
                SalarioLiquido = Convert.ToDouble(pessoaDb.Saldo),
                SaldoDolar = pessoaDb.SaldoDolar
            };

            _cacheService.Set(chave, pessoa, 60);

            return pessoa;
        }

        public List<PessoaHistoricoResponse> ObterHistoricoPessoas()
        {
            var pessoasDb = _context.Pessoas.ToList();
            var pessoas = new List<PessoaHistoricoResponse>(); //lista vazia

            foreach(var item in pessoasDb) // foreach = fazer varias vezes a mesma coisa para todas as pessoas de uma lista (vai linha a linha) // pra ele fazer o que estiver dentro da chaves, para todos os itens do pessoasDb(nosso banco de dados) //ele pega o item atual do banco de dados e joga dentro da variavel(item) para cada vez que ele se repetir(ele repete de acordo com o numero de usuarios que estão dentro do banco de dados) 
            {
                pessoas.Add(new PessoaHistoricoResponse()//adicionar dentro da variavel(pessoas), as pessoas que estao no banco de dados //A variavel pessoas é uma lista
                { 
                    Aliquota = Convert.ToDouble(item.aliquota),
                    altura = item.altura,
                    Classificacao = item.Classificacao,
                    DataNascimento = item.dataNascimento,
                    Id = item.id,
                    Idade = item.idade,
                    idUsuario = item.idUsuario,
                    imc = item.imc,
                    Inss = Convert.ToDouble(item.inss),
                    Nome = item.nome,
                    Peso = item.Peso,
                    Salario = Convert.ToDouble(item.Salário),
                    SalarioLiquido = Convert.ToDouble(item.Saldo),
                    SaldoDolar = item.SaldoDolar
                });   
            }

            return pessoas;
        }

        public PessoaResponse ProcessarInformacoesPessoa(PessoaRequest request,int usuarioId ) // usuarioId criado na PessoaController
        {
            var idade = CalcularIdade(request.DataNascimento);//LINHA DE COMANDO PARA DESCOBRI A IDADE DA PESSOA

            var imc = CalcularImc(request.Peso, request.altura);// o peso que a pessoa te enviou, esta no pessoa "Request"

            var classificacao = CalcularClassificacao(imc); //OBTER A CLASSIFICAÇÃO DO IMC

            var aliquota = CalcularAliquota(request.Salario); //ALIQUOTA

            var inss = CalcularInss(request.Salario, aliquota);//CALCULO DO INSS

            var salarioLiquido = SalarioLiquido(request.Salario, inss);//CALCULAR SALARIO LIQUIDO

            var saldoDolar = CalcularDolar(request.Saldo);//TEU SALDO TRANSFORMADO EM DOLAR


            //INSTANCIAMENTO DOS OBJETOS // RESPOSTA
            var resposta = new PessoaResponse();
            resposta.SaldoDolar = saldoDolar;
            resposta.Aliquota = aliquota;
            resposta.SalarioLiquido = salarioLiquido;
            resposta.Classificacao = classificacao;
            resposta.Idade = idade;
            resposta.imc = imc;
            resposta.Inss = inss;
            resposta.Nome = request.Nome;

            var pessoa = new TabPessoa() //SALVA NO BANCO DE DADOS 
            {
                aliquota = Convert.ToDecimal(aliquota), // dentro da minha tabela (TabPessoa), eu estou colocando a coluno aliquota, que recebe a variavel aliquota (a variavel aliquota está declarada mais acima, que é o calculo da minha aliquota)
                altura = request.altura,
                Classificacao = classificacao,
                dataNascimento = request.DataNascimento,
                idade = idade,
                idUsuario = usuarioId, // ele recebe o usuarioId mostrado nos parenteses dessa função(salvando o usuario de acordo com quem fez essa requisição)
                imc = imc,
                inss = Convert.ToDecimal(inss),
                nome = request.Nome,
                Peso =request.Peso,
                Salário = Convert.ToDecimal(request.Salario),
                SalarioLiquido = Convert.ToDecimal(salarioLiquido), // Precisou fazer essa coversão pois em um local estava como Double, e em outro estava como decimal
                Saldo = request.Saldo,
                SaldoDolar = saldoDolar
            };

            _context.Pessoas.Add(pessoa);
            _context.SaveChanges();

            return resposta;

            //NO INSTANCIAMENTO SIGNIFICA QUE:
            /*
             pegue o valor do "SaldoDolar", que esta dentro da variavel "resposta", 
            que é a variavel "resposta" é a "PessoaResponse()", e depois me retorne a resposta
            para quem requisitou a nossa API (no caso, o front-end)
             */



        }
        private int CalcularIdade(DateTime dataNascimento)
        {
            var anoAtual = DateTime.Now.Year;
            var idade = anoAtual - dataNascimento.Year;

            var mesAtual = DateTime.Now.Month;
            if (mesAtual < dataNascimento.Month)
            {
                idade = idade - 1;
                //idade =- 1;
            }

            return idade;
        }

        private decimal CalcularImc(decimal peso, decimal altura)
        {
            var imc = Math.Round(peso / (altura * altura), 2);
            return imc;
        }

        private string CalcularClassificacao(decimal imc)
        {
            var classificacao = "";

            if (imc < (decimal)18.5)
            {
                classificacao = "Abaixo do peso ideal";
            }

            else if (imc >= (decimal)18.5 && imc <= (decimal)24.99)
            {
                classificacao = "Peso normal";
            }

            else if (imc >= (decimal)25 && imc <= (decimal)24.99)
            {
                classificacao = "Pré obesidade";
            }

            else if (imc >= (decimal)25 && imc <= (decimal)34.99)
            {
                classificacao = "Obesidade grau 1";
            }

            else if (imc >= (decimal)35 && imc <= (decimal)39.99)
            {
                classificacao = "Obesidade grau 2";
            }

            else
            {
                classificacao = "Obesidade grau 3";
            }

            return classificacao;
        }

        private double CalcularAliquota(double salario)
        {
            var aliquota = 7.5;
            if (salario <= 1212)//O "Salario" está dentro do request(input)
            {
                aliquota = 7.5;
            }

            else if (salario >= 1212.01 && salario <= 2427.35)
            {
                aliquota = 9;
            }

            else if (salario >= 2427.36 && salario <= 3641.03)
            {
                aliquota = 12;
            }

            else
            {
                aliquota = 14;
            }

            return aliquota;
        }

        private double CalcularInss(double salario, double aliquota)
        {
            var inss = (salario * aliquota) / 100;
            return inss;
        }

        private double SalarioLiquido(double salario, double inss)
        {
            var salarioLiquido = salario - inss;
            return salarioLiquido;
        }

        private decimal CalcularDolar(decimal Saldo)
        {
            var dolar = (decimal)5.14;
            var saldoDolar = Math.Round(Saldo / dolar, 2);
            return saldoDolar;
        }
    }
}
