using Domain.Entities.Shared;

namespace Domain.Entities
{
    public class Playlist : EntityBase
    {
        public string Title { get; set; }
        public Guid CreatedBy { get; set; }
        public string Source { get; set; }
        public User User { get; set; }
        public string? ThumbnailId { get; set; }
        public string? ThumbnailSource { get; set; } //YT link or blob

        // public virtual ICollection<Song> Songs { get; set; } = new HashSet<Song>();
        public virtual ICollection<UserFavoritePlaylist> FavoredBy { get; set; } = new HashSet<UserFavoritePlaylist>();

        public virtual ICollection<PlaylistSong> PlaylistSongs { get; set; } = new HashSet<PlaylistSong>();

        // public IEnumerable<Song> Songs => PlaylistSongs.OrderBy(ps => ps.CreatedAt).Select(ps => ps.Song);
        
        public Playlist(string title, Guid createdBy, string source,
            string? thumbnailSource = null, string? thumbnailId = null) : base()
        {
            Title = title;
            CreatedBy = createdBy;
            Source = source;
            ThumbnailSource = thumbnailSource;
            ThumbnailId = thumbnailId;
        }
    }
}