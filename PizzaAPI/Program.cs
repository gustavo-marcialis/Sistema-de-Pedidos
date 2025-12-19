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

            // 1. Configura a Autenticação (Lê o bloco AzureAd do appsettings)
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

            // 2. Configura o Banco de Dados (Lê a ConnectionString)
            builder.Services.AddDbContext<pizzariaContext>(
                options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Ativa o Swagger para você testar (mesmo publicado)
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseRouting();

            // 3. Configura CORS (Permite acesso de qualquer lugar por enquanto)
            app.UseCors(options => options
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            // 4. Ativa os Guardas de Segurança (Ordem obrigatória!)
            app.UseAuthentication(); // Quem é você?
            app.UseAuthorization();  // O que você pode fazer?

            app.MapControllers();

            app.Run();
        }
    }
}
