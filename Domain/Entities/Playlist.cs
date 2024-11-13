using Domain.Entities.Shared;

namespace Domain.Entities
{
    public class Playlist : EntityBase
    {
        public string Title { get; set; }
        public Guid CreatedBy { get; set; }

        public User User { get; set; }

        public Playlist() : base() { }
        public Playlist(string title, Guid createdBy) : base()
        {
            Title = title;
            CreatedBy = createdBy;
        }
    }
}
