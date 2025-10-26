using Microsoft.Extensions.Logging;
using QueryGlass.Domain.Events;

namespace QueryGlass.Application.TodoItems.EventHandlers;

public class TodoItemCompletedEventHandler : INotificationHandler<TodoItemCompletedEvent>
{
    private readonly ILogger<TodoItemCompletedEventHandler> _logger;

    public TodoItemCompletedEventHandler(ILogger<TodoItemCompletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(TodoItemCompletedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("QueryGlass Domain Event: {DomainEvent}", notification.GetType().Name);

        return Task.CompletedTask;
    }
}
