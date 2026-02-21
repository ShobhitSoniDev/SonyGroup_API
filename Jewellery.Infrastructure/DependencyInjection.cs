using Jewellery.Application.Master.Interfaces;
using Jewellery.Infrastructure.Master.Repositories;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IMetalRepository, MetalRepository>();


        return services;
    }
}
