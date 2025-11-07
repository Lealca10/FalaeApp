using System;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Infrastructure.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly ILogger<PasswordService> _logger;

        public PasswordService(ILogger<PasswordService> logger)
        {
            _logger = logger;
        }

        public string HashPassword(string password)
        {
            var stackTrace = new StackTrace();
            var callingMethod = stackTrace.GetFrame(1)?.GetMethod();
            var callingClass = callingMethod?.DeclaringType?.Name;
            var callingMethodName = callingMethod?.Name;

            _logger.LogInformation($"HashPassword chamado por: {callingClass}.{callingMethodName}");

            if (password == null)
            {
                _logger.LogError($"HashPassword recebeu NULL do chamador: {callingClass}.{callingMethodName}");
                throw new ArgumentNullException(nameof(password));
            }

            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            var stackTrace = new StackTrace();
            var callingMethod = stackTrace.GetFrame(1)?.GetMethod();
            var callingClass = callingMethod?.DeclaringType?.Name;
            var callingMethodName = callingMethod?.Name;

            _logger.LogInformation($"VerifyPassword chamado por: {callingClass}.{callingMethodName}");

            if (password == null)
            {
                _logger.LogError($"VerifyPassword - Password NULL do chamador: {callingClass}.{callingMethodName}");
                return false;
            }

            if (hashedPassword == null)
            {
                _logger.LogError($"VerifyPassword - Hash NULL do chamador: {callingClass}.{callingMethodName}");
                return false;
            }

            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro no BCrypt.Verify - Chamador: {callingClass}.{callingMethodName}");
                return false;
            }
        }
    }
}