using Microsoft.Extensions.Logging;
using QueryGlass.Domain.Events;

namespace QueryGlass.Application.TodoItems.EventHandlers;

public class TodoItemCreatedEventHandler : INotificationHandler<TodoItemCreatedEvent>
{
    private readonly ILogger<TodoItemCreatedEventHandler> _logger;

    public TodoItemCreatedEventHandler(ILogger<TodoItemCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(TodoItemCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("QueryGlass Domain Event: {DomainEvent}", notification.GetType().Name);

        return Task.CompletedTask;
    }
}
