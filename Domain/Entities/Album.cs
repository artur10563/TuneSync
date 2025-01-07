using Domain.Entities.Shared;

namespace Domain.Entities;

public class Album : EntityBase
{
    public string Title { get; set; }
    public Guid CreatedBy { get; set; }
    public string SourceId { get; set; } //only YT for now
    public User User { get; set; }
    public Guid? ArtistGuid { get; set; }
    public Artist? Artist { get; set; }
    public string? ThumbnailId { get; set; }
    public string? ThumbnailSource { get; set; } //YT link or blob

    //Song can have only one album. Album can have many songs
    public virtual ICollection<Song> Songs { get; set; } = new HashSet<Song>();

    public Album()
    {
    }

    public Album(string title, Guid createdBy, string sourceId, Guid? artistGuid = null, string? thumbnailId = null,
        string? thumbnailSource = null)
    {
        Title = title;
        CreatedBy = createdBy;
        SourceId = sourceId;
        ArtistGuid = artistGuid;
        ThumbnailId = thumbnailId;
        ThumbnailSource = thumbnailSource;
    }
}