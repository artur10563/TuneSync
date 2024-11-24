using Application.Extensions;
using Application.Repositories.Shared;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
using FluentValidation;
using MediatR;

namespace Application.CQ.Playlists.Command.Create
{
    internal class AddSongToPlaylistCommandHandler : IRequestHandler<AddSongToPlaylistCommand, Result<Guid>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IValidator<AddSongToPlaylistCommand> _validator;
        public AddSongToPlaylistCommandHandler(IUnitOfWork uow, IValidator<AddSongToPlaylistCommand> validator)
        {
            _uow = uow;
            _validator = validator;
        }

        public async Task<Result<Guid>> Handle(AddSongToPlaylistCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return validationResult.AsErrors();

            var playlist = await _uow.PlaylistRepository.GetByGuidAsync(request.PlaylistGuid, includes: p => p.Songs);
            var song = await _uow.SongRepository.GetByGuidAsync(request.SongGuid);

            if (playlist.Songs.Contains(song)) return new Error("Song allready in playlist!");

            playlist!.Songs.Add(song);

            await _uow.SaveChangesAsync();
            return playlist.Guid;
        }
    }
}
