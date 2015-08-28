using System;
using System.Threading.Tasks;
using DavidLievrouw.MSBuildTasks.Handlers.Models;
using DavidLievrouw.Utils;
using DavidLievrouw.Utils.Crypto;

namespace DavidLievrouw.MSBuildTasks.Handlers {
  public class DecryptForLocalMachineScopeQueryHandler : IQueryHandler<DecryptForLocalMachineScopeRequest, string> {
    readonly ILocalMachineScopeStringEncryptor _localMachineScopeStringEncryptor;

    public DecryptForLocalMachineScopeQueryHandler(ILocalMachineScopeStringEncryptor localMachineScopeStringEncryptor) {
      if (localMachineScopeStringEncryptor == null) throw new ArgumentNullException("localMachineScopeStringEncryptor");
      _localMachineScopeStringEncryptor = localMachineScopeStringEncryptor;
    }

    public Task<string> Handle(DecryptForLocalMachineScopeRequest request) {
      return Task.FromResult(_localMachineScopeStringEncryptor.Decrypt(request.StringToDecrypt, request.Purposes));
    }
  }
}