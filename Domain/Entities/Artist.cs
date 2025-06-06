﻿using System.Text.RegularExpressions;
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

        public string? ThumbnailUrl { get; set; }
        public virtual ICollection<Song> Songs { get; set; } = new HashSet<Song>();
        public virtual ICollection<Album> Albums { get; set; } = new HashSet<Album>();

        #region Parent_Child

        public Guid? ParentId { get; set; }
        public virtual Artist? Parent { get; set; }
        
        public Guid? TopLvlParentId { get; set; }
        public virtual Artist? TopLvlParent { get; set; }
        
        /// <summary>
        /// All descendants by TopLvlParentId. Includes nested children
        /// </summary>
        public virtual ICollection<Artist> AllChildren { get; set; } = new HashSet<Artist>();
        
        /// <summary>
        /// Only descendants by parentId
        /// </summary>
        public virtual ICollection<Artist> Children { get; set; }

        #endregion
        
        public Artist(string name, string youtubeChannelId, string? thumbnailUrl = null) : base()
        {
            Name = name;
            DisplayName = SanitizeTitle(name);
            YoutubeChannelId = youtubeChannelId;
            ThumbnailUrl = thumbnailUrl;
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