namespace TheGourmet.Application.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message, object key) : base($"Entity \"{message}\" ({key}) was not found.")
    {
    }

    public NotFoundException(string message) : base(message)
    {
    }
}