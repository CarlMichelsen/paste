using Presentation.Dto;
using Presentation.Dto.File;

namespace Presentation.Handler;

public interface IFileSearchHandler
{
    Task<ServiceResponse<List<FileDto>>> GetFiles(int skip, int amount);
    
    Task<ServiceResponse<List<FileDto>>> SearchFiles(string partialFileName, int amount);
}