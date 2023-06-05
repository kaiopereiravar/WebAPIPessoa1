using System;
using System.Collections.Generic;
using System.Text;

namespace WebAPIPessoa.Application.Autenticacao
{
    public class AutenticacaoResponse
    {
        public string token { get; set; }

        public int UsuarioId { get; set; }
    }
}
