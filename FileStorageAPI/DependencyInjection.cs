using FileStorage.App.Services;
using FileStorage.Repository.Options;
using Serilog;

namespace FileStorageAPI;

public static class DependencyInjection
{
    public static IServiceCollection AddFileStorageAPI(this IServiceCollection services, IConfiguration configuration, WebApplicationBuilder builder)
    {
        IConfiguration config = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json")
                        .Build();

        Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(config)
                        .CreateLogger();

        services.AddControllers();

        services.AddHttpClient("client", options =>
        {
            options.BaseAddress = new Uri("https://localhost:7048");
        });

        services.Configure<FileStorageConfig>(configuration.GetSection(FileStorageConfig.FileStorageConfigOptions));

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        builder.Logging.AddSerilog();

        services.AddScoped<IFileService, FileService>();

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        return services;
    }
}
