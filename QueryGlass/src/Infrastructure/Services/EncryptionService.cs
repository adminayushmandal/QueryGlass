using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using QueryGlass.Application.Common.Interfaces;

namespace QueryGlass.Infrastructure.Services;

internal sealed class EncryptionService(IDataProtectionProvider provider, IConfiguration configuration) : IEncryptionService
{
    private readonly IDataProtector _protector = provider.CreateProtector(configuration["EncryptionKey"] ?? throw new InvalidOperationException("Encryption key not found in configuration."));
    public string DecryptData(string? value) => _protector.Unprotect(value ?? throw new ArgumentNullException(nameof(value), "Value connot be null or empty."));

    public string EncryptData(string? value) => _protector.Protect(value ?? throw new ArgumentNullException(nameof(value), "Value connot be null or empty."));
}
