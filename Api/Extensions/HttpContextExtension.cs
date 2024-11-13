using Application.Repositories.Shared;
using Domain.Entities;

namespace Api.Extensions
{
    public static class HttpContextExtension
    {
        public static string GetExternalUserId(this HttpContext context)
        {
            return context?.User?.FindFirst("user_id")?.Value
                ?? String.Empty;
        }
    }
}
