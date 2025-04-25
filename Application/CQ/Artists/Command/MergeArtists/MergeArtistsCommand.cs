using Domain.Primitives;
using MediatR;

namespace Application.CQ.Artists.Command.MergeArtists;

public sealed record MergeArtistsCommand(Guid ParentId, Guid ChildId) : IRequest<Result>;