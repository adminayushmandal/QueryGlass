namespace QueryGlass.Domain.Constants;

public abstract class Policies
{
    public const string AdminCanPurge = nameof(AdminCanPurge);
    public const string OperatorCanPurge = nameof(OperatorCanPurge);
    public const string DBACanPurge = nameof(DBACanPurge);
    public const string ReadOnly = nameof(ReadOnly);
}