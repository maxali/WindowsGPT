using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.DataProtection;


namespace WindowsGPT
{
  public static class KeyProvider
  {
    private static readonly byte[] entropy = Encoding.Unicode.GetBytes("kjhadjk$$*&^@P)({J;'sjdfsjkhdfsjdkfh");

    public static void SaveAPIKey(string apiKey, string fileName = "appdata")
    {
      byte[] apiKeyBytes = Encoding.Unicode.GetBytes(apiKey);
      byte[] encryptedApiKey = ProtectedData.Protect(apiKeyBytes, entropy, DataProtectionScope.CurrentUser);
      File.WriteAllBytes(fileName, encryptedApiKey);
    }

    public static string LoadAPIKey(string fileName = "appdata")
    {
      byte[] encryptedApiKey = File.ReadAllBytes(fileName);
      byte[] decryptedApiKey = ProtectedData.Unprotect(encryptedApiKey, entropy, DataProtectionScope.CurrentUser);
      return Encoding.Unicode.GetString(decryptedApiKey);
    }
  }
}
