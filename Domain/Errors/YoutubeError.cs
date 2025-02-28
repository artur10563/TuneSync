using Domain.Primitives;

namespace Domain.Errors;

public class YoutubeError
{
    public static Error MaxYoutubeLengthError => new Error($"Maximum allowed length is {GlobalVariables.AlbumConstants.MaxYoutubeAlbumLength} songs");
}