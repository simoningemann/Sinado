using System.Collections.Concurrent;
using Sinado.Core.Domain;
using Sinado.Core.WorkItems;

namespace Sinado.Api.WorkItems;

public sealed class InMemoryWorkItemRepository : IWorkItemRepository
{
    private readonly ConcurrentDictionary<Guid, WorkItem> _items = new();

    public Task<WorkItem> Add(WorkItem item, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        _items[item.Id] = Clone(item);
        return Task.FromResult(Clone(item));
    }

    public Task<WorkItem?> Get(Guid id, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        return Task.FromResult(_items.TryGetValue(id, out var item) ? Clone(item) : null);
    }

    public Task<IReadOnlyList<WorkItem>> List(WorkItemListFilter filter, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var take = filter.Take <= 0 ? 50 : Math.Min(filter.Take, 200);
        var skip = Math.Max(filter.Skip, 0);

        IEnumerable<WorkItem> query = _items.Values;

        if (filter.Status is not null)
            query = query.Where(w => w.Status == filter.Status.Value);

        if (filter.Type is not null)
            query = query.Where(w => w.Type == filter.Type.Value);

        var page = query
            .OrderByDescending(w => w.CreatedAt)
            .Skip(skip)
            .Take(take)
            .Select(Clone)
            .ToList()
            .AsReadOnly();

        return Task.FromResult<IReadOnlyList<WorkItem>>(page);
    }

    public Task<WorkItem?> Update(WorkItem item, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (!_items.ContainsKey(item.Id))
            return Task.FromResult<WorkItem?>(null);

        _items[item.Id] = Clone(item);
        return Task.FromResult<WorkItem?>(Clone(item));
    }

    private static WorkItem Clone(WorkItem w) => new()
    {
        Id = w.Id,
        Title = w.Title,
        Description = w.Description,
        Status = w.Status,
        Type = w.Type,
        CreatedAt = w.CreatedAt
    };
}

