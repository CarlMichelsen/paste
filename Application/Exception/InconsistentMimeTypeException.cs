namespace Application.Exception;

public class InconsistentMimeTypeException(string message)
    : System.Exception(message);