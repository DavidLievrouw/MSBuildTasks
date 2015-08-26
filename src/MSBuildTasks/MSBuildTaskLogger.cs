using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace DavidLievrouw.MSBuildTasks {
  public class MSBuildTaskLogger : ITaskLogger {
    readonly TaskLoggingHelper _log;

    public MSBuildTaskLogger(Task task) {
      if (task == null) throw new ArgumentNullException(nameof(task));
      _log = task.Log;
    }

    public void LogMessage(MessageImportance importance, string message, params object[] messageArgs) {
      _log.LogMessage(importance, message, messageArgs);
    }
  }
}