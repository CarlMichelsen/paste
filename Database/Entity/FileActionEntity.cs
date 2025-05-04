using System.ComponentModel.DataAnnotations;
using Database.Entity.Id;
using Database.Util;
using Microsoft.EntityFrameworkCore;

namespace Database.Entity;

public class FileActionEntity : IEntity
{
    public required FileActionEntityId Id { get; init; }
    
    public required FileEntityId FileId { get; init; }
    
    public FileEntity? File { get; init; }
    
    [MinLength(2)]
    [MaxLength(255)]
    public string? ChecksumAlgorithm { get; init; }
    
    [MinLength(2)]
    [MaxLength(255)]
    public string? Checksum { get; init; }
    
    public required FileAction Action { get; init; }
    
    public required long PerformedById { get; init; }
    
    public UserEntity? PerformedBy { get; init; }
    
    public required DateTimeOffset PerformedAt { get; init; }
    
    public static void Configure(ModelBuilder modelBuilder)
    {
        var entityBuilder = modelBuilder
            .Entity<FileActionEntity>();
        
        entityBuilder
            .HasKey(x => x.Id);
        
        entityBuilder
            .Property(x => x.Id)
            .RegisterTypedKeyConversion<FileActionEntity, FileActionEntityId>(x =>
                new FileActionEntityId(x, true));
        
        entityBuilder
            .Property(x => x.FileId)
            .RegisterTypedKeyConversion<FileEntity, FileEntityId>(x =>
                new FileEntityId(x, true));

        entityBuilder
            .Property(x => x.Action)
            .HasConversion<string>(
                x => x.ConvertToActionString(),
                x => x.ConvertToActionEnum());
        
        entityBuilder
            .HasOne(x => x.PerformedBy)
            .WithMany(x => x.Actions)
            .HasForeignKey(x => x.PerformedById);
        
        entityBuilder
            .HasOne(x => x.File)
            .WithMany(x => x.Actions)
            .HasForeignKey(x => x.FileId);
    }
}