using Application.Extensions;
using Application.Repositories.Shared;
using Domain.Entities;
using Domain.Primitives;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQ.Playlists.Command.Create
{
    internal class CreatePlaylistCommandHandler : IRequestHandler<CreatePlaylistCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IValidator<CreatePlaylistCommand> _validator;
        public CreatePlaylistCommandHandler(IUnitOfWork uow, IValidator<CreatePlaylistCommand> validator)
        {
            _uow = uow;
            _validator = validator;
        }

        public async Task<Result<Guid>> Handle(CreatePlaylistCommand request, CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
                return validationResult.AsErrors();

            var newPlaylist = new Playlist(request.PlaylistName, request.CreatedBy);
            _uow.PlaylistRepository.Insert(newPlaylist);
            await _uow.SaveChangesAsync();
            return newPlaylist.Guid;
        }
    }
}
