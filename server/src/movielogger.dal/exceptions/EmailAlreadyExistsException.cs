using System;

namespace movielogger.dal.Exceptions;

public class EmailAlreadyExistsException : Exception
{
    public EmailAlreadyExistsException()
        : base("A user with this email already exists.") { }

    public EmailAlreadyExistsException(string message)
        : base(message) { }

    public EmailAlreadyExistsException(string message, Exception innerException)
        : base(message, innerException) { }
}