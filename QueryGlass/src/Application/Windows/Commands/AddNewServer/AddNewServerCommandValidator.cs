namespace QueryGlass.Application.Windows.Commands.AddNewServer;

internal sealed class AddNewServerCommandValidator : AbstractValidator<AddNewServerCommand>
{
    public AddNewServerCommandValidator()
    {
        RuleFor(x => x.ServerName)
            .NotNull()
            .WithMessage("Server name should not be null");

        RuleFor(x => x.ServerName)
            .Matches(@"^[a-zA-Z0-9\-.]+$")
            .WithMessage("Invalid machine name.");
    }
}