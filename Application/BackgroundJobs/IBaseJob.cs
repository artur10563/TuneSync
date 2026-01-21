namespace Application.BackgroundJobs;

public interface IBaseJob;
public interface IBaseJob<in TIn, TOut> : IBaseJob
{
    Task<TOut> ExecuteAsync(TIn input, CancellationToken cancellationToken);
}

public readonly struct Unit
{
    public static readonly Unit Value = new();
}