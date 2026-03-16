namespace Sinado.Core.Domain;

public enum WorkItemStatus { New, Active, Resolved, Closed }
public enum WorkItemType { Task, Bug, UserStory }

public class WorkItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public WorkItemStatus Status { get; set; } = WorkItemStatus.New;
    public WorkItemType Type { get; set; } = WorkItemType.Task;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}