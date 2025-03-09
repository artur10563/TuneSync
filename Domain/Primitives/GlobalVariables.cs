namespace Domain.Primitives
{
    /// <summary>
    /// Changing constants might require running a migration
    /// </summary>
    public static class GlobalVariables
    {
        private static string _fbStorage;
        private static string FirebaseMP3StringFormat = @"https://firebasestorage.googleapis.com/v0/b/{0}/o/{1}.mp3?alt=media";
        private static string YoutubeThumbnailStringFormat = @"https://img.youtube.com/vi/{0}/mqdefault.jpg";
        private static string YoutubeChannelStringFormat = @"https://www.youtube.com/channel/{0}";
        private static string YoutubeVideoStringFormat = @"https://www.youtube.com/watch?v={0}";
        private static string YoutubePlaylistThumbnailStringFormat = @"https://i.ytimg.com/vi/{0}/mqdefault.jpg";
        private static string YoutubeAlbumStringFormat = @"https://www.youtube.com/playlist?list={0}";
        public static void Initialize(string fbStorage)
        {
            _fbStorage = fbStorage;
        }

        public static string GetFirebaseMP3Link(Guid fileGuid) =>
            string.Format(FirebaseMP3StringFormat, _fbStorage, fileGuid);

        public static string GetYoutubeThumbnail(string videoGuid) =>
            string.Format(YoutubeThumbnailStringFormat, videoGuid);
        public static string GetYoutubePlaylistThumbnail(string thumbnailId) =>
            string.Format(YoutubePlaylistThumbnailStringFormat, thumbnailId);
        
        public static string GetYoutubeAlbumUrl(string albumId) =>
            string.Format(YoutubeAlbumStringFormat, albumId);

        public static string GetYoutubeChannel(string channelId) =>
            string.Format(YoutubeChannelStringFormat, channelId);
        
        public static string GetYoutubeVideo(string videoId) =>
            string.Format(YoutubeVideoStringFormat, videoId);
        
        public static class SongSource
        {
            public const string File = "File";
            public const string YouTube = "Youtube";
        }
        
        public static class PlaylistSource
        {
            public const string User = "User";
            public const string YouTube = "Youtube";
        }

        public static class UserConstants
        {
            public const int NameMinLength = 2;
            public const int NameMaxLength = 100;

            public const int EmailMaxLength = 255;

            public const int PasswordMinLength = 6;
            public const int PasswordMaxLength = 50;
        }

        public static class SongConstants
        {
            public const int TitleMinLength = 1;
            public const int TitleMaxLength = 100;

            public const int EmailMaxLength = 255;
            public const int MaxSizeKB = 1024 * 15; // 15 megabytes
        }

        public static class PlaylistConstants
        {
            public const int TitleMinLength = 1;
            public const int TitleMaxLength = 255;
        }

        public static class PaginationConstants
        {
            public const int PageMin = 1;
            public const int PageSize = 25;
            public const int PageSizeMax = 100;
        }

        public static class AlbumConstants
        {
            public const int MaxYoutubeAlbumLength = 30;
        }

        public static class ArtistConstants
        {
            public static readonly HashSet<string> ArtistSortColumns = ["Name", "DisplayName", "CreatedAt"];
        }

        public static class MixConstants
        {
            public const int MinCount = 2;
            public const int MaxCount = 50;
        }
    }
}
