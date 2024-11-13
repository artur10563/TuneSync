using System.Web;

namespace Api.Extensions
{
    public static class DecodingExtension
    {
        public static string DecodeUrl(this string encoded) => HttpUtility.UrlDecode(encoded);
    }
}
