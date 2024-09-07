using Application.Repositories.Shared;
using Application.Services;
using Domain.Entities;
using Domain.Errors;
using Domain.Primitives;
using MediatR;
using System.IO;

namespace Application.CQ.Songs.Command.CreateSong
{
	internal class CreateSongCommandHandler : IRequestHandler<CreateSongCommand, Result<Song>>
	{
		private readonly IFirebaseStorageService _fileStorage;
		private readonly IUnitOfWork _uow;

		public CreateSongCommandHandler(IFirebaseStorageService fileStorage, IUnitOfWork uow)
		{
			_fileStorage = fileStorage;
			_uow = uow;
		}


		public async Task<Result<Song>> Handle(CreateSongCommand request, CancellationToken cancellationToken)
		{
			var filePath = await _fileStorage.UploadFile(request.audioFileStream);

			var newSong = new Song
			{
				Title = request.Title,
				Artist = "UNKNOWN",
				AudioPath = filePath
			};

			_uow.SongRepository.Create(newSong);
			await _uow.SaveChangesAsync();

			return Result.Success(newSong);
		}
	}
}
