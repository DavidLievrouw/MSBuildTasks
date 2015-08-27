using System;
using System.Text;
using DavidLievrouw.MSBuildTasks.Crypto;
using Microsoft.Build.Framework;

namespace DavidLievrouw.MSBuildTasks {
  public class DecryptForLocalMachineScope : CustomTask {
    IEntropyCreator _entropyCreator;
    IDataProtectorFactory _dataProtectorFactory;
    static readonly object Lock = new object();

    public override bool Execute() {
      if (string.IsNullOrWhiteSpace(StringToDecrypt)) throw new InvalidOperationException("No valid input string is defined.");

      Logger.LogMessage(MessageImportance.High, "Decrypting: " + StringToDecrypt);

      var entropy = EntropyCreator.CreateEntropy(Purposes);
      var dataProtector = DataProtectorFactory.Create(entropy);
      
      var cypher = Convert.FromBase64String(StringToDecrypt);
      var userData = dataProtector.Unprotect(cypher);

      DecryptedString = Encoding.UTF8.GetString(userData);

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
    public IEntropyCreator EntropyCreator {
      get {
        lock (Lock) {
          return _entropyCreator ?? (_entropyCreator = new EntropyCreator());
        }
      }
      internal set {
        lock (Lock) {
          if (value == null) throw new ArgumentNullException("value");
          _entropyCreator = value;
        }
      }
    }

    /// <remarks>
    ///   Property injection, with a local default, is needed, because MSBuild requires a default constructor.
    /// </remarks>
    public IDataProtectorFactory DataProtectorFactory {
      get {
        lock (Lock) {
          return _dataProtectorFactory ?? (_dataProtectorFactory = new DataProtectorFactory());
        }
      }
      internal set {
        lock (Lock) {
          if (value == null) throw new ArgumentNullException("value");
          _dataProtectorFactory = value;
        }
      }
    }
  }
}