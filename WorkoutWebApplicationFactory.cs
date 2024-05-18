using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Writers;
using Workout_API;
using Workout_API.DBContexts;

namespace Workout_API_Test_Suite
{
    /// <summary>
    /// Bootstrap a web api environment to simulate the workout api in a production environment
    /// 
    /// The connection string used will be overwritten, this should point to a test database to ensure
    /// no impact on production during testing
    /// </summary>
    internal class WorkoutWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                ConnectToTestDatabase(services);

                var dbContext = CreateDatabaseContext(services);

                EnsureCleanTestDatabase(dbContext);
            });
        }

        private static DBContext CreateDatabaseContext(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var Scope = serviceProvider.CreateScope();
            var dbContext = Scope.ServiceProvider.GetRequiredService<DBContext>();

            return dbContext;
        }

        private static void ConnectToTestDatabase(IServiceCollection services)
        {
            services.RemoveAll(typeof(DbContextOptions<DBContext>));

            var connectionString = Utils.GetConfigurationItem("TESTConnectionString");
            services.AddSqlServer<DBContext>(connectionString);
        }

        private static void EnsureCleanTestDatabase(DBContext dbContext)
        {
            lock(dbContext)
            {
                if (dbContext.Database.CanConnect())
                {
                    dbContext.Database.EnsureDeleted();
                    dbContext.Database.EnsureCreated();
                }
            }
        }
    }
}
