using Application.DTOs.Songs;
using Application.Extensions;
using Application.Repositories.Shared;
using Application.Services;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
using FluentValidation;
using MediatR;
using System;
using System.Text.RegularExpressions;
using System.Web;

namespace Application.CQ.Songs.Command.CreateSongFromYouTube
{
    internal class CreateSongFromYouTubeCommandHandler : IRequestHandler<CreateSongFromYoutubeCommand, Result<SongDTO>>
    {
        private readonly IYoutubeService _youtube;
        private readonly IFirebaseStorageService _storage;
        private readonly IUnitOfWork _uow;
        private readonly IValidator<CreateSongFromYoutubeCommand> _validator;

        public CreateSongFromYouTubeCommandHandler(
            IYoutubeService youtube,
            IFirebaseStorageService storage,
            IUnitOfWork uow,
            IValidator<CreateSongFromYoutubeCommand> validator)
        {
            _youtube = youtube;
            _storage = storage;
            _uow = uow;
            _validator = validator;
        }


        public async Task<Result<SongDTO>> Handle(CreateSongFromYoutubeCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return validationResult.AsErrors();

            var (videoInfo, streamInfo) = await _youtube.GetVideoInfoAsync(request.url);

            if (streamInfo.Size.KiloBytes > GlobalVariables.SongConstants.MaxSizeKB)
                return SongError.InvalidSize;


            using var stream = await _youtube.GetAudioStreamAsync(streamInfo);
            var filePath = await _storage.UploadFileAsync(stream);


            var newsong = new Song()
            {
                AudioPath = filePath,
                Artist = videoInfo.Author.ChannelTitle,
                Title = videoInfo.Title,
                Source = GlobalVariables.SongSource.YouTube,
                SourceId = videoInfo.Id,
                AudioSize = (int)streamInfo.Size.KiloBytes,
                AudioLength = videoInfo.Duration!.Value
            };
            _uow.SongRepository.Insert(newsong);
            await _uow.SaveChangesAsync();

            //Replace with automaper
            var dto = new SongDTO()
            {
                Guid = newsong.Guid,
                AudioPath = newsong.AudioPath,
                Artist = newsong.Artist,
                Title = newsong.Title,
                VideoId = newsong.SourceId,
                AudioSize = newsong.AudioSize,
                AudioLength = newsong.AudioLength
            };

            return Result.Success(dto);
        }
    }
}
