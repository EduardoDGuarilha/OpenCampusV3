using System;

namespace OpenCampus.API.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message, string? errorCode = null)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    public string? ErrorCode { get; }
}