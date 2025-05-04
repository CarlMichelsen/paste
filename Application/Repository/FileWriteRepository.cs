using Application.Exception;
using Database;
using Database.Entity;
using Database.Entity.Id;
using Database.Util;
using Microsoft.EntityFrameworkCore;
using Presentation.Repository;

namespace Application.Repository;

public class FileWriteRepository(
    ApplicationDatabaseContext databaseContext,
    TimeProvider timeProvider) : IFileWriteRepository
{
    public async Task<FileEntity> CreateFile(
        FileName fileName,
        IFileContent content,
        long ownerId)
    {
        var now = timeProvider.GetUtcNow();
        var fileId = new FileEntityId(Guid.CreateVersion7());
        var contentId = new ContentEntityId(Guid.CreateVersion7());
        var actionId = new FileActionEntityId(Guid.CreateVersion7());
        
        var contentEntity = new ContentEntity
        {
            Id = contentId,
            FileId = fileId,
            Data = content.Bytes,
        };
        
        var fileEntity = new FileEntity
        {
            Id = fileId,
            OwnerId = ownerId,
            Name = fileName,
            Actions = [
                new FileActionEntity
                {
                    Id = actionId,
                    FileId = fileId,
                    ChecksumAlgorithm = content.ChecksumAlgorithm,
                    Checksum = content.Checksum,
                    Action = FileAction.Create,
                    PerformedById = ownerId,
                    PerformedAt = now,
                },
            ],
            ContentId = contentId,
            Content = contentEntity,
            MimeType = content.MimeType,
            Size = contentEntity.Data.Length,
            CreatedAt = now,
        };
        
        databaseContext.File.Add(fileEntity);

        await databaseContext.SaveChangesAsync();
        return fileEntity;
    }

    public async Task<FileEntity?> SetFileContent(
        FileEntityId id,
        IFileContent content,
        long performedById)
    {
        var file = await databaseContext.File
            .Include(f => f.Content)
            .Include(f => f.Actions)
            .FirstOrDefaultAsync(f => f.OwnerId == performedById && f.Id == id);
        if (file is null)
        {
            return default;
        }
        
        file.Content!.Data = content.Bytes;

        var actionEntity = new FileActionEntity
        {
            Id = new FileActionEntityId(Guid.CreateVersion7()),
            FileId = file.Id,
            ChecksumAlgorithm = content.ChecksumAlgorithm,
            Checksum = content.Checksum,
            Action = FileAction.ContentUpdate,
            PerformedById = performedById,
            PerformedAt = timeProvider.GetUtcNow(),
        };
        file.Actions.Add(actionEntity);
        
        await databaseContext.SaveChangesAsync();
        return file;
    }

    public async Task<FileEntity?> SetFileName(
        FileEntityId id,
        FileName newFileName,
        long performedById)
    {
        var file = await databaseContext.File
            .Include(f => f.Actions)
            .FirstOrDefaultAsync(f => f.OwnerId == performedById && f.Id == id);
        if (file is null)
        {
            return default;
        }

        if (file.Name == newFileName)
        {
            throw new SameFileNameException(id, "Unable to change filename to the same name.");
        }
        
        file.Name = newFileName;
        
        var actionEntity = new FileActionEntity
        {
            Id = new FileActionEntityId(Guid.CreateVersion7()),
            FileId = file.Id,
            Action = FileAction.NameUpdate,
            PerformedById = performedById,
            PerformedAt = timeProvider.GetUtcNow(),
        };
        file.Actions.Add(actionEntity);
        
        await databaseContext.SaveChangesAsync();
        return file;
    }

    public async Task<FileEntity?> DeleteFile(
        FileEntityId id,
        long performedById)
    {
        var file = await databaseContext.File
            .Include(f => f.Actions)
            .FirstOrDefaultAsync(f => f.OwnerId == performedById && f.Id == id);
        if (file is null)
        {
            return default;
        }

        var now = timeProvider.GetUtcNow();
        var actionEntity = new FileActionEntity
        {
            Id = new FileActionEntityId(Guid.CreateVersion7()),
            FileId = file.Id,
            Action = FileAction.Delete,
            PerformedById = performedById,
            PerformedAt = now,
        };
        file.Actions.Add(actionEntity);
        databaseContext.File.Remove(file);
        
        await databaseContext.SaveChangesAsync();
        return file;
    }
}