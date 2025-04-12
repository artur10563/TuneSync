using Application.BackgroundJobs;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static partial class DependencyInjection
{
    public static IServiceCollection AddBackgroundWorkers(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<DownloadPlaylistFromYoutubeJob>();
        serviceCollection.AddTransient<FileCleanupJob>();
        return serviceCollection;
    }
}