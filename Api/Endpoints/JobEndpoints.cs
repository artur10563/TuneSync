using Application.Services;
using Domain.Errors;
using Domain.Primitives;
using MediatR;

namespace Api.Endpoints;

public static class JobEndpoints
{
    public static IEndpointRouteBuilder RegisterJobEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/job/{jobId}",
                async (IBackgroundJobService _backgroundService, string jobId) =>
                {
                    var jobStatus = _backgroundService.GetJobStatus(jobId);
                    if (string.IsNullOrEmpty(jobStatus))
                    {
                        var failure = Result.Failure(Error.NotFound("Job"));
                        return Results.BadRequest(failure.Errors);
                    }

                    var serializedData = jobStatus == "Succeeded"
                        ? _backgroundService.GetReturnedItems(jobId)
                        : string.Empty;

                    return Results.Ok(new { jobStatus, serializedData });
                })
            .WithName("DownloadingProgress")
            .WithDescription("Returns job status and serialized data")
            .RequireAuthorization();


        return app;
    }
}