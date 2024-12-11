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
        private readonly IStorageService _storage;
        private readonly IUnitOfWork _uow;
        private readonly IValidator<CreateSongFromYoutubeCommand> _validator;
        private readonly IMapper _mapper;

        public CreateSongFromYouTubeCommandHandler(
            IYoutubeService youtube,
            IStorageService storage,
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


        public async Task<Result<SongDTO>> Handle(CreateSongFromYoutubeCommand request,
            CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return validationResult.AsErrors();

            var (videoInfo, streamInfo) = await _youtube.GetVideoInfoAsync(request.Url);

            if (streamInfo.Size.KiloBytes > GlobalVariables.SongConstants.MaxSizeKB)
                return SongError.InvalidSize;


            using var stream = await _youtube.GetAudioStreamAsync(streamInfo);
            var fileGuid = await _storage.UploadFileAsync(stream);


            var artist =
                await _uow.ArtistRepository.FirstOrDefaultAsync(x =>
                    x.YoutubeChannelId == videoInfo.Author.ChannelId.Value)
                ?? new Artist(
                    name: videoInfo.Author.ChannelTitle,
                    youtubeChannelId: videoInfo.Author.ChannelId);

            if (artist.Guid == Guid.Empty)
                _uow.ArtistRepository.Insert(artist);

            var song = new Song(title: videoInfo.Title,
                source: GlobalVariables.SongSource.YouTube,
                artistGuid: artist.Guid,
                audioPath: fileGuid,
                sourceId: videoInfo.Id,
                audioLength: videoInfo.Duration!.Value,
                audioSize: (int)streamInfo.Size.KiloBytes,
                createdBy: request.CurrentUserGuid);
            
            _uow.SongRepository.Insert(song);
            await _uow.SaveChangesAsync();

            return Result.Success(_mapper.Map<SongDTO>(song));
        }
    }
}