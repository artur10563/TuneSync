using System.Linq.Expressions;
using Application.Services;
using Hangfire;
using Hangfire.Storage;
using Hangfire.Storage.Monitoring;
using Newtonsoft.Json;

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

    public string GetJobStatus(string jobId)
    {
        return JobStorage.Current.GetConnection().GetJobData(jobId)?.State ?? string.Empty;
    }

    public string? GetReturnedItems(string jobId)
    {
        var jobMonitoringApi = JobStorage.Current.GetMonitoringApi();
        var job = jobMonitoringApi.JobDetails(jobId);

        // Check if job has history and if history contains a result
        if (job.History == null || job.History.Count <= 0 || !job.History[0].Data.ContainsKey("Result"))
            return null;

        var resultSerialized = job.History[0].Data["Result"];
        return resultSerialized;
    }

    public bool IsRunning(string searchArg)
    {
        var isEnqueued = JobStorage.Current.GetMonitoringApi().EnqueuedJobs("default", 0, 1)
            .Any(job => job.Value.Job.Args.Contains(searchArg));

        var isProcessing = JobStorage.Current.GetMonitoringApi().ProcessingJobs(0, 100)
            .Any(job => job.Value.Job.Args.Contains(searchArg));

        return isEnqueued || isProcessing;
    }
}