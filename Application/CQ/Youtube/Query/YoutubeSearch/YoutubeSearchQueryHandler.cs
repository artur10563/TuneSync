using Application.DTOs.Songs;
using Application.Services;
using Domain.Helpers;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Youtube.Query.YoutubeSearch;

public class YoutubeSearchQueryHandler : IRequestHandler<YoutubeSearchQuery, Result<List<YoutubeSongInfo>>>
{
    private readonly IYoutubeService _youtube;

    public YoutubeSearchQueryHandler(IYoutubeService youtube)
    {
        _youtube = youtube;
    }

    public async Task<Result<List<YoutubeSongInfo>>> Handle(YoutubeSearchQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<YoutubeSongInfo> results = new List<YoutubeSongInfo>();
        //Search videos
        if (YoutubeHelper.IsYoutubeMusic(request.Query))
        {
            results = await _youtube.SongsByPlaylistId(request.Query, request.Results);
        }
        else
        {
            results = await _youtube.SearchAsync(request.Query, request.Results);
        }
        
        return results.ToList();
    }
}