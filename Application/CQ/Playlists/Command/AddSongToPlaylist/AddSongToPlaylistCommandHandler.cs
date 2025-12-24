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
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return validationResult.AsErrors();

            var playlist = await _uow.PlaylistRepository.GetByGuidAsync(request.PlaylistGuid, includes: p => p.PlaylistSongs);

            if (playlist.PlaylistSongs.Any(x => x.SongGuid == request.SongGuid)) return new Error("Song already in playlist!");

            playlist!.PlaylistSongs.Add(new PlaylistSong
            {
                PlaylistGuid = request.PlaylistGuid,
                SongGuid = request.SongGuid,
            });

            await _uow.SaveChangesAsync();
            return playlist.Guid;
        }
    }
}
