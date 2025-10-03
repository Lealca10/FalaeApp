using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Response
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public UsuarioResponse Usuario { get; set; } = new();
    }
}
