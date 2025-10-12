// WebApi/Program.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Infrastructure.Data;
using Application.Interfaces;
using Infrastructure.Services;
using Application.UseCases;
using Domain.Interfaces;
using Infrastructure.Repositories;
using Application.UsesCases;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database - CORRIGIDO para MySQL
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("falae"),
        new MySqlServerVersion(new Version(8, 0, 21))
    ));

// JWT Configuration
var key = Encoding.ASCII.GetBytes("#J4wBK^h4HLqH$%zXA3Y2YWiqw8j3DUc"); // Mova para appsettings depois

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// Dependency Injection
// Repositories
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IPreferenciasRepository, PreferenciasRepository>();
builder.Services.AddScoped<ILocalEncontroRepository, LocalEncontroRepository>();
builder.Services.AddScoped<IEncontroRepository, EncontroRepository>();
builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();

// Use Cases
builder.Services.AddScoped<IUsuarioUseCase, UsuarioUseCase>();
builder.Services.AddScoped<IPreferenciasUseCase, PreferenciasUseCase>();

// Services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// CRIAR BANCO E TABELAS AUTOMATICAMENTE
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    try
    {
        dbContext.Database.EnsureCreated();
        Console.WriteLine("Banco de dados e tabelas criados com sucesso!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao criar banco: {ex.Message}");
    }
}

app.Run();