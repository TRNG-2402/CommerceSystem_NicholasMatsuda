namespace CommerceSystem.Api.Exceptions;

public class DuplicateSkuException : Exception
{
    public DuplicateSkuException(string message)
        : base(message)
    {
    }
}