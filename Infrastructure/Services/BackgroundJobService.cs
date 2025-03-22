using System.Linq.Expressions;
using System.Text.Json.Nodes;
using Application.Services;
using Hangfire;
using Hangfire.Storage;
using Hangfire.Storage.Monitoring;
using Newtonsoft.Json;

namespace Infrastructure.Services;

public class BackgroundJobService : IBackgroundJobService
{
    private readonly IBackgroundJobClient _backgroundClient;
    private const string _parseErrorMessage = "Can not serialize the return value";

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
        if (job.History?.FirstOrDefault()?.Data.TryGetValue("Result", out var value) != true || value == _parseErrorMessage)
            return null;


        var jsonNode = JsonNode.Parse(value);
        
        if (jsonNode == null) return value;

        var valueNode = jsonNode["Value"];
        var isSuccessNode = jsonNode["IsSuccess"];

        // If it's not a Result<T> - return whole jsonResult
        if (valueNode == null || isSuccessNode == null) return value;

        var isSuccess = isSuccessNode.GetValue<bool>();
        if (isSuccess)
        {
            return valueNode.ToString();
        }
        var errorsNode = jsonNode["Errors"];
        return errorsNode == null ? value : errorsNode.ToString().Replace("\r\n", "").Replace("\n", "");

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