using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WA1.Data;
using Microsoft.AspNetCore.Identity; // for PasswordHasher<>
using WA1.Models;

namespace WA1.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private const string TestConnectionString =
            "Server=localhost;Port=3306;Database=StudentDb_Test;User=root;Password=Admin@123!;";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<AppDbContext>(options =>
                    options.UseMySql(TestConnectionString, ServerVersion.AutoDetect(TestConnectionString)));
            });

            builder.ConfigureAppConfiguration((context, config) =>
            {
                // ensures JWT config etc. still comes through for the test host
            });
        }

        public async Task InitializeDatabaseAsync()
        {
            using var scope = Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            var hasher = new PasswordHasher<User>();
            var admin = new User
            {
                Username = "testadmin",
                Role = "Admin"
            };
            admin.PasswordHarsh = hasher.HashPassword(admin, "AdminPass123!");

            context.Users.Add(admin);
            await context.SaveChangesAsync();
        }
    }
}