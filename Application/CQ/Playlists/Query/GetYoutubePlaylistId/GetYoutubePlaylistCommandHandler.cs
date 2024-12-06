using Application.Extensions;
using Application.Services;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
using FluentValidation;
using MediatR;

namespace Application.CQ.Playlists.Query.GetYoutubePlaylistId;

public class GetYoutubePlaylistCommandHandler : IRequestHandler<GetYoutubePlaylistCommand, Result<string>>
{
    private readonly IYoutubeService _youtubeService;
    private readonly IValidator<GetYoutubePlaylistCommand> _validator;

    public GetYoutubePlaylistCommandHandler(IYoutubeService youtubeService, IValidator<GetYoutubePlaylistCommand> validator)
    {
        _youtubeService = youtubeService;
        _validator = validator;
    }


    public async Task<Result<string>> Handle(GetYoutubePlaylistCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return validationResult.AsErrors();
        
        var playlistUrl = await _youtubeService.SearchPlaylistBySongAndAuthorAsync(request.ChannelId, request.SongTitle);
        if (string.IsNullOrEmpty(playlistUrl))
            return Error.NotFound(nameof(Playlist));
        return playlistUrl;
    }
}