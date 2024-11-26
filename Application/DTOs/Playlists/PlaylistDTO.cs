using Application.DTOs.Songs;

namespace Application.DTOs.Playlists
{
    public class PlaylistDTO
    {
        public Guid Guid { get; set; }
        public string Title { get; set; }
        public Guid CreatedBy { get; set; } 
        public string CreatedByName { get; set; } 
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public ICollection<SongDTO> Songs { get; set; } = [];
    }
}
