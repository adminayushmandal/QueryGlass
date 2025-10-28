using QueryGlass.Application.Common.Interfaces;

namespace QueryGlass.Application.SystemInformation.Commands.DeleteServer;

public record DeleteServerCommand(Guid SystemInfoId) : IRequest<Result>;

public class DeleteServerCommandValidator : AbstractValidator<DeleteServerCommand>
{
    public DeleteServerCommandValidator()
    {
    }
}

internal sealed class DeleteServerCommandHandler(ISystemInfoRepository systemInfoRepository) : IRequestHandler<DeleteServerCommand, Result>
{
    private readonly ISystemInfoRepository _systemInfoRepository = systemInfoRepository;
    public async Task<Result> Handle(DeleteServerCommand request, CancellationToken cancellationToken)
    {
        var result = await _systemInfoRepository.DeleteAsync(request.SystemInfoId, cancellationToken);
        return result ? Result.Success() : Result.Failure(["Failed to delete the server."]);
    }
}
