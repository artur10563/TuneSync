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
    private readonly IUnitOfWork _uow;
    private readonly IYoutubeService _youtubeService;
    private readonly IStorageService _storageService;
    private readonly IValidator<CreatePlaylistFromYoutubeCommand> _validator;

    public CreatePlaylistFromYoutubeCommandHandler(IBackgroundJobService backgroundJob, IUnitOfWork uow,
        IStorageService storageService, IYoutubeService youtubeService, IValidator<CreatePlaylistFromYoutubeCommand> validator)
    {
        _backgroundJob = backgroundJob;
        _uow = uow;
        _storageService = storageService;
        _youtubeService = youtubeService;
        _validator = validator;
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


        return Result<string>.Success(jobId);
    }
}