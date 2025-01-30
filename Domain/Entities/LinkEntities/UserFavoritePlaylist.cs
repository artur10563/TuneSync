using Domain.Entities.Shared;

namespace Domain.Entities;

public class UserFavoritePlaylist : LinkEntity
{
    public Guid UserGuid { get; set; }
    public Guid PlaylistGuid { get; set; }

    public bool IsFavorite { get; set; } // in case if more fields will be added
    
    private UserFavoritePlaylist() { }

    public UserFavoritePlaylist(Guid userGuid, Guid playlistGuid, bool isFavorite)
    {
        UserGuid = userGuid;
        PlaylistGuid = playlistGuid;
        IsFavorite = isFavorite;
    }
}