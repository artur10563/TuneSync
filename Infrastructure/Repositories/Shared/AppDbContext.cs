using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Shared
{
	public class AppDbContext : DbContext
	{
		public DbSet<Song> _songs { get; set; }
	}
}
