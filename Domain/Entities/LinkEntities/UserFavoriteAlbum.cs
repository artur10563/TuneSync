using Domain.Entities.Shared;

namespace Domain.Entities;

public class UserFavoriteAlbum : LinkEntity
{
    public Guid UserGuid { get; set; }
    public Guid AlbumGuid { get; set; }

    public bool IsFavorite { get; set; } // in case if more fields will be added
    
    private UserFavoriteAlbum() { }

    public UserFavoriteAlbum(Guid userGuid, Guid albumGuid, bool isFavorite)
    {
        UserGuid = userGuid;
        AlbumGuid = albumGuid;
        IsFavorite = isFavorite;
    }
}