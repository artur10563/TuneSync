using Domain.Entities.Shared;

namespace Domain.Entities
{
    public class Song : EntityBase
	{
		public string Title { get; set; }
		public string Artist { get; set; }
		public string Source { get; set; } //File, Youtube, Deezer etc.
		public string? SourceId { get; set; } // Youtube video id, deezer id and etc.
		public Guid AudioPath { get; set; }
		public TimeSpan AudioLength { get; set; } //seconds
		public int AudioSize { get; set; } //kb

		public Guid? CreatedBy { get; set; }
		public virtual User? User { get; set; }

		public virtual ICollection<Playlist> Playlists { get; set; }
	}
}