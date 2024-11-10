using Domain.Entities.Shared;

namespace Domain.Entities
{
	public class Song : EntityBase
	{
		public string Title { get; set; }
		public string Artist { get; set; }
		public string Source { get; set; } //File, Youtube, Deezer etc.
		public string? SourceId { get; set; } // Youtube video id, deezer id and etc.
		public string AudioPath { get; set; }
		public TimeSpan AudioLength { get; set; } //seconds
		public int AudioSize { get; set; } //kb
	}
}


/*
	Song can be uploaded from PC. User should be able to select artist from dropdown or create new artist


	Sources:
	-youtube
	-pc

	-port from deezer?

	DatabaseStoredSong:
	-Title
	-Author
	-StorageAudioLink
	-VideoId (for now youtube video ID)
	-length
	-file size

	YoutubeSong:
	Is not stored in database, used as a way to create a new DatabaseStoredSong

	Author:
	Stored in database. 
	PROBLEM: youtube can create many junk authors.
	FIX IDEA: apply search to author, let user select from dropdown or ranked authors. Check milan jovanovic search.

 */