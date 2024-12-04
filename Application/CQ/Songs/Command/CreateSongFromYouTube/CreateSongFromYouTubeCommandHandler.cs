using Application.DTOs.Songs;
using Application.Extensions;
using Application.Repositories.Shared;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
using FluentValidation;
using MediatR;
using System.Text.RegularExpressions;

namespace Application.CQ.Songs.Command.CreateSongFromYouTube
{
    internal class CreateSongFromYouTubeCommandHandler : IRequestHandler<CreateSongFromYoutubeCommand, Result<SongDTO>>
    {
        private readonly IYoutubeService _youtube;
        private readonly IFirebaseStorageService _storage;
        private readonly IUnitOfWork _uow;
        private readonly IValidator<CreateSongFromYoutubeCommand> _validator;
        private readonly IMapper _mapper;

        public CreateSongFromYouTubeCommandHandler(
            IYoutubeService youtube,
            IFirebaseStorageService storage,
            IUnitOfWork uow,
            IValidator<CreateSongFromYoutubeCommand> validator,
            IMapper mapper
            )
        {
            _youtube = youtube;
            _storage = storage;
            _uow = uow;
            _validator = validator;
            _mapper = mapper;
        }


        public async Task<Result<SongDTO>> Handle(CreateSongFromYoutubeCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return validationResult.AsErrors();

            var (videoInfo, streamInfo) = await _youtube.GetVideoInfoAsync(request.Url);

            if (streamInfo.Size.KiloBytes > GlobalVariables.SongConstants.MaxSizeKB)
                return SongError.InvalidSize;


            using var stream = await _youtube.GetAudioStreamAsync(streamInfo);
            var fileGuid = await _storage.UploadFileAsync(stream);


            var artist = await _uow.ArtistRepository.FirstOrDefaultAsync(x => x.YoutubeChannelId == videoInfo.Author.ChannelId.Value)
                ?? new Artist(
                    name: videoInfo.Author.ChannelTitle,
                    displayName: SanitizeChannelTitle(videoInfo.Author.ChannelTitle),
                    youtubeChannelId: videoInfo.Author.ChannelId);

            if (artist.Guid == Guid.Empty) 
                _uow.ArtistRepository.Insert(artist);

            var newsong = new Song()
            {
                AudioPath = fileGuid,
                Artist = artist,
                Title = SanitizeVideoTitle(videoInfo.Title, artist.Name, artist.DisplayName),
                Source = GlobalVariables.SongSource.YouTube,
                SourceId = videoInfo.Id,
                AudioSize = (int)streamInfo.Size.KiloBytes,
                AudioLength = videoInfo.Duration!.Value,
                CreatedBy = request.CurrentUserGuid
            };
            _uow.SongRepository.Insert(newsong);
            await _uow.SaveChangesAsync();

            return Result.Success(_mapper.Map<SongDTO>(newsong));
        }

        private string SanitizeChannelTitle(string channelTitle)
        {
            string pattern = @"\b(Official|VEVO|Channel|TV|Media|Music)\b";
            string result = Regex.Replace(channelTitle, pattern, "", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, @"\s{2,}", " ").Trim();

            return result;
        }

        private string SanitizeVideoTitle(string videoTitle, params string[] additionalFilters)
        {
            //Remove patterns like "(...)", "[...]"
            string pattern = @"(\[.*?\]|\(.*?\))";
            string result = Regex.Replace(videoTitle, pattern, "", RegexOptions.IgnoreCase);

            //Remove additional filters provided, for example - name of channel from title
            if (additionalFilters != null)
            {
                foreach (var filter in additionalFilters)
                {
                    result = Regex.Replace(result, Regex.Escape(filter), "", RegexOptions.IgnoreCase);
                }
            }

            //Normalize spaces and dashes
            result = Regex.Replace(result, @"\s{2,}", " ").Trim();
            result = Regex.Replace(result, @"\s*-\s*", "-").Trim('-');

            return result;
        }
    }
}
