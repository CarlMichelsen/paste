using Microsoft.AspNetCore.Http;

namespace Presentation.Handler;

public interface IFileDownloadHandler
{
    Task<IResult> DownloadFile(Guid fileId);
    
    Task<IResult> GetFileMetadata(Guid fileId);
}