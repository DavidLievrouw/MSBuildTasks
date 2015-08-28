using System;
using DavidLievrouw.MSBuildTasks.Handlers;
using DavidLievrouw.MSBuildTasks.Handlers.Models;
using DavidLievrouw.MSBuildTasks.Handlers.Models.Validation;
using DavidLievrouw.Utils.Crypto;
using Microsoft.Build.Framework;

namespace DavidLievrouw.MSBuildTasks {
  public class DecryptForLocalMachineScope : CustomTask {
    IQueryHandler<DecryptForLocalMachineScopeRequest, string> _decryptForLocalMachineScopeQueryHandler;
    static readonly object Lock = new object();

    public override bool Execute() {
      Logger.LogMessage(MessageImportance.High, "Decrypting: " + (StringToDecrypt ?? "[NULL]"));
      DecryptedString = DecryptForLocalMachineScopeQueryHandler.Handle(
        new DecryptForLocalMachineScopeRequest {
          StringToDecrypt = StringToDecrypt,
          Purposes = Purposes
        }).Result;
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
    public IQueryHandler<DecryptForLocalMachineScopeRequest, string> DecryptForLocalMachineScopeQueryHandler {
      get {
        lock (Lock) {
          return _decryptForLocalMachineScopeQueryHandler ??
                 (_decryptForLocalMachineScopeQueryHandler = new ValidationAwareQueryHandler<DecryptForLocalMachineScopeRequest, string>(
                   new DecryptForLocalMachineScopeRequestValidator(),
                   new DecryptForLocalMachineScopeQueryHandler(
                     new LocalMachineScopeStringEncryptor(new EntropyCreator(), new DataProtectorFactory()))));
        }
      }
      internal set {
        lock (Lock) {
          if (value == null) throw new ArgumentNullException("value");
          _decryptForLocalMachineScopeQueryHandler = value;
        }
      }
    }
  }
}