using Domain.Entities.Shared;
using Domain.Primitives;

namespace Domain.Entities
{
    public class User : EntityBase
    {
        public string Name { get; set; }
        public string Email { get; set; }

        //Used to identify user from external provider
        public string IdentityId { get; set; }

        public virtual ICollection<Playlist> Playlists { get; set; } = new HashSet<Playlist>();
        public virtual ICollection<Album> Albums { get; set; } = new HashSet<Album>();
        public virtual ICollection<Song> Songs { get; set; } = new HashSet<Song>();
        public virtual ICollection<UserSong> FavoriteSongs { get; set; } = new HashSet<UserSong>();
        public virtual ICollection<UserFavoriteAlbum> FavoriteAlbums { get; set; } = new HashSet<UserFavoriteAlbum>();
        public virtual ICollection<UserFavoritePlaylist> FavoritePlaylists { get; set; } = new HashSet<UserFavoritePlaylist>();

        public string Role { get; set; } // Only User or Admin for now; TODO: Move to separate table to allow multiple roles
        
        public User(string name, string email, string identityId)
        {
            Name = name;
            Email = email;
            IdentityId = identityId;
            Role = GlobalVariables.UserConstants.Roles.User;
        }
    }
}
