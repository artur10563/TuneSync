using System.Linq.Expressions;
using Application.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories.Shared;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            return !(await _context.Users.AnyAsync(x => x.Email == email));
        }
        public async Task<User?> GetByExternalIdAsync(string uid)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.IdentityId == uid);
        }

        public override Task<User?> FirstOrDefaultWithDependantAsync(Expression<Func<User, bool>> predicate)
        {
            //TODO: Implement it if we will ever have user deletion :)
            throw new NotImplementedException();
        }
    }
}
