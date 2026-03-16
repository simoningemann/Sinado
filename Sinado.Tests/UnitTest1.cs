using Sinado.Core.Domain;
using Sinado.Core.WorkItems;

namespace Sinado.Tests;

public sealed class WorkItemsHandlersTests
{
    [Fact]
    public async Task CreateWorkItem_requires_title()
    {
        var handler = new CreateWorkItemHandler(new FakeRepo());

        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await handler.Handle(new CreateWorkItemCommand("   ", "x", WorkItemType.Task), CancellationToken.None)
        );
    }

    [Fact]
    public async Task CreateWorkItem_creates_new_item()
    {
        var repo = new FakeRepo();
        var handler = new CreateWorkItemHandler(repo);

        var created = await handler.Handle(
            new CreateWorkItemCommand("Hello", "World", WorkItemType.Bug),
            CancellationToken.None
        );

        Assert.NotEqual(Guid.Empty, created.Id);
        Assert.Equal("Hello", created.Title);
        Assert.Equal("World", created.Description);
        Assert.Equal(WorkItemType.Bug, created.Type);
        Assert.Equal(WorkItemStatus.New, created.Status);
    }

    [Fact]
    public async Task UpdateWorkItem_returns_null_when_missing()
    {
        var handler = new UpdateWorkItemHandler(new FakeRepo());
        var result = await handler.Handle(new UpdateWorkItemCommand(Guid.NewGuid(), "t", null, null), CancellationToken.None);
        Assert.Null(result);
    }

    [Fact]
    public async Task ChangeStatus_returns_null_when_missing()
    {
        var handler = new ChangeWorkItemStatusHandler(new FakeRepo());
        var result = await handler.Handle(new ChangeWorkItemStatusCommand(Guid.NewGuid(), WorkItemStatus.Active), CancellationToken.None);
        Assert.Null(result);
    }

    private sealed class FakeRepo : IWorkItemRepository
    {
        private readonly Dictionary<Guid, WorkItem> _items = new();

        public Task<WorkItem> Add(WorkItem item, CancellationToken ct)
        {
            _items[item.Id] = Clone(item);
            return Task.FromResult(Clone(item));
        }

        public Task<WorkItem?> Get(Guid id, CancellationToken ct)
        {
            return Task.FromResult(_items.TryGetValue(id, out var item) ? Clone(item) : null);
        }

        public Task<IReadOnlyList<WorkItem>> List(WorkItemListFilter filter, CancellationToken ct)
        {
            IEnumerable<WorkItem> query = _items.Values;
            if (filter.Status is not null) query = query.Where(w => w.Status == filter.Status.Value);
            if (filter.Type is not null) query = query.Where(w => w.Type == filter.Type.Value);

            var take = filter.Take <= 0 ? 50 : Math.Min(filter.Take, 200);
            var skip = Math.Max(filter.Skip, 0);

            return Task.FromResult<IReadOnlyList<WorkItem>>(
                query
                    .OrderByDescending(w => w.CreatedAt)
                    .Skip(skip)
                    .Take(take)
                    .Select(Clone)
                    .ToList()
                    .AsReadOnly()
            );
        }

        public Task<WorkItem?> Update(WorkItem item, CancellationToken ct)
        {
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
}
