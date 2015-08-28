using System;
using System.Text;
using DavidLievrouw.Utils.Crypto;
using Microsoft.Build.Framework;

namespace DavidLievrouw.MSBuildTasks {
  public class EncryptForLocalMachineScope : CustomTask {
    ILocalMachineScopeStringEncryptor _localMachineScopeStringEncryptor;
    static readonly object Lock = new object();

    public override bool Execute() {
      if (string.IsNullOrWhiteSpace(StringToEncrypt)) throw new InvalidOperationException("No valid input string is defined.");

      Logger.LogMessage(MessageImportance.High, "Encrypting: " + StringToEncrypt);
      EncryptedString = LocalMachineScopeStringEncryptor.Encrypt(StringToEncrypt, Purposes);
      Logger.LogMessage(MessageImportance.High, "Encrypted successfully.");

      return true;
    }

    [Output]
    public string EncryptedString { get; set; }

    [Required]
    public string StringToEncrypt { get; set; }

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