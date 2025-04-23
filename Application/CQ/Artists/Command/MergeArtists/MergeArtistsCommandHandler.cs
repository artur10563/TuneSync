using Application.Repositories.Shared;
using Application.Services;
using Domain.Errors;
using Domain.Primitives;
using MediatR;

namespace Application.CQ.Artists.Command.MergeArtists;

public class MergeArtistsCommandHandler : IRequestHandler<MergeArtistsCommand, Result>
{
    private readonly IUnitOfWork _uow;
    private readonly ILoggerService _logger;

    public MergeArtistsCommandHandler(IUnitOfWork uow, ILoggerService logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<Result> Handle(MergeArtistsCommand request, CancellationToken cancellationToken)
    {
        if (request.ParentId == request.ChildId) return ArtistError.SelfMergeAttempt;

        var parent = await _uow.ArtistRepository.FirstOrDefaultAsync(x => x.Guid == request.ParentId);
        var child = await _uow.ArtistRepository.FirstOrDefaultAsync(x => x.Guid == request.ChildId,
            includes: x => x.AllChildren);

        if (parent == null || child == null) return ArtistError.InvalidMergePair;
        if (child.Guid == parent.TopLvlParentId) return ArtistError.CircularDependency;

        try
        {
            await _uow.TransactedActionAsync(async () =>
            {
                child.ParentId = parent.Guid;
                child.TopLvlParentId = parent.TopLvlParentId ?? parent.Guid;

                if (child.AllChildren.Count != 0)
                {
                    foreach (var subChildren in child.AllChildren)
                    {
                        subChildren.TopLvlParentId = child.TopLvlParentId;
                    }

                    _uow.ArtistRepository.UpdateRange(child.AllChildren);
                }

                await _uow.SaveChangesAsync();

                _logger.Log("Created Child-Parent pair", LogLevel.Information,
                    new { parentId = parent.Guid, childId = child.Guid, parentName = parent.Name, childName = child.Name });

                if (child.AllChildren.Count != 0)
                    _logger.Log($"Updated {child.AllChildren.Count} sub children", LogLevel.Information);
            });
        }
        catch (Exception e)
        {
            _logger.Log(e.Message, LogLevel.Critical);
            return new Error(e.Message);
        }

        return Result.Success();
    }
}