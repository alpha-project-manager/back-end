using System.Security.Cryptography;

namespace Application.Utils;

public static class GuidExtensions
{
    public static Guid FromInt(int value)
    {
        var input = BitConverter.GetBytes(value);
        var hash = MD5.HashData(input);
        return new Guid(hash);
    }
}