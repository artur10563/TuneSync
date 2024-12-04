using Domain.Primitives;

namespace Domain.Errors
{
    public static class SongError
    {
        public static Error InvalidSize => new Error($"Max file size is {GlobalVariables.SongConstants.MaxSizeKB / 1024} megabytes");
        public static Error Exists => new Error($"Song allready exists");
        public static Error InvalidTitleLength => new Error($"Title length must be {GlobalVariables.SongConstants.TitleMinLength}-{GlobalVariables.SongConstants.TitleMaxLength} symbols");
    }
}
