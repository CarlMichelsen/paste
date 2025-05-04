using Database.Util;

namespace Database.Entity.Id;

public class FileEntityId(Guid value, bool allowWrongVersion = false)
    : TypedGuid<FileEntity>(value, allowWrongVersion);