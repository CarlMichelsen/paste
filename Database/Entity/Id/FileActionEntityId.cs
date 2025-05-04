using Database.Util;

namespace Database.Entity.Id;

public class FileActionEntityId(Guid value, bool allowWrongVersion = false)
    : TypedGuid<FileActionEntity>(value, allowWrongVersion);