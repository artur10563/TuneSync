namespace Domain.Errors;

public static class ArtistError
{
    public static Error InvalidMergePair => new("Invalid merge pair. Parent or Child does not exist.");
    public static Error SelfMergeAttempt => new("Parent and Child must be different. Self merge is not allowed.");
    public static Error CircularDependency => new("Invalid merge pair. This pair creates circular dependency.");
}