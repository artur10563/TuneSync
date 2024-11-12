using Domain.Primitives;

namespace Domain.Errors
{
    public static class SongError
    {
        public static Error InvalidSize => new Error($"Max file size is {GlobalVariables.SongConstants.MaxSizeKB / 1024} megabytes");
    }
}
