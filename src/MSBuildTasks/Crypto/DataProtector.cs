using System.Security.Cryptography;

namespace DavidLievrouw.MSBuildTasks.Crypto {
  public class DataProtector : IDataProtector {
    readonly byte[] _entropy;

    public DataProtector(byte[] entropy) {
      _entropy = entropy;
    }

    public byte[] Protect(byte[] userData) {
      return userData == null
        ? null
        : ProtectedData.Protect(userData, _entropy, DataProtectionScope.LocalMachine);
    }

    public byte[] Unprotect(byte[] cypher) {
      return cypher == null
        ? null
        : ProtectedData.Unprotect(cypher, _entropy, DataProtectionScope.LocalMachine);
    }
  }
}