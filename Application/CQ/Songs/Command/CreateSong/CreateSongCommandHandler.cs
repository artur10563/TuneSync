using Application.Extensions;
using Application.Repositories.Shared;
using Application.Services;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
using FluentValidation;
using MediatR;
using System.IO;
using Domain.Enums;

namespace Application.CQ.Songs.Command.CreateSong
{
    internal class CreateSongCommandHandler : IRequestHandler<CreateSongCommand, Result<Song>>
    {
        private readonly IStorageService _fileStorage;
        private readonly IUnitOfWork _uow;
        private readonly IValidator<CreateSongCommand> _validator;

        public CreateSongCommandHandler(IStorageService fileStorage, IUnitOfWork uow, IValidator<CreateSongCommand> validator)
        {
            _fileStorage = fileStorage;
            _uow = uow;
            _validator = validator;
        }


        public async Task<Result<Song>> Handle(CreateSongCommand request, CancellationToken cancellationToken)
        {
            var validationResults = _validator.Validate(request);
            if (!validationResults.IsValid)
                return validationResults.AsErrors();

            var filePath = await _fileStorage.UploadFileAsync(request.AudioFileStream, StorageFolder.Audio);

            var song = new Song(title: request.Title,
                artistGuid: request.ArtistGuid,
                audioPath: new Guid(filePath),
                source: GlobalVariables.SongSource.File,
                sourceId: null,
                audioSize: (int)(request.AudioFileStream.Length/1000),
                createdBy: request.CreatedBy,
                audioLength: TimeSpan.Zero); //TODO: fix audioLength for files uploaded from pc

            _uow.SongRepository.Insert(song);
            await _uow.SaveChangesAsync();

            return Result.Success(song);
        }
    }
}
