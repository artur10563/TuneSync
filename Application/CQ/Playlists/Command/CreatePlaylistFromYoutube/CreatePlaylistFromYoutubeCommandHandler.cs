using Application.BackgroundJobs;
using Application.Extensions;
using Application.Repositories.Shared;
using Application.Services;
using Domain.Errors;
using Domain.Primitives;
using FluentValidation;
using MediatR;

namespace Application.CQ.Playlists.Command.CreatePlaylistFromYoutube;

internal class
    CreatePlaylistFromYoutubeCommandHandler : IRequestHandler<CreatePlaylistFromYoutubeCommand, Result<string>>
{
    private readonly IBackgroundJobService _backgroundJob;
    private readonly IValidator<CreatePlaylistFromYoutubeCommand> _validator;
    private readonly ILoggerService _logger;

    public CreatePlaylistFromYoutubeCommandHandler(IBackgroundJobService backgroundJob, IUnitOfWork uow,
        IStorageService storageService, IYoutubeService youtubeService, IValidator<CreatePlaylistFromYoutubeCommand> validator, ILoggerService logger)
    {
        _backgroundJob = backgroundJob;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<string>> Handle(CreatePlaylistFromYoutubeCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.AsErrors();

        if (_backgroundJob.IsRunning(request.PlaylistId))
        {
            return new Error("Playlist is being processed");
        }

        var jobId = _backgroundJob.Enqueue<DownloadPlaylistFromYoutubeJob>(
            job => job.ExecuteAsync(request.PlaylistId, request.CreatedBy, cancellationToken));

        _logger.Log("Job started", LogLevel.Information, new { jobId, request });

        return jobId;
    }
}