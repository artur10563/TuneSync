namespace Domain.Primitives
{
    /// <summary>
    /// Changing constants might reuire running a migration
    /// </summary>
    public static class GlobalVariables
    {
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
