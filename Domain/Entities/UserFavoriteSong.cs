using Domain.Entities.Shared;

namespace Domain.Entities;

public class UserFavoriteSong : LinkEntity
{
    public Guid SongGuid { get; set; }
    public Guid UserGuid { get; set; }
    

    public UserFavoriteSong()
    {
    }

    public UserFavoriteSong(Guid songGuid, Guid userGuid)
    {
        SongGuid = songGuid;
        UserGuid = userGuid;
    }
}