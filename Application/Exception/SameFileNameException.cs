using Database.Entity.Id;

namespace Application.Exception;

public class SameFileNameException(FileEntityId id, string message)
    : FileEntityException(id, message);