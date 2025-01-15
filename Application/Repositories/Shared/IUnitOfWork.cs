using Domain.Entities;

namespace Application.Repositories.Shared
{
    public interface IUnitOfWork
    {
        ISongRepository SongRepository { get; }
        IUserRepository UserRepository { get; }
        IPlaylistRepository PlaylistRepository { get; }
        IAlbumRepository AlbumRepository { get; }
        ILinkEntityRepository<PlaylistSong> PlaylistSongRepository { get; }
        ILinkEntityRepository<UserSong> UserSongRepository { get; }
        IArtistRepository ArtistRepository { get; }
        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
