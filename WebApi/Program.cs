
using Application.Interfaces;
using Application.UsesCases;
using Domain.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.BaseDados;
using Infrastructure.Repositories;
using WebApi.Validation;
using Infrastructure.Data;

namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddSingleton<IDBContext, DataBaseContext>();
            builder.Services.AddScoped<IAdicionarFaturaUseCase, AdicionarFaturaUseCases>();
            builder.Services.AddScoped<IConsultarFaturaEntidade, ConsultarFaturaRepositorio>();
            builder.Services.AddScoped<ObterFaturasUseCase>();

            builder.Services.AddScoped<IAdicionarFaturaEntidade, AdicionarFaturaRepositorio>();

            builder.Services.AddTransient < IValidator, ValidarRequisicaoFatura>();
            builder.Services.AddControllers().AddFluentValidation(fv => { fv.RegisterValidatorsFromAssemblyContaining<ValidarRequisicaoFatura>(); });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            // Database
            builder.Services.AddDbContext<DatabaseContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Dependency Injection
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            builder.Services.AddScoped<IPreferenciasRepository, PreferenciasRepository>();
            builder.Services.AddScoped<ILocalEncontroRepository, LocalEncontroRepository>();
            builder.Services.AddScoped<IEncontroRepository, EncontroRepository>();
            builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();

            builder.Services.AddScoped<IUsuarioUseCase, UsuarioUseCase>();
            builder.Services.AddScoped<IPreferenciasUseCase, PreferenciasUseCase>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}