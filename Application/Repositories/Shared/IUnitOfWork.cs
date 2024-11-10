namespace Application.Repositories.Shared
{
	public interface IUnitOfWork
	{
		ISongRepository SongRepository { get; }
		IUserRepository UserRepository { get; }

		int SaveChanges();
		Task<int> SaveChangesAsync();
	}
}
