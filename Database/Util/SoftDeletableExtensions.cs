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

        var entityType = modelBuilder.Model.FindEntityType(typeof(TEntity))!;
        var deletedAtColumn = entityType.FindProperty(nameof(ISoftDeletable.DeletedAt))!;
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