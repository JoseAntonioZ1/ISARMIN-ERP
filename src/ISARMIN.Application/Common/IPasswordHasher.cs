namespace ISARMIN.Application.Common;

public interface IPasswordHasher
{
    string HashearCredencial(string credencialPlana);

    bool VerificarCredencial(string credencialHash, string credencialPlana);
}
