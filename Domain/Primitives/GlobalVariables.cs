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
        public static void Initialize(string fbStorage)
        {
            _fbStorage = fbStorage;
        }

        public static string GetFirebaseMP3Link(Guid fileGuid) =>
            string.Format(FirebaseMP3StringFormat, _fbStorage, fileGuid);

        public static string GetYoutubeThumbnail(string videoGuid) =>
            string.Format(YoutubeThumbnailStringFormat, videoGuid);

        public static string GetYoutubeChannel(string channelId) =>
            string.Format(YoutubeChannelStringFormat, channelId);


        public static string YoutubeWatch = @"https://www.youtube.com/watch?v=";
        public static class SongSource
        {
            public const string File = "File";
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
    }
}
