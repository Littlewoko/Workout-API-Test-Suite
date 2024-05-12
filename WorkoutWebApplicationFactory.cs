using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Writers;
using Workout_API;
using Workout_API.DBContexts;

namespace Workout_API_Test_Suite
{
    /// <summary>
    /// Bootstrap workout api and redirect connection to test database
    /// </summary>
    internal class WorkoutWebApplicationFactory : WebApplicationFactory<Program>
    {
        /// <summary>
        /// Configure services and overwrite db context connection string
        /// </summary>
        /// <param name="builder"></param>
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                // remove current options
                services.RemoveAll(typeof(DbContextOptions<DBContext>));
                // use test database connection string
                services.AddSqlServer<DBContext>(ConnectionString.GetConnectionString());

                var dbContext = CreateDbContext(services);
                // if database exists destroy it so that we can have a clean database for integration testing
                dbContext.Database.EnsureDeleted();
            });
        }

        /// <summary>
        /// Creates a new db context object
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        private static DBContext CreateDbContext(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var Scope = serviceProvider.CreateScope();
            var dbContext = Scope.ServiceProvider.GetRequiredService<DBContext>();
            return dbContext;
        }
    }
}
