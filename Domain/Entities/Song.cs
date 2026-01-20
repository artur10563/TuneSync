using System.Text.RegularExpressions;
using Domain.Entities.Shared;
using Domain.Enums;

namespace Domain.Entities
{
    public class Song : EntityBase
    {
        public string Title { get; set; }

        //File, Youtube, Deezer. Indicates where song entity was created from. Should never be changed after creation
        public string Source { get; set; }
        public string? SourceId { get; set; } // Youtube video id, deezer id and etc.
        public string? AudioSource { get; set; } // File, Youtube, Deezer. Indicates where audio was retrieved from.
        public Guid? AudioPath { get; set; }
        public TimeSpan AudioLength { get; set; } //seconds
        public int AudioSize { get; set; } //kb

        public Guid? CreatedBy { get; set; }
        public virtual User? User { get; set; }

        public Guid ArtistGuid { get; set; }
        public virtual Artist Artist { get; set; }

        public virtual ICollection<Playlist> Playlists { get; set; } = new HashSet<Playlist>();
        public virtual ICollection<UserSong> FavoredBy { get; set; } = new HashSet<UserSong>();

        public Guid? AlbumGuid { get; set; }
        public virtual Album? Album { get; set; }

        private Song()
        {
        }

        public Song(string title, string source, string? sourceId, Guid? audioPath, TimeSpan audioLength, int audioSize, Guid? createdBy,
            Guid artistGuid, Guid? albumGuid = null)
        {
            Title = SanitizeTitle(title);
            Source = source;
            SourceId = sourceId;
            AudioPath = audioPath;
            AudioSource = source;
            AudioLength = audioLength;
            AudioSize = audioSize;
            CreatedBy = createdBy;
            ArtistGuid = artistGuid;
            AlbumGuid = albumGuid;
        }

        public static Song CreateWithAudio(string title, string source, string? sourceId, Guid? audioPath, TimeSpan audioLength, int audioSize, Guid? createdBy,
            Guid artistGuid, Guid? albumGuid = null)
        {
            return new Song(title, source, sourceId, audioPath, audioLength, audioSize, createdBy, artistGuid, albumGuid);
        }
        public static Song CreateWithoutAudio(string title, string source, string? sourceId, Guid? createdBy,
            Guid artistGuid, Guid? albumGuid = null)
        {
            return new Song
            {
                Title = SanitizeTitle(title),
                Source = source,
                SourceId = sourceId,
                AudioPath = null,
                AudioLength = TimeSpan.Zero,
                AudioSize = 0,
                CreatedBy = createdBy,
                ArtistGuid = artistGuid,
                AlbumGuid = albumGuid,
                AudioSource = null
            };
        }

        private static string SanitizeTitle(string title, params string[] additionalFilters)
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

        public string? GetAudioPath()
        {
            if (AudioPath == null)
                return null;

            return StorageFolder.Audio.GetPath() + "/" + AudioPath;
        }
    }
}