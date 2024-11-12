using Application.Repositories.Shared;
using Domain.Entities;

namespace Application.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<bool> IsEmailUniqueAsync(string email);
    }
}
