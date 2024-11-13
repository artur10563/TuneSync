using Domain.Primitives;

namespace Domain.Errors
{
    public static class PlaylistError
    {
        public static Error InvalidTitleLength => new Error($"Playlist title length must be {GlobalVariables.PlaylistConstants.TitleMinLength}-{GlobalVariables.PlaylistConstants.TitleMaxLength} symbols");
        public static Error OwnerRequired => new Error($"Playlist cant be created without owner");
    }
}
