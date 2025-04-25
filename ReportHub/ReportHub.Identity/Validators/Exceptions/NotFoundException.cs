﻿namespace ReportHub.Identity.Validators.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException()
    {
    }

    public NotFoundException(string? message) : base(message)
    {
    }

    public NotFoundException(string entityName, object key) 
        : base($"{entityName} with {key} not found")
    {
    }
}
