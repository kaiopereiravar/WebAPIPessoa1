using System;
using System.Collections.Generic;
using System.Text;

namespace WebAPIPessoa.Repository.Models
{
    public class TabPessoa // Reflexo da tabela TabUsuario no banco de dados
    {
        public int id { get; set; }

        public string nome { get; set; }

        public DateTime dataNascimento { get; set; }

        public decimal altura { get; set;}

        public decimal Peso{ get; set;}

        public decimal Salário{ get; set;}

        public decimal Saldo { get; set;}

        public int idade{ get; set;}

        public decimal imc{ get; set;}

        public string Classificacao { get; set; }

        public decimal inss { get; set; }

        public decimal aliquota{ get; set; }

        public decimal SalarioLiquido{ get; set; }

        public decimal SaldoDolar{ get; set; }

        public int idUsuario{ get; set; }
    }
}
