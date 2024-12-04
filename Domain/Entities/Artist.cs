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

        public virtual ICollection<Song> Songs { get; set; }


        public Artist(string name, string displayName, string youtubeChannelId) : base()
        {
            Name = name;
            DisplayName = displayName;
            YoutubeChannelId = youtubeChannelId;
        }
    }
}
