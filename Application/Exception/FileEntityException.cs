using Database.Entity.Id;

namespace Application.Exception;

public abstract class FileEntityException(FileEntityId id, string message) : System.Exception(message)
{
    public FileEntityId FileEntityId => id;
}