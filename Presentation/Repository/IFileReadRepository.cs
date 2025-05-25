using Database.Entity;
using Database.Entity.Id;
using Database.Util;

namespace Presentation.Repository;

public interface IFileReadRepository
{
    Task<FileEntity?> GetFileWithoutContentById(FileEntityId id, long userId);
    
    Task<FileEntity?> GetFullFileById(FileEntityId id, long userId);
    
    Task<FileEntity?> GetFullFileByName(FileName fileName, long userId);
    
    Task<List<FileEntity>> SearchByName(string partialFileName, int maxResults, long userId);
    
    Task<List<FileEntity>> GetLatestFiles(long userId, int skip, int amount);
}