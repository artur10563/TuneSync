﻿using System.Collections.ObjectModel;
using Domain.Entities.Shared;

namespace Domain.Entities
{
    public class Playlist : EntityBase
    {
        public string Title { get; set; }
        public Guid CreatedBy { get; set; }
        public string Source { get; set; }
        public string? SourceId { get; set; }
        public User User { get; set; }
        public Guid? ArtistGuid { get; set; }
        public Artist? Artist { get; set; }

        public virtual ICollection<Song> Songs { get; set; } = new HashSet<Song>();
        public Playlist(string title, Guid createdBy, string source, string? sourceId = null, Guid? artistGuid = null) : base()
        {
            Title = title;
            CreatedBy = createdBy;
            Source = source;
            if (!string.IsNullOrEmpty(sourceId))
                SourceId = sourceId;
            if (artistGuid.HasValue)
                ArtistGuid = artistGuid;
        }
    }
}