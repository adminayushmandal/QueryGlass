using QueryGlass.Application.Common.Interfaces;
using QueryGlass.Domain.Entities;

namespace QueryGlass.Application.Common.Models;

public record WindowsLookupDto
{
    public IReadOnlyCollection<WindowsDto> Windows { get; set; } = [];
}

public record WindowsDto
{
    public Guid Id { get; set; }
    public string? ServerName { get; set; }
    public string? OS { get; set; }
    public DateTimeOffset Created { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<WindowsServer, WindowsDto>()
            .ForMember(dest => dest.ServerName, o => o.MapFrom<DecryptionValueResolver, string?>(src => src.MachineName))
            .ForMember(dest => dest.OS, o => o.MapFrom<DecryptionValueResolver, string?>(src => src.OSVersion));

            CreateMap<WindowsDto, WindowsServer>()
            .ForMember(dest => dest.MachineName, o => o.MapFrom<EncryptionValueResolver, string?>(src => src.ServerName))
            .ForMember(dest => dest.OSVersion, o => o.MapFrom<EncryptionValueResolver, string?>(src => src.OS));
        }
    }
}


public class EncryptionValueResolver(IEncryptionService service) : IMemberValueResolver<object, object, string?, string?>
{
    private readonly IEncryptionService _service = service;
    public string? Resolve(object source, object destination, string? sourceMember, string? destMember, ResolutionContext context)
    {
        return string.IsNullOrEmpty(destMember)
        ? string.Empty
        : _service.EncryptData(destMember);
    }
}

public class DecryptionValueResolver(IEncryptionService service) : IMemberValueResolver<object, object, string?, string?>
{
    private readonly IEncryptionService _service = service;
    public string? Resolve(object source, object destination, string? sourceMember, string? destMember, ResolutionContext context)
    {
        return string.IsNullOrEmpty(sourceMember)
        ? string.Empty
        : _service.DecryptData(sourceMember);
    }
}