using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sinado.Core.Domain;
using Sinado.Core.WorkItems;

namespace Sinado.Api.WorkItems;

[ApiController]
[Route("api/work-items")]
public sealed class WorkItemsController(ISender sender) : ControllerBase
{
    public sealed record CreateWorkItemRequest(string Title, string? Description, WorkItemType Type);
    public sealed record UpdateWorkItemRequest(string? Title, string? Description, WorkItemType? Type);
    public sealed record ChangeStatusRequest(WorkItemStatus Status);

    [HttpPost]
    public async Task<ActionResult<WorkItem>> Create([FromBody] CreateWorkItemRequest request, CancellationToken ct)
    {
        var created = await sender.Send(
            new CreateWorkItemCommand(request.Title, request.Description, request.Type),
            ct
        );

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<WorkItem>> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var item = await sender.Send(new GetWorkItemQuery(id), ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<WorkItem>>> List(
        [FromQuery] WorkItemStatus? status,
        [FromQuery] WorkItemType? type,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50,
        CancellationToken ct = default
    )
    {
        var filter = new WorkItemListFilter(status, type, skip, take);
        var items = await sender.Send(new ListWorkItemsQuery(filter), ct);
        return Ok(items);
    }

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<WorkItem>> Update([FromRoute] Guid id, [FromBody] UpdateWorkItemRequest request, CancellationToken ct)
    {
        var updated = await sender.Send(
            new UpdateWorkItemCommand(id, request.Title, request.Description, request.Type),
            ct
        );

        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPut("{id:guid}/status")]
    public async Task<ActionResult<WorkItem>> ChangeStatus([FromRoute] Guid id, [FromBody] ChangeStatusRequest request, CancellationToken ct)
    {
        var updated = await sender.Send(new ChangeWorkItemStatusCommand(id, request.Status), ct);
        return updated is null ? NotFound() : Ok(updated);
    }
}

