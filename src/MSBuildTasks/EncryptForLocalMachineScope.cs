using System;
using System.Linq;
using System.Text;
using DavidLievrouw.MSBuildTasks.Crypto;
using Microsoft.Build.Framework;

namespace DavidLievrouw.MSBuildTasks {
  public class EncryptForLocalMachineScope : CustomTask {
    IEntropyCreator _entropyCreator;
    IDataProtectorFactory _dataProtectorFactory;
    static readonly object Lock = new object();

    public override bool Execute() {
      if (string.IsNullOrWhiteSpace(StringToEncrypt)) throw new InvalidOperationException("No valid input string is defined.");

      Logger.LogMessage(MessageImportance.High, "Encrypting: " + StringToEncrypt);

      var entropy = EntropyCreator.CreateEntropy(Purposes);
      var dataProtector = DataProtectorFactory.Create(entropy);

      var userData = Encoding.UTF8.GetBytes(StringToEncrypt);
      var cypher = dataProtector.Protect(userData);

      EncryptedString = Convert.ToBase64String(cypher);

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