namespace Application.DTOs.Songs
{
    public class SongDTO
    {
        public Guid Guid { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }

        public string Source { get; set; } //Youtube, deezer, upload from PC, etc
        public string SourceUrl { get; set; } //Link to YT video, etc
        public string ThumbnailUrl { get; set; }
        public string AudioPath { get; set; }
        public int AudioSize { get; set; }
        public TimeSpan AudioLength { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsFavorite { get; set; }
    }
}
