using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Xunit;

namespace AlmentorTask.Tests.IntegrationTests;

public class ApiIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApiIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    // -------------------------------------------------------------
    // Flow 1: Create project -> Add task -> Mark task as done -> Delete project
    // -------------------------------------------------------------
    [Fact]
    public async Task Flow1_CreateProject_AddTask_MarkDone_DeleteProject()
    {
        // 1. Create Project
        var createProjectPayload = new { name = "Project Alpha", description = "Test project" };
        var projectResponse = await _client.PostAsJsonAsync("/api/projects", createProjectPayload);
        projectResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var project = await projectResponse.Content.ReadFromJsonAsync<ProjectDto>();
        project.Should().NotBeNull();
        project!.Id.Should().BeGreaterThan(0);

        // 2. Add Task to Project
        var createTaskPayload = new
        {
            title = "Task 1",
            priority = "medium",
            due_date = DateTime.UtcNow.AddDays(5)
        };
        var taskResponse = await _client.PostAsJsonAsync($"/api/projects/{project.Id}/tasks", createTaskPayload);
        taskResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var task = await taskResponse.Content.ReadFromJsonAsync<TaskDto>();
        task.Should().NotBeNull();

        // 3. Mark Task as Done
        var updateTaskPayload = new { status = "done" };
        var updateResponse = await _client.PutAsJsonAsync($"/api/tasks/{task!.Id}", updateTaskPayload);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedTask = await updateResponse.Content.ReadFromJsonAsync<TaskDto>();
        updatedTask!.Status?.ToString()?.ToLower().Should().Contain("done");

        // 4. Delete Project (Cascade delete tasks)
        var deleteResponse = await _client.DeleteAsync($"/api/projects/{project.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify task is also deleted
        var getTaskResponse = await _client.GetAsync($"/api/tasks/{task.Id}");
        getTaskResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // -------------------------------------------------------------
    // Flow 2: Filter tasks by status and priority
    // -------------------------------------------------------------
    [Fact]
    public async Task Flow2_FilterTasksByStatusAndPriority()
    {
        // Arrange
        var projectRes = await _client.PostAsJsonAsync("/api/projects", new { name = "Project Beta" });
        var project = await projectRes.Content.ReadFromJsonAsync<ProjectDto>();

        await _client.PostAsJsonAsync($"/api/projects/{project!.Id}/tasks", new { title = "Task A", status = "todo", priority = "high" });
        await _client.PostAsJsonAsync($"/api/projects/{project.Id}/tasks", new { title = "Task B", status = "done", priority = "high" });

        // Act
        var response = await _client.GetAsync("/api/tasks?status=todo&priority=high");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PaginatedResponse<TaskDto>>();

        // Assert
        result.Should().NotBeNull();
        result!.Data.Should().NotBeEmpty();
    }

    // -------------------------------------------------------------
    // Flow 3: Search tasks and verify pagination
    // -------------------------------------------------------------
    [Fact]
    public async Task Flow3_SearchTasksAndVerifyPagination()
    {
        // Arrange
        var projectRes = await _client.PostAsJsonAsync("/api/projects", new { name = "Project Gamma" });
        var project = await projectRes.Content.ReadFromJsonAsync<ProjectDto>();

        for (int i = 1; i <= 3; i++)
        {
            await _client.PostAsJsonAsync($"/api/projects/{project!.Id}/tasks", new { title = $"SearchBug #{i}", description = "Item" });
        }

        var response = await _client.GetAsync("/api/tasks?q=SearchBug&pageIndex=1&pageSize=2");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PaginatedResponse<TaskDto>>();

        // Assert
        result.Should().NotBeNull();
        result!.Data.Should().HaveCount(2);
        result.PageIndex.Should().Be(1);
    }
    // -------------------------------------------------------------
    // DTOs matching the exact JSON response of your API
    // -------------------------------------------------------------
    public record ProjectDto(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description
);

    public record TaskDto(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("projectId")] int ProjectId,
        [property: JsonPropertyName("projectName")] string? ProjectName,
        [property: JsonPropertyName("title")] string Title,
        [property: JsonPropertyName("status")] object? Status,
        [property: JsonPropertyName("priority")] object? Priority
    );

    public record PaginatedResponse<T>(
        [property: JsonPropertyName("data")] List<T> Data,
        [property: JsonPropertyName("count")] int Count,
        [property: JsonPropertyName("pageIndex")] int PageIndex,
        [property: JsonPropertyName("pageSize")] int PageSize
    );
}
