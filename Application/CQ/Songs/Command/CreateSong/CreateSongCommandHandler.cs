using Application.Extensions;
using Application.Repositories.Shared;
using Application.Services;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
using FluentValidation;
using MediatR;
using System.IO;

namespace Application.CQ.Songs.Command.CreateSong
{
    internal class CreateSongCommandHandler : IRequestHandler<CreateSongCommand, Result<Song>>
    {
        private readonly IFirebaseStorageService _fileStorage;
        private readonly IUnitOfWork _uow;
        private readonly IValidator<CreateSongCommand> _validator;

        public CreateSongCommandHandler(IFirebaseStorageService fileStorage, IUnitOfWork uow, IValidator<CreateSongCommand> validator)
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

            var filePath = await _fileStorage.UploadFileAsync(request.audioFileStream);

            var newSong = new Song
            {
                Title = request.Title,
                ArtistGuid = request.artistGuid,
                AudioPath = filePath,
                Source = GlobalVariables.SongSource.File,
                SourceId = null
            };

            _uow.SongRepository.Insert(newSong);
            await _uow.SaveChangesAsync();

            return Result.Success(newSong);
        }
    }
}
