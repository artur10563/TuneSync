﻿using Domain.Entities;

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
        ILinkEntityRepository<UserFavoriteAlbum> UserFavoriteAlbumRepository { get; }
        ILinkEntityRepository<UserFavoritePlaylist> UserFavoritePlaylistRepository { get; }
        IArtistRepository ArtistRepository { get; }
        int SaveChanges();
        Task<int> SaveChangesAsync();
        /// <summary>
        /// Sets seed for DB.random. MUST BE CALLED IN TRANSACTION TO WORK
        /// </summary>
        void SetSeed(double seed);
        IQueryable<T> ApplyOrdering<T>(IQueryable<T> query, string orderBy, bool isDescending) where T : class;
        void TransactedAction(Action action);
        Task TransactedActionAsync(Func<Task> action);
    }
}
