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
        ILinkEntityRepository<UserFavoriteSong> UserFavoriteSongRepository { get; }
        IArtistRepository ArtistRepository { get; }
        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
