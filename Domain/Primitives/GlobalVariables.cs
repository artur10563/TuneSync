namespace Domain.Primitives
{
    /// <summary>
    /// Changing constants might require running a migration
    /// </summary>
    public static class GlobalVariables
    {
        private static string _fbStorage;
        public static string FirebaseBaseUrl => $"https://firebasestorage.googleapis.com/v0/b/{_fbStorage}/o/";

        public static string FirebaseMediaFileFormat (string fileExtension)=> $"{FirebaseBaseUrl}{{0}}.{fileExtension}?alt=media";
        public static void Initialize(string fbStorage)
        {
            _fbStorage = fbStorage;
        }

        public static string GetFirebaseMP3Link(Guid fileGuid) => string.Format(FirebaseMediaFileFormat("mp3"), fileGuid );
        
        public static class SongSource
        {
            public const string File = "File";
            public const string YouTube = "Youtube";
        }
        
        public static class PlaylistSource
        {
            public const string User = "User";
            public const string YouTube = "Youtube";
            public const string YouTubeMusic = "YoutubeMusic";
        }

        public static class UserConstants
        {
            public static class Roles
            {
                public const string Admin = "Admin";
                public const string User = "User";
            }
            
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
