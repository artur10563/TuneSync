using Domain.Entities.Shared;

namespace Domain.Entities
{
    public class User : EntityBase
    {
        public string Name { get; set; }
        public string Email { get; set; }

        //Used to identify user from external provider
        public string IdentityId { get; set; }

        public virtual ICollection<Playlist> Playlists { get; set; }

        public User(string name, string email, string identityId)
        {
            Name = name;
            Email = email;
            IdentityId = identityId;
        }
    }
}
