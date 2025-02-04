using Application.DTOs.Artists;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Artists.Query.GetArtistSummary;

public sealed record GetArtistSummaryQuery(Guid ArtistGuid, Guid CurrentUserGuid = default) : IRequest<Result<ArtistSummaryDTO>>;