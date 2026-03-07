using System.Security.Cryptography;
using System.Text;

namespace RoleSystemDemo.Services;

public static class PasswordService
{
    public static string ComputeHash(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexStringLower(bytes);
    }
}
