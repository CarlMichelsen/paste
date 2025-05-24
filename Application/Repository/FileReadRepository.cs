using Database;
using Database.Entity;
using Database.Entity.Id;
using Database.Util;
using Microsoft.EntityFrameworkCore;
using Presentation.Repository;

namespace Application.Repository;

public class FileReadRepository(
    ApplicationDatabaseContext databaseContext) : IFileReadRepository
{
    public Task<FileEntity?> GetFileWithoutContentById(FileEntityId id, long userId)
    {
        return databaseContext.File
            .Where(file => file.OwnerId == userId && file.Id == id)
            .FirstOrDefaultAsync();
    }

    public Task<FileEntity?> GetFullFileById(FileEntityId id, long userId)
    {
        return databaseContext.File
            .Where(file => file.OwnerId == userId && file.Id == id)
            .Include(file => file.Content)
            .FirstOrDefaultAsync();
    }

    public Task<FileEntity?> GetFullFileByName(FileName fileName, long userId)
    {
        return databaseContext.File
            .Where(file => file.OwnerId == userId && file.Name == fileName)
            .Include(file => file.Content)
            .FirstOrDefaultAsync();
    }

    public async Task<List<FileEntity>> SearchByName(string partialFileName, int maxResults, long userId)
    {
        var results = await databaseContext.File
            .Where(file => file.OwnerId == userId && EF.Functions.Like(file.Name, $"%{partialFileName}%"))
            .Take(maxResults)
            .ToListAsync();

        var conformingFilename = partialFileName.Replace(' ', '_');
        
        return results
            .Select(file => new
            {
                File = file,
                
                // Ranking factors (in priority order)
                ExactMatch = file.Name.Value.Equals(conformingFilename, StringComparison.OrdinalIgnoreCase) ? 100 : 0,
                ExactNameMatch = file.Name.ToString().Equals(conformingFilename, StringComparison.OrdinalIgnoreCase) ? 80 : 0,
                StartsWithName = file.Name.ToString().StartsWith(conformingFilename, StringComparison.OrdinalIgnoreCase) ? 60 : 0,
                StartsWithFullName = file.Name.Value.StartsWith(conformingFilename, StringComparison.OrdinalIgnoreCase) ? 40 : 0,
                PositionInName = file.Name.ToString().IndexOf(conformingFilename, StringComparison.OrdinalIgnoreCase),
                PositionInFullName = file.Name.Value.IndexOf(conformingFilename, StringComparison.OrdinalIgnoreCase),
                NameLength = file.Name.ToString().Length,
                FullNameLength = file.Name.Value.Length,
            })
            .OrderByDescending(x => x.ExactMatch)
            .ThenByDescending(x => x.ExactNameMatch)
            .ThenByDescending(x => x.StartsWithName)
            .ThenByDescending(x => x.StartsWithFullName)
            .ThenBy(x => x.PositionInName >= 0 ? x.PositionInName : int.MaxValue)
            .ThenBy(x => x.PositionInFullName)
            .ThenBy(x => x.NameLength)
            .ThenBy(x => x.FullNameLength)
            .Select(x => x.File)
            .Take(maxResults)
            .ToList();
    }

    public Task<List<FileEntity>> GetLatestFiles(long userId, int skip, int amount)
    {
        return databaseContext.File
            .Where(file => file.OwnerId == userId)
            .OrderByDescending(file => file.CreatedAt)
            .Skip(skip)
            .Take(amount)
            .ToListAsync();
    }
}