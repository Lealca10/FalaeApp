using Application.Interfaces;

namespace Infrastructure.Services
{
    public class PasswordService : IPasswordService
    {
        // Apenas retorna a senha como está
        public string HashPassword(string password)
        {
            return password;
        }

        // Compara diretamente o texto
        public bool VerifyPassword(string hashedPassword, string password)
        {
            return hashedPassword == password;
        }
    }
}
