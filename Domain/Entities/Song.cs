using System.Text.RegularExpressions;
using Domain.Entities.Shared;

namespace Domain.Entities
{
    public class Song : EntityBase
	{
		public string Title { get; set; }
		public string Source { get; set; } //File, Youtube, Deezer etc.
		public string? SourceId { get; set; } // Youtube video id, deezer id and etc.
		public Guid AudioPath { get; set; }
		public TimeSpan AudioLength { get; set; } //seconds
		public int AudioSize { get; set; } //kb

		public Guid? CreatedBy { get; set; }
		public virtual User? User { get; set; }

		public Guid ArtistGuid { get; set; }
		public virtual Artist Artist { get; set;}

		public virtual ICollection<Playlist> Playlists { get; set; }
		
		public Song(string title, string source, string? sourceId, Guid audioPath, TimeSpan audioLength, int audioSize, Guid? createdBy, Guid artistGuid)
		{
			Title = SanitizeTitle(title);
			Source = source;
			SourceId = sourceId;
			AudioPath = audioPath;
			AudioLength = audioLength;
			AudioSize = audioSize;
			CreatedBy = createdBy;
			ArtistGuid = artistGuid;
		}

		private string SanitizeTitle(string title, params string[] additionalFilters)
		{
			string pattern = @"(\[.*?\]|\(.*?\))";
			string result = Regex.Replace(title, pattern, "", RegexOptions.IgnoreCase);
			
			if (additionalFilters != null)
			{
				foreach (var filter in additionalFilters)
				{
					result = Regex.Replace(result, Regex.Escape(filter), "", RegexOptions.IgnoreCase);
				}
			}
			//Normalize spaces and dashes
			result = Regex.Replace(result, @"\s{2,}", " ").Trim();
			result = Regex.Replace(result, @"\s*-\s*", "-").Trim('-');

			return result;
		}
	}
}