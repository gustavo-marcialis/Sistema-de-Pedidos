using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.EntityFrameworkCore;
using PizzaAPI.Models;

namespace PizzaAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Configuração de Segurança (SC-900: Identidade como Perímetro)
            // Isso prepara a API para validar tokens vindos do Microsoft Entra ID
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

            // 2. Configuração do Banco de Dados
            builder.Services.AddDbContext<pizzariaContext>(
                options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Swagger habilitado para facilitar testes e demonstrações
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseRouting();

            // 3. Configuração de CORS (Permite que o Frontend Next.js chame esta API)
            app.UseCors(options => options
                .AllowAnyOrigin() // Em produção real, restrinja para o domínio da Vercel
                .AllowAnyMethod()
                .AllowAnyHeader());

            // 4. Ativação dos Middlewares de Segurança (Ordem é crucial!)
            app.UseAuthentication(); // Verifica QUEM é o usuário
            app.UseAuthorization();  // Verifica O QUE ele pode fazer

            app.MapControllers();

            app.Run();
        }
    }
}
