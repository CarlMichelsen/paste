using System.ComponentModel.DataAnnotations;
using Database.Util;
using Microsoft.EntityFrameworkCore;

namespace Database.Entity;

public class UserEntity : IEntity 
{
    public required long Id { get; init; }
    
    [MaxLength(512)]
    public required string AuthenticationMethod { get; init; }
    
    [MaxLength(128)]
    public required string AuthenticationId { get; init; }
    
    public List<FileEntity>? Files { get; init; }
    
    public List<FileActionEntity>? Actions { get; init; }
    
    public required DateTimeOffset FirstLoginUtc { get; init; }
    
    public static void Configure(ModelBuilder modelBuilder)
    {
        var entityBuilder = modelBuilder
            .Entity<UserEntity>();
        
        entityBuilder
            .HasMany(x => x.Files)
            .WithOne(x => x.Owner);

        entityBuilder
            .HasMany(x => x.Actions)
            .WithOne(x => x.PerformedBy);
        
        entityBuilder
            .HasKey(x => x.Id);
    }
}