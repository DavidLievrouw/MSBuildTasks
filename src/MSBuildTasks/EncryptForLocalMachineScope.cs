using System;
using DavidLievrouw.MSBuildTasks.Handlers;
using DavidLievrouw.MSBuildTasks.Handlers.Models;
using DavidLievrouw.MSBuildTasks.Handlers.Models.Validation;
using DavidLievrouw.Utils.Crypto;
using Microsoft.Build.Framework;

namespace DavidLievrouw.MSBuildTasks {
  public class EncryptForLocalMachineScope : CustomTask {
    IQueryHandler<EncryptForLocalMachineScopeRequest, string> _encryptForLocalMachineScopeQueryHandler;
    static readonly object Lock = new object();

    public override bool Execute() {
      Logger.LogMessage(MessageImportance.High, "Encrypting: " + (StringToEncrypt ?? "[NULL]"));
      EncryptedString = EncryptForLocalMachineScopeQueryHandler.Handle(
        new EncryptForLocalMachineScopeRequest {
          StringToEncrypt = StringToEncrypt,
          Purposes = Purposes
        }).Result;
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
    public IQueryHandler<EncryptForLocalMachineScopeRequest, string> EncryptForLocalMachineScopeQueryHandler {
      get {
        lock (Lock) {
          return _encryptForLocalMachineScopeQueryHandler ??
                 (_encryptForLocalMachineScopeQueryHandler = new ValidationAwareQueryHandler<EncryptForLocalMachineScopeRequest, string>(
                   new EncryptForLocalMachineScopeRequestValidator(),
                   new EncryptForLocalMachineScopeQueryHandler(
                     new LocalMachineScopeStringEncryptor(new EntropyCreator(), new DataProtectorFactory()))));
        }
      }
      internal set {
        lock (Lock) {
          if (value == null) throw new ArgumentNullException("value");
          _encryptForLocalMachineScopeQueryHandler = value;
        }
      }
    }
  }
}