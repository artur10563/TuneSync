using Domain.Primitives;

namespace Domain.Helpers;

public static class YoutubeHelper
{
    private static string YoutubeThumbnailStringFormat = @"https://img.youtube.com/vi/{0}/mqdefault.jpg";
    private static string YoutubeChannelStringFormat = @"https://www.youtube.com/channel/{0}";
    private static string YoutubeVideoStringFormat = @"https://www.youtube.com/watch?v={0}";

    private static string YoutubePlaylistThumbnailStringFormat = @"https://i.ytimg.com/vi/{0}/mqdefault.jpg";
  
    private static string YoutubeAlbumStringFormat = @"https://www.youtube.com/playlist?list={0}";

    public static string GetYoutubeThumbnail(string videoGuid) =>
        string.Format(YoutubeThumbnailStringFormat, videoGuid);

    public static string GetYoutubePlaylistThumbnail(string thumbnailId, string playlistId)
    {
        return string.Format(IsYoutubeMusic(playlistId)
            ? string.Format(GlobalVariables.FirebaseMediaFileFormat("jpg"), thumbnailId.Replace("/", "%2F"))
            : YoutubePlaylistThumbnailStringFormat, thumbnailId);
    }

    public static string GetYoutubeAlbumUrl(string albumId) =>
        string.Format(YoutubeAlbumStringFormat, albumId);

    public static string GetYoutubeChannel(string channelId) =>
        string.Format(YoutubeChannelStringFormat, channelId);

    public static string GetYoutubeVideo(string videoId) =>
        string.Format(YoutubeVideoStringFormat, videoId);


    public static string YTMIdentifier = "OLAK";

    public static bool IsYoutubeMusic(string playlistId) => playlistId.StartsWith(YTMIdentifier, StringComparison.CurrentCultureIgnoreCase);

    public static string ExtractVideoIdFromUrl(string url)
    {
        var startIndex = url.IndexOf("v=");
        if (startIndex == -1) throw new ArgumentException("Invalid YouTube URL or 'v' parameter not found", nameof(url));

        // Extract the substring starting after "v=" and ending at the next '&' or end of string
        startIndex += 2; // Move past "v="

        int endIndex = url.IndexOf('&', startIndex);
        if (endIndex == -1)
        {
            endIndex = url.Length; // No '&' found, take the rest of the string
        }

        return url.Substring(startIndex, endIndex - startIndex);
    }

    public static string GetPlaylistThumbnailIdFromUrl(string? thumbnailUrl, string playlistId)
    {
        //https://i.ytimg.com/vi/{thumbnailId}/default.jpg (Youtube)
        //https://i9.ytimg.com/s_p/{albumId}/{resolution_format}.jpg?{identifier} (YoutubeMusic)
        //YTM has different identifiers for different formats, so all url params are saved

        var ytMarker = "/vi/";
        var ytmMarker = "/s_p/";


        var id = "";
        if (string.IsNullOrEmpty(thumbnailUrl)) return id;

        if (YoutubeHelper.IsYoutubeMusic(playlistId))
        {
            var index = thumbnailUrl.IndexOf(ytmMarker, StringComparison.Ordinal);
            id = thumbnailUrl.Substring(index + ytmMarker.Length);
        }
        else
        {
            var index = thumbnailUrl.IndexOf(ytMarker, StringComparison.Ordinal);
            var endIndex = thumbnailUrl.IndexOf('/', index + ytMarker.Length);
            id = thumbnailUrl.Substring(index + ytMarker.Length, endIndex - index);
        }

        return id;
    }
}