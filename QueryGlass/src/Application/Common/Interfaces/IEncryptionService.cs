namespace QueryGlass.Application.Common.Interfaces;

public interface IEncryptionService
{
    string EncryptData(string? value);
    string DecryptData(string? value);
}
