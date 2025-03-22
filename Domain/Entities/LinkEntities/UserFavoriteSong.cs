using Domain.Entities.Shared;

namespace Domain.Entities;

public class UserSong : LinkEntity
{
    public Guid SongGuid { get; set; }
    public Guid UserGuid { get; set; }
    public bool IsFavorite { get; set; }
    public bool IsOffline { get; set; }

    public UserSong()
    {
    }

    public UserSong(Guid songGuid, Guid userGuid, bool isFavorite, bool isOffline)
    {
        SongGuid = songGuid;
        UserGuid = userGuid;
        IsFavorite = isFavorite;
        IsOffline = isOffline;
    }
}