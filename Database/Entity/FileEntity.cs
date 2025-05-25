using System.ComponentModel.DataAnnotations;
using Database.Entity.Id;
using Database.Util;
using Microsoft.EntityFrameworkCore;

namespace Database.Entity;

/// <summary>
/// Stores file-content in the database.
/// This is not ideal, but it will do for now.
/// This entity should have minimal responsibility to make migration easier later.
/// </summary>
public class FileEntity : IEntity, ISoftDeletable
{
    public required FileEntityId Id { get; init; }
    
    [MinLength(4)]
    public required FileName Name { get; set; }
    
    public required List<FileActionEntity> Actions { get; init; }
    
    public required ContentEntityId ContentId { get; init; }
    
    public ContentEntity? Content { get; init; }
    
    public required long OwnerId { get; init; }
    
    public UserEntity? Owner { get; init; }
    
    [MinLength(2)]
    [MaxLength(255)]
    public required string MimeType { get; set; }
    
    public required long Size { get; set; }
    
    public required DateTimeOffset CreatedAt { get; init; }
    
    public DateTimeOffset? DeletedAt { get; set; }
    
    public static void Configure(ModelBuilder modelBuilder)
    {
        var entityBuilder = modelBuilder
            .Entity<FileEntity>();

        entityBuilder
            .MakeSoftDeletable(modelBuilder);
        
        entityBuilder
            .HasKey(x => x.Id);
        
        entityBuilder
            .Property(x => x.Id)
            .RegisterTypedKeyConversion<FileEntity, FileEntityId>(x => new FileEntityId(x, true));
        
        entityBuilder
            .Property(x => x.ContentId)
            .RegisterTypedKeyConversion<ContentEntity, ContentEntityId>(x => new ContentEntityId(x, true));
        
        entityBuilder.Property(e => e.Name)
            .HasConversion(
                v => v.Value,
                v => new FileName(v))
            .HasMaxLength(255);

        entityBuilder
            .HasOne(x => x.Owner)
            .WithMany(x => x.Files)
            .HasForeignKey(x => x.OwnerId);

        entityBuilder
            .HasOne(x => x.Content)
            .WithOne(x => x.File)
            .HasForeignKey<FileEntity>(x => x.ContentId);

        entityBuilder
            .HasMany(x => x.Actions)
            .WithOne(x => x.File)
            .HasForeignKey(x => x.FileId);
        
        entityBuilder
            .HasIndex(x => new { Name = x.Name, x.OwnerId })
            .IsUnique();
    }
}