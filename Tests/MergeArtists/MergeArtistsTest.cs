using Application.CQ.Artists.Command.MergeArtists;
using Domain.Entities;
using Domain.Errors;

namespace Tests.MergeArtists;

public class MergeArtistsTest : BaseTest
{
    [Fact]
    public async Task Handle_ShouldReturnSelfMergeAttempt_WhenParentIdEqualsChildId()
    {
        var guid = Guid.NewGuid();
        var command = new MergeArtistsCommand(guid, guid);
        var result = await _mediator.Send(command);
        Assert.True(result.IsFailure);
        Assert.Contains(ArtistError.SelfMergeAttempt, result.Errors);
    }
    
    [Fact]
    public async Task Handle_ShouldReturnAlreadyLinked_WhenPairExists()
    {
        var parent = new Artist("Parent", RandomString(), RandomString());
        var child = new Artist("Child", RandomString(), RandomString());

        _uow.ArtistRepository.Insert(parent);
        _uow.ArtistRepository.Insert(child);
        await _uow.SaveChangesAsync();
        
        //Do same merge twice
        var command = new MergeArtistsCommand(parent.Guid, child.Guid);
        var result = await _mediator.Send(command);
        var result1 = await _mediator.Send(command);
        Assert.True(result.IsSuccess);
        Assert.True(result1.IsFailure);
        Assert.Contains(ArtistError.AlreadyLinked, result1.Errors);
    }

    [Fact]
    public async Task Handle_ShouldReturnInvalidMergePair_WhenParentOrChildDoesNotExist()
    {
        var command = new MergeArtistsCommand(Guid.NewGuid(), Guid.NewGuid());
        var result = await _mediator.Send(command);
        Assert.True(result.IsFailure);
        Assert.Contains(ArtistError.InvalidMergePair, result.Errors);
    }

    [Fact]
    public async Task Handle_ShouldUpdateParentAndChildren_WhenValidMerge()
    {
        //Create simple parent - child pair with no sub children
        var parent = new Artist("Parent", RandomString(), RandomString());
        var child = new Artist("Child", RandomString(), RandomString());

        _uow.ArtistRepository.Insert(parent);
        _uow.ArtistRepository.Insert(child);
        await _uow.SaveChangesAsync();

        var childParentCommand = new MergeArtistsCommand(parent.Guid, child.Guid);
        var childParentResult = await _mediator.Send(childParentCommand);

        Assert.True(childParentResult.IsSuccess);

        var updatedChild = await _uow.ArtistRepository.FirstOrDefaultAsync(x => x.Guid == child.Guid);
        Assert.NotNull(updatedChild);
        Assert.Equal(parent.Guid, updatedChild.ParentId);
        Assert.Equal(parent.Guid, updatedChild.TopLvlParentId);

        //Add sub children to child
        var sub1 = new Artist("subChild1", RandomString(), RandomString());
        var sub2 = new Artist("subChild2", RandomString(), RandomString())
        {
            ParentId = sub1.Guid,
            TopLvlParentId = sub1.Guid
        };
        _uow.ArtistRepository.Insert(sub1);
        _uow.ArtistRepository.Insert(sub2);
        await _uow.SaveChangesAsync();

        var subChildCommand = new MergeArtistsCommand(ParentId: updatedChild.Guid, ChildId: sub1.Guid);
        var subChildCommandResult = await _mediator.Send(subChildCommand);

        Assert.True(subChildCommandResult.IsSuccess);

        var updatedSub1 = await _uow.ArtistRepository.FirstOrDefaultAsync(x => x.Guid == sub1.Guid);
        var updatedSub2 = await _uow.ArtistRepository.FirstOrDefaultAsync(x => x.Guid == sub2.Guid);

        var parentWithChildren = await _uow.ArtistRepository.FirstOrDefaultAsync(x => x.Guid == parent.Guid,
            includes: x => x.AllChildren);

        Assert.NotNull(updatedSub1);
        Assert.NotNull(updatedSub2);
        Assert.NotNull(parentWithChildren);

        var parentChildren = parentWithChildren.AllChildren.Select(x => x.Guid).ToArray();
        var expectedChildren = new[] { child.Guid, sub1.Guid, sub2.Guid };
        
        Assert.True(parentChildren.OrderBy(x => x).SequenceEqual(expectedChildren.OrderBy(x => x)));
        Assert.Equal(updatedSub1.ParentId, child.Guid);
    }

    [Fact]
    public async Task Handle_ShouldReturnCircularDependency_WhenChildIsTopLvlParentOfParent()
    {
        // Creating a parent and child artist
        var parent = new Artist("Parent", RandomString(), RandomString());
        var child = new Artist("Child", RandomString(), RandomString())
        {
            ParentId = parent.Guid,
            TopLvlParentId = parent.Guid
        };
        var subChild = new Artist("SubChild", RandomString(), RandomString())
        {
            ParentId = child.Guid,
            TopLvlParentId = child.TopLvlParentId
        };

        _uow.ArtistRepository.Insert(parent);
        _uow.ArtistRepository.Insert(child);
        _uow.ArtistRepository.Insert(subChild);

        await _uow.SaveChangesAsync();

        var command = new MergeArtistsCommand(subChild.Guid, parent.Guid);

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(ArtistError.CircularDependency, result.Errors);
    }
}