using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DavidLievrouw.MSBuildTasks.Crypto {
  public class DataProtector : IDataProtector {
    readonly byte[] _entropy;
    static readonly byte[] UTF8Preamble = Encoding.UTF8.GetPreamble();

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
        : StripPreamble(ProtectedData.Unprotect(cypher, _entropy, DataProtectionScope.LocalMachine));
    }

    static byte[] StripPreamble(byte[] userData) {
      if (userData == null) return null;
      if (userData.Take(UTF8Preamble.Length).SequenceEqual(UTF8Preamble)) {
        userData = userData.Skip(UTF8Preamble.Length).ToArray();
      }
      return userData;
    }
  }
}