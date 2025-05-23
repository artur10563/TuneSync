﻿using Application.Repositories.Shared;
using Domain.Entities;
using Domain.Errors;
using FluentValidation;

namespace Application.CQ.Playlists.Command.Create
{
    public sealed class AddSongToPlaylistCommandValidator : AbstractValidator<AddSongToPlaylistCommand>
    {
        public AddSongToPlaylistCommandValidator(IUnitOfWork uow)
        {
            RuleFor(x => x.SongGuid)
                .NotEmpty().WithMessage("Song is required.")
                .MustAsync(async (songGuid, cancellation) =>
                await uow.SongRepository.ExistsAsync(x => x.Guid == songGuid))
           .WithMessage(Error.NotFound(nameof(Song)).Description);

            RuleFor(x => x.PlaylistGuid)
                .NotEmpty().WithMessage("Playlist is required.")
                .MustAsync(async (playlistGuid, cancellation) =>
                    await uow.PlaylistRepository.ExistsAsync(x => x.Guid == playlistGuid))
                .WithMessage(Error.NotFound(nameof(Playlist)).Description);

            RuleFor(x => x)
                .MustAsync(async (command, cancellation) =>
                {
                    var playlist = await uow.PlaylistRepository.GetByGuidAsync(command.PlaylistGuid);
                    return playlist != null && playlist.CreatedBy == command.CurrentUserGuid;
                })
                .WithMessage(Error.AccessDenied.Description);
        }
    }
}
