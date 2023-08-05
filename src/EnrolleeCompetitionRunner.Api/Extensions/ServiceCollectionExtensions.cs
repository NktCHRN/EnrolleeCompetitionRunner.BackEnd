using EnrolleeCompetitionRunner.Core;
using EnrolleeCompetitionRunner.Core.Abstractions;
using EnrolleeCompetitionRunner.Domain.Abstractions;
using EnrolleeCompetitionRunner.Domain.Utilities;
using EnrolleeCompetitionRunner.Infrastructure;
using EnrolleeCompetitionRunner.Infrastructure.DataProviders.AbitPoisk.Abstractions;
using EnrolleeCompetitionRunner.Infrastructure.DataProviders.AbitPoisk.Clients;
using EnrolleeCompetitionRunner.Infrastructure.DataProviders.AbitPoisk.Scrapers;
using EnrolleeCompetitionRunner.Infrastructure.DataProviders.Common;
using EnrolleeCompetitionRunner.Infrastructure.DataProviders.Edbo.Abstractions;
using EnrolleeCompetitionRunner.Infrastructure.DataProviders.Edbo.Clients;
using EnrolleeCompetitionRunner.Infrastructure.DataProviders.Edbo.Scrapers;
using EnrolleeCompetitionRunner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace EnrolleeCompetitionRunner.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddDomainServices()
            .AddCoreServices()
            .AddInfrastructureServices(configuration)
            .AddApiServices();   
    }

    private static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        return services.AddSingleton<IDefaultQuotePlacesCalculator, DefaultQuotePlacesCalculator>()
            .AddSingleton<ISupercompetitionRunner, SupercompetitionRunner>()
            .AddSingleton<IDateTimeProvider, DateTimeProvider>();
    }

    private static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        return services.AddAutoMapper(typeof(ICoreAssemblyMarker), typeof(IInfrastructureAssemblyMarker), typeof(IApiAssemblyMarker))
            .AddMediatR(config => config.RegisterServicesFromAssemblyContaining<ICoreAssemblyMarker>())
            .AddTransient<ISupercompetitionDataProvider, SupercompetitionDataProvider>();
    }

    private static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDbContext<IApplicationDbContext, ApplicationDbContext>(options => 
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                     providerOptions =>
                     {
                         providerOptions.CommandTimeout((int)TimeSpan.FromMinutes(20).TotalSeconds);
                     }));

        services
            .AddHttpClient<AbitPoiskHttpClient>(nameof(AbitPoiskHttpClient));
        services
            .AddHttpClient<EdboHttpClient>(nameof(EdboHttpClient), config => config.Timeout = TimeSpan.FromMinutes(5));

        services
            .AddSingleton<IAbitPoiskScraper, AbitPoiskScraper>()
            .AddSingleton<IEdboScraper, EdboScraper>();

        return services;
    }

    private static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        return services;
    }
}
