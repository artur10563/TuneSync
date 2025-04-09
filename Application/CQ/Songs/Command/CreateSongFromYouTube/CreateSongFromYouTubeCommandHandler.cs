using Application.DTOs.Songs;
using Application.Extensions;
using Application.Repositories.Shared;
using Application.Services;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
using FluentValidation;
using MediatR;
using System.Text.RegularExpressions;

namespace Application.CQ.Songs.Command.CreateSongFromYouTube
{
    internal class CreateSongFromYouTubeCommandHandler : IRequestHandler<CreateSongFromYoutubeCommand, Result<Guid>>
    {
        private readonly IYoutubeService _youtube;
        private readonly IStorageService _storage;
        private readonly IUnitOfWork _uow;
        private readonly IValidator<CreateSongFromYoutubeCommand> _validator;

        public CreateSongFromYouTubeCommandHandler(
            IYoutubeService youtube,
            IStorageService storage,
            IUnitOfWork uow,
            IValidator<CreateSongFromYoutubeCommand> validator
        )
        {
            _youtube = youtube;
            _storage = storage;
            _uow = uow;
            _validator = validator;
        }


        public async Task<Result<Guid>> Handle(CreateSongFromYoutubeCommand request,
            CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return validationResult.AsErrors();

            // var (videoInfo, streamInfo) = await _youtube.GetVideoInfoAsync(request.Url);
            
            var videoInfo = await _youtube.GetVideoInfoAsyncDLP(request.Url);
            await using var stream = await _youtube.GetAudioStreamAsyncDLP(videoInfo.VideoId);
            
            if (stream.GetKilobytes() > GlobalVariables.SongConstants.MaxSizeKB)
                return SongError.InvalidSize;


            var artist = await _uow.ArtistRepository.FirstOrDefaultAsync(x =>
                x.YoutubeChannelId == videoInfo.Author.ChannelId);
            if (artist == null)
            {
                var artistInfo = await _youtube.GetChannelInfoAsync(videoInfo.Author.ChannelId); // double check this shit. 
                
                artist = new Artist(
                    name: videoInfo.Author.ChannelTitle,
                    youtubeChannelId: videoInfo.Author.ChannelId,
                    thumbnailUrl: artistInfo?.Thumbnail?.Url
                    );
                _uow.ArtistRepository.Insert(artist);
            }
            
            var fileGuid = await _storage.UploadFileAsync(stream);

            var song = new Song(title: videoInfo.Title,
                source: GlobalVariables.SongSource.YouTube,
                artistGuid: artist.Guid,
                audioPath: new Guid(fileGuid),
                sourceId: videoInfo.VideoId,
                audioLength: videoInfo.Duration,
                audioSize: (int)stream.GetKilobytes(),
                createdBy: request.CurrentUserGuid);

            _uow.SongRepository.Insert(song);
            await _uow.SaveChangesAsync();

            return Result.Success(song.Guid);
        }
    }
}