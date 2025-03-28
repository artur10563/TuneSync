namespace Application.DTOs.Youtube;

public class YouTubeVideoInfo
{
    public string VideoId { get; set; }
    public string Title { get; set; }
    public TimeSpan Duration { get; set; }
    public Author Author { get; set; }
}

public class Author
{
    public string ChannelId { get; set; }
    public string ChannelTitle { get; set; }
    public string? ChannelThumbnail { get; set; }
}