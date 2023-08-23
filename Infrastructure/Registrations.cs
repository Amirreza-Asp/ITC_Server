using Application.Repositories;
using Application.Services;
using Infrastructure.Initializer;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Infrastructure
{
    public static class Registrations
    {

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Sql context
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                //options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
                options.UseInMemoryDatabase("ITC");
            });

            // Initializer
            services.AddScoped<IDbInitializer, DbInitializer>();

            // Repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // MediatR
            services.AddMediatR(Assembly.GetExecutingAssembly());

            // AutoMapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return services;
        }


    }
}
