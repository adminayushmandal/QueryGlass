namespace QueryGlass.Application.SqlServer.Commands.AddNewSqlInstance;

public class AddNewSqlInstanceCommandValidator : AbstractValidator<AddNewSqlInstanceCommand>
{
    public AddNewSqlInstanceCommandValidator()
    {
        RuleFor(x => x.ConnectionString).NotEmpty().WithMessage("Connection string should not be empty.");
    }
}
