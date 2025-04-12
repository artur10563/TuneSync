using Domain.Primitives;
using MediatR;

namespace Application.CQ.Admin.Artists.Command.DeleteArtist;

public sealed record DeleteArtistCommand(Guid ArtistGuid) : IRequest<Result>;