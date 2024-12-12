using Application.DTOs.Playlists;
using Application.Repositories.Shared;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQ.Playlists.Query.GetPlaylistsByUser
{
    internal sealed class GetPlaylistsByUserCommandHandler : IRequestHandler<GetPlaylistsByUserCommand, Result<List<PlaylistSummaryDTO>>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GetPlaylistsByUserCommandHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<Result<List<PlaylistSummaryDTO>>> Handle(GetPlaylistsByUserCommand request, CancellationToken cancellationToken)
        {
            if (request.UserGuid == Guid.Empty)
                return Error.NotFound(nameof(Playlist));

            var userPlaylists = _uow.PlaylistRepository
                .Where(x => x.CreatedBy == request.UserGuid && x.Source == GlobalVariables.PlaylistSource.User,
                    asNoTracking: true)
                .ProjectTo<PlaylistSummaryDTO>(_mapper.ConfigurationProvider)
                .ToList();

            return Result.Success(userPlaylists);
        }
    }
}
