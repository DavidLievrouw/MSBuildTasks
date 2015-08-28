using System;
using DavidLievrouw.Utils.Crypto;
using Microsoft.Build.Framework;

namespace DavidLievrouw.MSBuildTasks {
  public class DecryptForLocalMachineScope : CustomTask {
    ILocalMachineScopeStringEncryptor _localMachineScopeStringEncryptor;
    static readonly object Lock = new object();

    public override bool Execute() {
      if (string.IsNullOrWhiteSpace(StringToDecrypt)) throw new InvalidOperationException("No valid input string is defined.");

      Logger.LogMessage(MessageImportance.High, "Decrypting: " + StringToDecrypt);
      DecryptedString = LocalMachineScopeStringEncryptor.Decrypt(StringToDecrypt, Purposes);
      Logger.LogMessage(MessageImportance.High, "Decrypted successfully.");

      return true;
    }

    [Required]
    public string StringToDecrypt { get; set; }

    [Output]
    public string DecryptedString { get; set; }

    public string[] Purposes { get; set; }

    /// <remarks>
    ///   Property injection, with a local default, is needed, because MSBuild requires a default constructor.
    /// </remarks>
    public ILocalMachineScopeStringEncryptor LocalMachineScopeStringEncryptor {
      get {
        lock (Lock) {
          return _localMachineScopeStringEncryptor ?? (_localMachineScopeStringEncryptor = new LocalMachineScopeStringEncryptor(new EntropyCreator(), new DataProtectorFactory()));
        }
      }
      internal set {
        lock (Lock) {
          if (value == null) throw new ArgumentNullException("value");
          _localMachineScopeStringEncryptor = value;
        }
      }
    }
  }
}