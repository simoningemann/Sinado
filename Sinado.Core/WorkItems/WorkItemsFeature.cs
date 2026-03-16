using MediatR;
using Sinado.Core.Domain;

namespace Sinado.Core.WorkItems;

public interface IWorkItemRepository
{
    Task<WorkItem> Add(WorkItem item, CancellationToken ct);
    Task<WorkItem?> Get(Guid id, CancellationToken ct);
    Task<IReadOnlyList<WorkItem>> List(WorkItemListFilter filter, CancellationToken ct);
    Task<WorkItem?> Update(WorkItem item, CancellationToken ct);
}

public sealed record WorkItemListFilter(
    WorkItemStatus? Status = null,
    WorkItemType? Type = null,
    int Skip = 0,
    int Take = 50
);

public sealed record CreateWorkItemCommand(
    string Title,
    string? Description,
    WorkItemType Type
) : IRequest<WorkItem>;

public sealed class CreateWorkItemHandler(IWorkItemRepository repo) : IRequestHandler<CreateWorkItemCommand, WorkItem>
{
    public Task<WorkItem> Handle(CreateWorkItemCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new ArgumentException("Title is required.", nameof(request.Title));

        var item = new WorkItem
        {
            Title = request.Title.Trim(),
            Description = request.Description?.Trim() ?? string.Empty,
            Type = request.Type,
            Status = WorkItemStatus.New,
            CreatedAt = DateTime.UtcNow
        };

        return repo.Add(item, ct);
    }
}

public sealed record GetWorkItemQuery(Guid Id) : IRequest<WorkItem?>;

public sealed class GetWorkItemHandler(IWorkItemRepository repo) : IRequestHandler<GetWorkItemQuery, WorkItem?>
{
    public Task<WorkItem?> Handle(GetWorkItemQuery request, CancellationToken ct) => repo.Get(request.Id, ct);
}

public sealed record ListWorkItemsQuery(WorkItemListFilter Filter) : IRequest<IReadOnlyList<WorkItem>>;

public sealed class ListWorkItemsHandler(IWorkItemRepository repo)
    : IRequestHandler<ListWorkItemsQuery, IReadOnlyList<WorkItem>>
{
    public Task<IReadOnlyList<WorkItem>> Handle(ListWorkItemsQuery request, CancellationToken ct) =>
        repo.List(request.Filter, ct);
}

public sealed record UpdateWorkItemCommand(
    Guid Id,
    string? Title,
    string? Description,
    WorkItemType? Type
) : IRequest<WorkItem?>;

public sealed class UpdateWorkItemHandler(IWorkItemRepository repo) : IRequestHandler<UpdateWorkItemCommand, WorkItem?>
{
    public async Task<WorkItem?> Handle(UpdateWorkItemCommand request, CancellationToken ct)
    {
        var existing = await repo.Get(request.Id, ct);
        if (existing is null) return null;

        if (request.Title is not null)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                throw new ArgumentException("Title cannot be empty.", nameof(request.Title));
            existing.Title = request.Title.Trim();
        }

        if (request.Description is not null)
            existing.Description = request.Description.Trim();

        if (request.Type is not null)
            existing.Type = request.Type.Value;

        return await repo.Update(existing, ct);
    }
}

public sealed record ChangeWorkItemStatusCommand(Guid Id, WorkItemStatus Status) : IRequest<WorkItem?>;

public sealed class ChangeWorkItemStatusHandler(IWorkItemRepository repo)
    : IRequestHandler<ChangeWorkItemStatusCommand, WorkItem?>
{
    public async Task<WorkItem?> Handle(ChangeWorkItemStatusCommand request, CancellationToken ct)
    {
        var existing = await repo.Get(request.Id, ct);
        if (existing is null) return null;

        existing.Status = request.Status;
        return await repo.Update(existing, ct);
    }
}
