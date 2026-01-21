using Application.BackgroundJobs;
using Infrastructure.BackgroundJobs;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static partial class DependencyContainer
{
    private static IServiceCollection AddBackgroundWorkers(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IDownloadPlaylistFromYoutubeJob, DownloadPlaylistFromYoutubeJob>();
        serviceCollection.AddTransient<IFileCleanupJob, FileCleanupJob>();
        return serviceCollection;
    }
}