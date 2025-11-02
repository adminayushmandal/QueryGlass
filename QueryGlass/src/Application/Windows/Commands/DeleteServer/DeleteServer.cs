using QueryGlass.Application.Common.Interfaces;

namespace QueryGlass.Application.Windows.Commands.DeleteServer
{
    public record DeleteServerCommand(Guid SystemInfoId) : IRequest<Result>;

    internal sealed class DeleteServerCommandHandler(IWindowsRepository systemInfoRepository) : IRequestHandler<DeleteServerCommand, Result>
    {
        private readonly IWindowsRepository _systemInfoRepository = systemInfoRepository;
        public async Task<Result> Handle(DeleteServerCommand request, CancellationToken cancellationToken)
        {
            var result = await _systemInfoRepository.DeleteAsync(request.SystemInfoId, cancellationToken);
            return result ? Result.Success() : Result.Failure(["Failed to delete the server."]);
        }
    }
}
