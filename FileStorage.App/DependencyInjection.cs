using FileStorage.Repository.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace FileStorage.App;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        //services.AddScoped(typeof(IFileService), typeof(FileService));
        services.AddScoped<IFileRepository, FileRepository>();

        return services;
    }
}
