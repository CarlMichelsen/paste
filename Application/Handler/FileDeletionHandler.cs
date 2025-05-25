using Database.Entity.Id;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Presentation.Accessor;
using Presentation.Dto;
using Presentation.Handler;
using Presentation.Repository;

namespace Application.Handler;

public class FileDeletionHandler(
    ILogger<FileUploadHandler> logger,
    IUserRepository userRepository,
    IFileWriteRepository fileWriteRepository,
    IUserContextAccessor userContextAccessor) : IFileDeletionHandler
{
    public async Task<IResult> DeleteFile(Guid fileId)
    {
        var userContext = userContextAccessor.GetUserContext();
        await userRepository.TryUpsertUser(userContext.User);
        
        var deletedFile = await fileWriteRepository
            .DeleteFile(new FileEntityId(fileId), userContext.User.Id);

        if (deletedFile is not null)
        {
            logger.LogInformation(
                "{user} deleted file {fileName} - <{fileId}>",
                userContext.User.Username,
                deletedFile.Name,
                fileId);

            return Results.Ok(new ServiceResponse<bool>(true));
        }
        
        logger.LogInformation(
            "{user} attempted to delete file <{fileId}> but it was not deleted",
            userContext.User.Username,
            fileId);
        
        return Results.Ok(new ServiceResponse<bool>(false));
    }
}