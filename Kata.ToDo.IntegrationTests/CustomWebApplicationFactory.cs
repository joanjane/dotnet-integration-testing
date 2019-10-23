using Kata.ToDo.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Kata.ToDo.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Startup>
    {
        public delegate Task Seed(ApplicationDbContext dbContext);
        private readonly Seed _seed;

        public CustomWebApplicationFactory(Seed seed = null)
        {
            _seed = seed;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((ctx, configurationBuilder) =>
                {
                    configurationBuilder.AddJsonFile("appsettings.Testing.json");
                })
                .ConfigureServices(async (ctx, services) =>
                {
                    if (ctx.Configuration.GetValue<bool>("UseInMemory"))
                    {
                        var serviceProvider = new ServiceCollection()
                            .AddEntityFrameworkInMemoryDatabase()
                            .BuildServiceProvider();

                        services.AddDbContext<ApplicationDbContext>(options =>
                        {
                            options.UseInMemoryDatabase("InMemoryDbForTesting");
                            options.UseInternalServiceProvider(serviceProvider);
                        });
                    }
                    else
                    {
                        services.AddDbContext<ApplicationDbContext>(
                            options => options.UseSqlServer(ctx.Configuration["DbConnectionString"]));
                    }

                    // Build the service provider.
                    var sp = services.BuildServiceProvider();

                    using (var scope = sp.CreateScope())
                    {
                        var scopedServices = scope.ServiceProvider;
                        var db = scopedServices.GetRequiredService<ApplicationDbContext>();

                        // This reduces performance. Better delete DB data instead of hole DB
                        //db.Database.EnsureDeleted();

                        db.Database.EnsureCreated();
                        ClearDatabase(db);
                        
                        // Seed the database with test data.
                        if (_seed != null)
                        {
                            await _seed(db);
                        }
                    }
                });
        }

        private static void ClearDatabase(ApplicationDbContext db)
        {
            db.ToDos.RemoveRange(db.ToDos);
            db.SaveChanges();
        }
    }
}
