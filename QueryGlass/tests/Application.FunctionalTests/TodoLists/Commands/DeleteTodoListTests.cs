using QueryGlass.Application.TodoLists.Commands.CreateTodoList;
using QueryGlass.Application.TodoLists.Commands.DeleteTodoList;
using QueryGlass.Domain.Entities;

using static QueryGlass.Application.FunctionalTests.Testing;

namespace QueryGlass.Application.FunctionalTests.TodoLists.Commands;

public class DeleteTodoListTests : BaseTestFixture
{
    [Test]
    public async Task ShouldRequireValidTodoListId()
    {
        var command = new DeleteTodoListCommand(99);
        await Should.ThrowAsync<NotFoundException>(() => SendAsync(command));
    }

    [Test]
    public async Task ShouldDeleteTodoList()
    {
        var listId = await SendAsync(new CreateTodoListCommand
        {
            Title = "New List"
        });

        await SendAsync(new DeleteTodoListCommand(listId));

        var list = await FindAsync<TodoList>(listId);

        list.ShouldBeNull();
    }
}
