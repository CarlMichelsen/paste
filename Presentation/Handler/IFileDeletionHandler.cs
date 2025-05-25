using Microsoft.AspNetCore.Http;

namespace Presentation.Handler;

public interface IFileDeletionHandler
{
    Task<IResult> DeleteFile(Guid fileId);
}