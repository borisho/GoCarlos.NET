using System;

namespace GoCarlos.NET.Exceptions;

public class EgdServiceException : Exception
{
    public EgdServiceException() { }

    public EgdServiceException(string message) : base(message) { }

    public EgdServiceException(string message, Exception inner) : base(message, inner) { }
}
