using Database.Entity;
using Database.Entity.Id;
using Database.Util;

namespace Presentation.Repository;

public interface IFileWriteRepository
{
    Task<FileEntity> CreateFile(FileName fileName, IFileContent content, long ownerId);
    
    Task<FileEntity?> SetFileContent(FileEntityId id, IFileContent content, long performedById);
    
    Task<FileEntity?> SetFileName(FileEntityId id, FileName newFileName, long performedById);
    
    Task<FileEntity?> DeleteFile(FileEntityId id, long performedById);
}