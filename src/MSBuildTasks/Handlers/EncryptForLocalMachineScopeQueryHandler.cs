using System;
using System.Threading.Tasks;
using DavidLievrouw.MSBuildTasks.Handlers.Models;
using DavidLievrouw.Utils.Crypto;

namespace DavidLievrouw.MSBuildTasks.Handlers {
  public class EncryptForLocalMachineScopeQueryHandler : IQueryHandler<EncryptForLocalMachineScopeRequest, string> {
    readonly ILocalMachineScopeStringEncryptor _localMachineScopeStringEncryptor;

    public EncryptForLocalMachineScopeQueryHandler(ILocalMachineScopeStringEncryptor localMachineScopeStringEncryptor) {
      if (localMachineScopeStringEncryptor == null) throw new ArgumentNullException("localMachineScopeStringEncryptor");
      _localMachineScopeStringEncryptor = localMachineScopeStringEncryptor;
    }

    public Task<string> Handle(EncryptForLocalMachineScopeRequest request) {
      return Task.FromResult(_localMachineScopeStringEncryptor.Encrypt(request.StringToEncrypt, request.Purposes));
    }
  }
}