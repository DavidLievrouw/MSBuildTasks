using Microsoft.Build.Framework;

namespace DavidLievrouw.MSBuildTasks {
  public interface ITaskLogger {
    void LogMessage(MessageImportance importance, string message, params object[] messageArgs);
  }
}