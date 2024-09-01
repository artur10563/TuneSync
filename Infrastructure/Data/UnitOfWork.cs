using Application.Repositories;
using Application.Repositories.Shared;

namespace Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public ISongRepository SongRepository { get; set; }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }



        public UnitOfWork(AppDbContext context, ISongRepository songRepository)
        {
            _context = context;
            SongRepository = songRepository ?? throw new ArgumentNullException(nameof(songRepository));
        }
    }
}
