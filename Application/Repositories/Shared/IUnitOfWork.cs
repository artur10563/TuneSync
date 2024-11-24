using Domain.Entities;

namespace Application.Repositories.Shared
{
    public interface IUnitOfWork
    {
        ISongRepository SongRepository { get; }
        IUserRepository UserRepository { get; }
        IPlaylistRepository PlaylistRepository { get; }
        ILinkEntityRepository<PlaylistSong> PlaylistSongRepository { get; }

        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
