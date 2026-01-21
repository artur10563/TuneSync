using Domain.Primitives;

namespace Application.BackgroundJobs;

public sealed record DownloadPlaylistFromYoutubeJobInput(string YoutubePlaylistId, Guid CreatedBy);

public interface IDownloadPlaylistFromYoutubeJob
    : IBaseJob<DownloadPlaylistFromYoutubeJobInput, Result<Guid>>;