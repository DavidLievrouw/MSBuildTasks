using System.Collections.Generic;

namespace DavidLievrouw.MSBuildTasks.Crypto {
  public interface IEntropyCreator {
    byte[] CreateEntropy(IEnumerable<string> purposes);
  }
}