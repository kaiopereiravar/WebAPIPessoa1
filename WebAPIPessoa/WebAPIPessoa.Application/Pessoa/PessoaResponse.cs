﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WebAPIPessoa.Application.Pessoa
{
    public class PessoaResponse
    {
        public string Nome { get; set; }

        public int Idade { get; set; }

        public decimal imc { get; set; }

        public string Classificacao { get; set; }

        public double Inss { get; set; }

        public double Aliquota { get; set; }

        public double SalarioLiquido { get; set; }

        public decimal SaldoDolar { get; set; }
    }
}
