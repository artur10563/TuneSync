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
        
        public static async Task<User?> GetCurrentUserAsync(this HttpContext context)
        {
            var externalUserId = context.GetExternalUserId();
            if (string.IsNullOrEmpty(externalUserId)) return null;

            var unitOfWork = context.RequestServices.GetService<IUnitOfWork>();
            if (unitOfWork == null) throw new ArgumentNullException(nameof(IUnitOfWork));

            return await unitOfWork.UserRepository.GetByExternalIdAsync(externalUserId);
        }
    }
}
