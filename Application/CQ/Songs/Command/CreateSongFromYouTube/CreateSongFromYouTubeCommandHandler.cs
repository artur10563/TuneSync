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

            var newsong = new Song()
            {
                AudioPath = fileGuid,
                Artist = request.Author ?? videoInfo.Author.ChannelTitle,
                Title = request.SongName ?? videoInfo.Title,
                Source = GlobalVariables.SongSource.YouTube,
                SourceId = videoInfo.Id,
                AudioSize = (int)streamInfo.Size.KiloBytes,
                AudioLength = videoInfo.Duration!.Value
            };
            _uow.SongRepository.Insert(newsong);
            await _uow.SaveChangesAsync();

            return Result.Success(_mapper.Map<SongDTO>(newsong));
        }
    }
}
