using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Build.Framework;

namespace DavidLievrouw.MSBuildTasks {
  public class EncryptForLocalMachineScope : CustomTask {
    public override bool Execute() {
      if (string.IsNullOrWhiteSpace(StringToEncrypt)) throw new InvalidOperationException("No valid input string is defined.");

      Logger.LogMessage(MessageImportance.High, "Encrypting by machine key: " + StringToEncrypt);

      var userData = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(StringToEncrypt)).ToArray();
      var purposes = Purposes == null
        ? new string[] {}
        : Purposes.Where(purpose => !string.IsNullOrWhiteSpace(purpose)).ToArray()
        ;
      var entropyString = string.Join(";", purposes);
      var entropy = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(entropyString)).ToArray();

      var cypher = ProtectedData.Protect(userData, entropy, DataProtectionScope.LocalMachine);

      EncryptedString = Convert.ToBase64String(cypher);

      Logger.LogMessage(MessageImportance.High, "Encrypted successfully.");

      return true;
    }

    [Output]
    public string EncryptedString { get; set; }

    [Required]
    public string StringToEncrypt { get; set; }

    public string[] Purposes { get; set; }
  }
}