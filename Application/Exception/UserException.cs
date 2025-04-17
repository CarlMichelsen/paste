namespace Application.Exception;

public class UserException : System.Exception
{
    public UserException()
        : base()
    {
    }
    
    public UserException(string message)
        : base(message)
    {
    }
}