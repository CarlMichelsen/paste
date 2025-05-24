using Database.Entity;
using Presentation.Dto.File;

namespace Application.Mapper;

public static class FileDtoMapper
{
    public static FileDto ToDto(this FileEntity file)
    {
        return new FileDto(
            Id: file.Id.ToString(),
            Name: file.Name,
            ContentId: file.ContentId.ToString(),
            OwnerId: file.OwnerId.ToString(),
            MimeType: file.MimeType,
            Size: file.Size,
            CreatedAt: file.CreatedAt);
    }
}