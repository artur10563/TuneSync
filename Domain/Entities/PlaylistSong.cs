using Domain.Entities.Shared;

namespace Domain.Entities
{
    public class PlaylistSong : LinkEntity
    {
        public Guid PlaylistGuid { get; set; }
        public Guid SongGuid { get; set; }
    }
}
