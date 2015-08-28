using System.Collections.Generic;

namespace DavidLievrouw.MSBuildTasks.Handlers.Models {
  public class EncryptForLocalMachineScopeRequest {
    public string StringToEncrypt { get; set; }
    public IEnumerable<string> Purposes { get; set; }
  }
}