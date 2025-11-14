using System;

namespace OpenCampus.API.Common.Exceptions;

public class ForbiddenException : Exception
{
    public ForbiddenException(string message, string? errorCode = null)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    public string? ErrorCode { get; }
}