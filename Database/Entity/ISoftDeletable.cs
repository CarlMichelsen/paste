namespace Database.Entity;

public interface ISoftDeletable
{
    DateTimeOffset? DeletedAt { get; }
}