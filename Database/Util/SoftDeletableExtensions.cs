using Database.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Util;

public static class SoftDeletableExtensions
{
    public static void MakeSoftDeletable<TEntity>(
        this EntityTypeBuilder<TEntity> entity,
        ModelBuilder modelBuilder)
        where TEntity : class, ISoftDeletable
    {
        entity.HasQueryFilter(e => e.DeletedAt == null);

        var entityType = modelBuilder.Model.FindEntityType(typeof(TEntity));
        if (entityType is null)
        {
            throw new NullReferenceException($"Cannot find entity type {typeof(TEntity).Name}");
        }
        
        var deletedAtColumn = entityType.FindProperty(nameof(ISoftDeletable.DeletedAt));
        if (deletedAtColumn is null)
        {
            throw new NullReferenceException($"Cannot find {nameof(ISoftDeletable.DeletedAt)} column when configuring soft-deletable on {typeof(TEntity).Name}");
        }
        
        entity
            .HasIndex(p => p.DeletedAt)
            .HasFilter($"\"{entityType.GetTableName()}\".\"{deletedAtColumn.GetColumnName()}\" IS NULL");
    }

    public static void ReplaceDeletionWithSoftDeletionForSoftDeletableEntities(
        this ChangeTracker changeTracker,
        TimeProvider timeProvider)
    {
        var softDeleteEntries = changeTracker
            .Entries<ISoftDeletable>()
            .Where(e => e.State == EntityState.Deleted);
        
        foreach (var entityEntry in softDeleteEntries)
        {
            entityEntry.State = EntityState.Modified;
            entityEntry.Property(nameof(ISoftDeletable.DeletedAt)).CurrentValue = timeProvider.GetUtcNow();
        }
    }
}