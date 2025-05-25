using Database.Entity;
using Database.Util;
using Microsoft.EntityFrameworkCore;

namespace Database;

public class ApplicationDatabaseContext(
    DbContextOptions<ApplicationDatabaseContext> options,
    TimeProvider timeProvider)
    : DbContext(options)
{
    public const string SchemaName = "paste";
    
    public DbSet<UserEntity> User { get; init; }
    
    public DbSet<FileEntity> File { get; init; }
    
    public DbSet<ContentEntity> Content { get; init; }
    
    public DbSet<FileActionEntity> Action { get; init; }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        this.ChangeTracker.ReplaceDeletionWithSoftDeletionForSoftDeletableEntities(timeProvider);
        return await base.SaveChangesAsync(cancellationToken);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaName);
        
        UserEntity.Configure(modelBuilder);
        FileEntity.Configure(modelBuilder);
        FileActionEntity.Configure(modelBuilder);
        ContentEntity.Configure(modelBuilder);
        
        base.OnModelCreating(modelBuilder);
    }
}