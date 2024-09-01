namespace Application.Repositories.Shared
{
	public interface IUnitOfWork
	{
		ISongRepository SongRepository { get; }

		int SaveChanges();
		Task<int> SaveChangesAsync();
	}
}
