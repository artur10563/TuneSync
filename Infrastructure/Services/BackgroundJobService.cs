using System.Linq.Expressions;
using Application.Services;
using Hangfire;
using Hangfire.Storage;

namespace Infrastructure.Services;

public class BackgroundJobService : IBackgroundJobService
{
    private readonly IBackgroundJobClient _backgroundClient;

    public BackgroundJobService(IBackgroundJobClient backgroundJobClient)
    {
        _backgroundClient = backgroundJobClient;
    }

    public string Enqueue(Expression<Action> methodCall)
    {
        return _backgroundClient.Enqueue(methodCall);
    }

    public string Enqueue<T>(Expression<Action<T>> methodCall)
    {
        return _backgroundClient.Enqueue<T>(methodCall);
    }

    public string GetJobDetails(string jobId)
    {
        return JobStorage.Current.GetConnection().GetJobData(jobId).State;
    }

    public bool IsRunning(string searchArg)
    {
        var isEnqueued = JobStorage.Current.GetMonitoringApi().EnqueuedJobs("default", 0, 1)
            .Any(job=> job.Value.Job.Args.Contains(searchArg));
        
        var isProcessing = JobStorage.Current.GetMonitoringApi().ProcessingJobs(0, 100)
            .Any(job=> job.Value.Job.Args.Contains(searchArg));
        
        return isEnqueued || isProcessing;
    }
}