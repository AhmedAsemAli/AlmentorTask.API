using System;
using System.Collections.Generic;
using System.Text;

namespace Almentor.Services.Exceptions
{
    public abstract class NotFoundException(string message) : Exception(message)
    {

    }
    public sealed class ProjectNotFoundException(int id) : NotFoundException($"Project with Id:{id} is not found ") { }
    public sealed class TaskNotFoundException(int id) : NotFoundException($"Task with Id:{id} is not found ") { }

    public abstract class BadRequestException(string message) : Exception(message)
    {
    }

    public sealed class InvalidDueDateException()
        : BadRequestException("Due date cannot be in the past.")
    {
    }

    public sealed class DuplicateProjectNameException(string name)
        : BadRequestException($"Project '{name}' already exists.")
    {
    }
}
