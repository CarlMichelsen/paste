using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Handler;

public interface IFileUploadHandler
{
    Task<IResult> UploadFile(IFormFile file);
}