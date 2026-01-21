using Domain.Primitives;

namespace Application.BackgroundJobs;

public interface IFileCleanupJob : IBaseJob<Unit, Result<int>>;