namespace Application.DTOs.Songs
{
    public class SongDTO
    {
        public Guid Guid { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string VideoId { get; set; }
        public string AudioPath { get; set; }
        public int AudioSize { get; set; }
        public TimeSpan AudioLength { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsFavorite { get; set; }
    }
}
