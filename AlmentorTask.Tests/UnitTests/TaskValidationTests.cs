using FluentAssertions;
using Xunit;

public class TaskValidationTests
{
    [Fact]
    public void Task_Should_Fail_Validation_When_DueDate_Is_In_Past()
    {
        // Arrange
        var pastDueDate = DateTime.UtcNow.AddDays(-1);

        // Act
        var result = ValidateTaskDueDate(pastDueDate);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Be("Due date cannot be in the past.");
    }

    [Fact]
    public void Task_Should_Pass_Validation_When_DueDate_Is_Today_Or_Future()
    {
        // Arrange
        var futureDueDate = DateTime.UtcNow.AddDays(2);

        // Act
        var result = ValidateTaskDueDate(futureDueDate);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void StatusTransition_From_Done_To_Todo_Should_Log_Warning_And_Allow()
    {
        // Arrange
        var currentStatus = TaskStatus.Done;
        var newStatus = TaskStatus.Todo;

        // Act
        var transitionResult = HandleStatusTransition(currentStatus, newStatus);

        // Assert
        transitionResult.IsAllowed.Should().BeTrue();
        transitionResult.WasWarningLogged.Should().BeTrue();
    }

    // Helper simulation methods
    private (bool IsValid, string? ErrorMessage) ValidateTaskDueDate(DateTime? dueDate)
    {
        if (dueDate.HasValue && dueDate.Value.Date < DateTime.UtcNow.Date)
        {
            return (false, "Due date cannot be in the past.");
        }
        return (true, null);
    }

    private (bool IsAllowed, bool WasWarningLogged) HandleStatusTransition(TaskStatus current, TaskStatus target)
    {
        bool warningLogged = false;
        if (current == TaskStatus.Done && target == TaskStatus.Todo)
        {
            // Simulate logging unusual status transition
            warningLogged = true;
        }
        return (true, warningLogged);
    }
}

public enum TaskStatus { Todo, InProgress, Done }