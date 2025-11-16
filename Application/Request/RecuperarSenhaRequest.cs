using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Request
{
    public class RecuperarSenhaRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty; // código de verificação
        public string NovaSenha { get; set; } = string.Empty;
    }
}
