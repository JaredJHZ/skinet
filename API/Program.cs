using System;
using System.Threading.Tasks;
using Infraestructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            // uso el using porque esta wea no tiene inyeccion de dependencias asi que asi nos aseguramos que al arrancar la app se saquen alv los
            // recursos utilizados para crear las migraciones y actualizar la bd
            using (var scope = host.Services.CreateScope()) {
                // hace referencia a los servicios
                var services = scope.ServiceProvider;
                // crear un logger para hacer los logs si hay pedos
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                try {
                    // crea el contexto de tipo StoreContext que es la clase que cree que trae todo el contexto de las conexiones a las bd
                    var context = services.GetRequiredService<StoreContext>();
                    // crea las migraciones y crea la bd
                    await context.Database.MigrateAsync();

                    await StoreContextSeed.SeedAsync(context , loggerFactory);
                }catch (Exception ex) {
                    // si hay pedos hace los logs
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError("An error ocurred during migrations!");
                }

                // despues de esto arranca la app

                host.Run();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
