using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DavidLievrouw.MSBuildTasks.Crypto {
  public class EntropyCreator : IEntropyCreator {
    public byte[] CreateEntropy(IEnumerable<string> purposes) {
      var realPurposes = purposes == null
        ? new string[] {}
        : purposes.Where(purpose => !string.IsNullOrWhiteSpace(purpose)).ToArray();
      var entropyString = string.Join(";", realPurposes).Trim();

      return Encoding.UTF8.GetBytes(entropyString);
    }
  }
}