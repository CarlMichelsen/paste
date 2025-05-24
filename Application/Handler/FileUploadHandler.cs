using Application.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Presentation.Accessor;
using Presentation.Handler;
using Presentation.Repository;

namespace Application.Handler;

public class FileUploadHandler(
    ILogger<FileUploadHandler> logger,
    IUserRepository userRepository,
    IUserContextAccessor userContextAccessor,
    IFileWriteRepository fileWriteRepository) : IFileUploadHandler
{
    public async Task<IResult> UploadFile(IFormFile file)
    {
        var userContext = userContextAccessor.GetUserContext();
        await userRepository.TryUpsertUser(userContext.User);
        
        IFileContent fileContent = new FileContent(file);
        var fileEntity = await fileWriteRepository
            .CreateFile(FileNameSanitizer.SanitizedFileName(file.FileName), fileContent, userContext.User.Id);
        
        logger.LogInformation(
            "{user} uploaded file '{fileName}'",
            userContext.User.Username,
            fileEntity.Name);
        
        return Results.Created(fileEntity.Id.ToString(), file);
    }
}