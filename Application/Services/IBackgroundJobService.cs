using System.Linq.Expressions;

namespace Application.Services;

public interface IBackgroundJobService
{
    /// <returns>Job id</returns>
    string Enqueue(Expression<Action> methodCall);

    /// <returns>Job id</returns>
    string Enqueue<T>(Expression<Action<T>> methodCall);

    string GetJobStatus(string jobId);
    /// <returns>True if job is currently running</returns>
    public bool IsRunning(string searchArg);

    string? GetReturnedItems(string jobId);
}