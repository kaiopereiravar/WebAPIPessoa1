﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WebAPIPessoa.Application.Pessoa
{
    public class PessoaRequest
    {
        public string Nome { get; set; }

        public DateTime DataNascimento { get; set; }

        public decimal altura { get; set; }


        public decimal Peso { get; set; }

        public double Salario { get; set; }

        public decimal Saldo { get; set; }
    }
}
