using System.Text.RegularExpressions;
using Domain.Entities.Shared;

namespace Domain.Entities
{
    public class Artist : EntityBase
    {
        //Name of channel
        public string Name { get; set; }

        //Display name, filtered from common words
        public string DisplayName { get; set; }

        //Id of channel
        public string YoutubeChannelId { get; set; }

        public virtual ICollection<Song> Songs { get; set; } = new HashSet<Song>();
        public virtual ICollection<Playlist> Playlists { get; set; } = new HashSet<Playlist>();

        public Artist(string name, string youtubeChannelId) : base()
        {
            Name = name;
            DisplayName = SanitizeTitle(name);
            YoutubeChannelId = youtubeChannelId;
        }

        private string SanitizeTitle(string channelTitle)
        {
            string pattern = @"\b(Official|VEVO|Channel|TV|Media|Music)\b";
            string result = Regex.Replace(channelTitle, pattern, "", RegexOptions.IgnoreCase);
            result = Regex.Replace(result, @"\s{2,}", " ").Trim();

            return result;
        }
    }
}