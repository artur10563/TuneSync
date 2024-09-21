using Application.DTOs.Songs;
using Application.Repositories.Shared;
using Application.Services;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
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

		public CreateSongFromYouTubeCommandHandler(IYoutubeService youtube, IFirebaseStorageService storage, IUnitOfWork uow)
		{
			_youtube = youtube;
			_storage = storage;
			_uow = uow;
		}

		const int MaxSizeKB = 1024 * 15;

		public async Task<Result<SongDTO>> Handle(CreateSongFromYoutubeCommand request, CancellationToken cancellationToken)
		{
			string decodedUrl = HttpUtility.UrlDecode(request.url);
			var regex = new Regex("^(https?:\\/\\/)?(www\\.)?youtube\\.com\\/watch\\?v=([a-zA-Z0-9_-]{11})(&.*)?$");

			if (!regex.IsMatch(decodedUrl)) return new Error("Invalid link");

			var (videoInfo, streamInfo) = await _youtube.GetVideoInfoAsync(decodedUrl);

			if (streamInfo.Size.KiloBytes > MaxSizeKB)
				return SongError.InvalidSize;


			using var stream = await _youtube.GetAudioStreamAsync(streamInfo);
			var filePath = await _storage.UploadFileAsync(stream);

			//TODO: add max length validation
			//TODO: check is this song allready exists in database

			var newsong = new Song()
			{
				AudioPath = filePath,
				Artist = videoInfo.Author.ChannelTitle,
				Title = videoInfo.Title,
				VideoId = videoInfo.Id
			};
			_uow.SongRepository.Insert(newsong);
			await _uow.SaveChangesAsync();

			var dto = new SongDTO()
			{
				Guid = newsong.Guid,
				AudioPath = newsong.AudioPath,
				Artist = newsong.Artist,
				Title = newsong.Title,
				VideoId = newsong.VideoId
			};

			return Result.Success(dto);
		}
	}
}
