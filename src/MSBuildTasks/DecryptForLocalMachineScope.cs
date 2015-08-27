using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Build.Framework;

namespace DavidLievrouw.MSBuildTasks {
  public class DecryptForLocalMachineScope : CustomTask {
    static readonly byte[] UTF8Preamble = Encoding.UTF8.GetPreamble();

    public override bool Execute() {
      if (string.IsNullOrWhiteSpace(StringToDecrypt)) throw new InvalidOperationException("No valid input string is defined.");

      Logger.LogMessage(MessageImportance.High, "Decrypting: " + StringToDecrypt);

      var cypher = Convert.FromBase64String(StringToDecrypt);
      var purposes = Purposes == null
        ? new string[] {}
        : Purposes.Where(purpose => !string.IsNullOrWhiteSpace(purpose)).ToArray();
      var entropyString = string.Join(";", purposes).Trim();
      var entropy = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(entropyString)).ToArray();

      var userData = ProtectedData.Unprotect(cypher, entropy, DataProtectionScope.LocalMachine);
      userData = StripBOM(userData);

      DecryptedString = Encoding.UTF8.GetString(userData);

      Logger.LogMessage(MessageImportance.High, "Decrypted successfully.");

      return true;
    }

    [Required]
    public string StringToDecrypt { get; set; }

    [Output]
    public string DecryptedString { get; set; }

    public string[] Purposes { get; set; }

    static byte[] StripBOM(byte[] userData) {
      if (userData == null) return null;
      if (userData.Take(UTF8Preamble.Length).SequenceEqual(UTF8Preamble)) {
        userData = userData.Skip(UTF8Preamble.Length).ToArray();
      }
      return userData;
    }
  }
}