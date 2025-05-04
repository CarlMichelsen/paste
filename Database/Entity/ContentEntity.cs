using Database.Entity.Id;
using Database.Util;
using Microsoft.EntityFrameworkCore;

namespace Database.Entity;

public class ContentEntity : IEntity
{
    public required ContentEntityId Id { get; init; }
    
    public required byte[] Data { get; set; }
    
    public required FileEntityId FileId { get; init; }
    
    public FileEntity? File { get; init; }
    
    public static void Configure(ModelBuilder modelBuilder)
    {
        var entityBuilder = modelBuilder
            .Entity<ContentEntity>();
        
        entityBuilder
            .HasKey(x => x.Id);
        
        entityBuilder
            .Property(x => x.Id)
            .RegisterTypedKeyConversion<ContentEntity, ContentEntityId>(x =>
                new ContentEntityId(x, true));
        
        entityBuilder
            .Property(x => x.FileId)
            .RegisterTypedKeyConversion<FileEntity, FileEntityId>(x =>
                new FileEntityId(x, true));
        
        entityBuilder
            .Property(b => b.Data)
            .HasColumnType("bytea")
            .IsRequired();

        entityBuilder
            .HasOne(x => x.File)
            .WithOne(x => x.Content)
            .HasForeignKey<ContentEntity>(x => x.FileId);
    }
}