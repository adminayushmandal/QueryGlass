namespace QueryGlass.Application.Configuration.Commands.ConfigureUser;

internal sealed class ConfigureUserCommandValidator : AbstractValidator<ConfigureUserCommand>
{
    public ConfigureUserCommandValidator()
    {
        RuleFor(x => x.Email).NotNull().WithMessage("Email address should not be null.");
        RuleFor(x => x.DisplayName).NotNull().WithMessage("Display name is required to register user.");
        RuleFor(x => x.Role).NotEmpty().WithMessage("Role should not be null or empty for user configuration.");
    }
}
