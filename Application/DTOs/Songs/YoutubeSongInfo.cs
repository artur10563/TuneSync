namespace Application.DTOs.Songs
{
	public record YoutubeSongInfo(string Id, string Title, SongAuthor Author, string Description = "", SongThumbnail? Thumbnail = null, string? PlaylistId = null);

	public record SongThumbnail(int Height, int Width, string Url);

	public record SongAuthor(string Id, string Title);
	
	public record ChannelInfo(string Id, string Title, string Url, SongThumbnail? Thumbnail = null);
	
	public record AlbumInfo(string Id, string Title, string AlbumUrl, SongThumbnail? Thumbnail = null);
	
}
