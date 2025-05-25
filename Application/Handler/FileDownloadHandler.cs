using Application.Mapper;
using Database.Entity.Id;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Presentation.Accessor;
using Presentation.Dto;
using Presentation.Dto.File;
using Presentation.Handler;
using Presentation.Repository;

namespace Application.Handler;

public class FileDownloadHandler(
    ILogger<FileSearchHandler> logger,
    IUserContextAccessor userContextAccessor,
    IFileReadRepository fileReadRepository) : IFileDownloadHandler
{
    public async Task<IResult> DownloadFile(Guid fileId)
    {
        var userContext = userContextAccessor.GetUserContext();
        var fileEntity = await fileReadRepository
            .GetFullFileById(new FileEntityId(fileId), userContext.User.Id);

        if (fileEntity is null)
        {
            return Results.NotFound();
        }
        
        logger.LogInformation(
            "{user} downloaded file '{fileName}' - <{fileId}>",
            userContext.User.Username,
            fileEntity.Name,
            fileEntity.Id);
        return Results.File(fileEntity.Content!.Data, fileEntity.MimeType, fileEntity.Name);
    }

    public async Task<IResult> GetFileMetadata(Guid fileId)
    {
        var userContext = userContextAccessor.GetUserContext();
        var fileEntity = await fileReadRepository
            .GetFileWithoutContentById(new FileEntityId(fileId), userContext.User.Id);
        
        if (fileEntity is null)
        {
            return Results.NotFound();
        }
        
        logger.LogInformation(
            "{user} requested file metadata for '{fileName}' - <{fileId}>",
            userContext.User.Username,
            fileEntity.Name,
            fileEntity.Id);
        return Results.Ok(new ServiceResponse<FileDto>(fileEntity.ToDto()));
    }
}