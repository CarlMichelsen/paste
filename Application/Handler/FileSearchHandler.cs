using Application.Mapper;
using Microsoft.Extensions.Logging;
using Presentation.Accessor;
using Presentation.Dto;
using Presentation.Dto.File;
using Presentation.Handler;
using Presentation.Repository;

namespace Application.Handler;

public class FileSearchHandler(
    ILogger<FileSearchHandler> logger,
    IUserContextAccessor userContextAccessor,
    IFileReadRepository fileReadRepository) : IFileSearchHandler
{
    public async Task<ServiceResponse<List<FileDto>>> GetFiles(
        int skip,
        int amount)
    {
        var userContext = userContextAccessor.GetUserContext();
        var files = await fileReadRepository
            .GetLatestFiles(userContext.User.Id, skip, amount);
        
        var fileDtos = files
            .Select(f => f.ToDto())
            .ToList();
        
        logger.LogInformation(
            "{user} searched for the {amount} latest files, skipping {skip} - found {fileCount} files",
            userContext.User.Username,
            amount,
            skip,
            files.Count);
        return new ServiceResponse<List<FileDto>>(fileDtos);
    }

    public async Task<ServiceResponse<List<FileDto>>> SearchFiles(string partialFileName, int amount)
    {
        var userContext = userContextAccessor.GetUserContext();
        var files = await fileReadRepository
            .SearchByName(partialFileName, amount, userContext.User.Id);
        
        var fileDtos = files
            .Select(f => f.ToDto())
            .ToList();
        
        logger.LogInformation(
            "{user} did a filename search for '{partialFileName}' - found {fileCount} files",
            userContext.User.Username,
            partialFileName,
            files.Count);
        return new ServiceResponse<List<FileDto>>(fileDtos);
    }
}