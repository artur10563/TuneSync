using Application.DTOs.Songs;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Youtube.Query.YoutubeSearch;

public sealed record YoutubeSearchQuery(string Query, int Results) : IRequest<Result<List<YoutubeSongInfo>>>;