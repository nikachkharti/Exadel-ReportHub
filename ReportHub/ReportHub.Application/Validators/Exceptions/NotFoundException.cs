﻿namespace ReportHub.Application.Validators.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }

        public NotFoundException(string name, object key) : base($"Entity \"{name}\" ({key}) not found.")
        {
        }
    }
}
