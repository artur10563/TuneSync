namespace Application.Helpers;

public static class YoutubeHelpers
{
    public static string ExtractVideoIdFromUrl(string url)
    {
        int startIndex = url.IndexOf("v=");
        if (startIndex != -1)
        {
            // Extract the substring starting after "v=" and ending at the next '&' or end of string
            startIndex += 2; // Move past "v="

            int endIndex = url.IndexOf('&', startIndex);
            if (endIndex == -1)
            {
                endIndex = url.Length; // No '&' found, take the rest of the string
            }

            return url.Substring(startIndex, endIndex - startIndex);
        }

        throw new ArgumentException("Invalid YouTube URL or 'v' parameter not found", nameof(url));
    }
}