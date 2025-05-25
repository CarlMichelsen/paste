namespace Presentation.Dto.File;

public record FileDto(
    string Id,
    string Name,
    string ContentId,
    string OwnerId,
    string MimeType,
    long Size,
    DateTimeOffset CreatedAt);