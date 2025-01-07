using Domain.Entities.Shared;

namespace Domain.Entities
{
    public class PlaylistSong : LinkEntity
    {
        public Guid PlaylistGuid { get; set; }
        public Guid SongGuid { get; set; }

        public virtual Playlist Playlist { get; set; }
        public virtual Song Song { get; set; }
        public PlaylistSong() { }
        public PlaylistSong(Guid playlistGuid, Guid songGuid)
        {
            PlaylistGuid = playlistGuid;
            SongGuid = songGuid;
        }
    }
}
