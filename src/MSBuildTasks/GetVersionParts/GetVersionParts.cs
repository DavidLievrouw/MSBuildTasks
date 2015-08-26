using System;
using System.Runtime.CompilerServices;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

[assembly: InternalsVisibleTo("DLMSBuildTasks.Tests")]

namespace DavidLievrouw.MSBuildTasks.GetVersionParts {
  public class GetVersionParts : Task {
    ITaskLogger _logger;
    static readonly object Lock = new object();

    public override bool Execute() {
      if (string.IsNullOrWhiteSpace(VersionNumber)) throw new InvalidOperationException("No version number is defined.");

      Logger.LogMessage(MessageImportance.High, "Getting version details of version number: " + VersionNumber + "...");

      var v = Version.Parse(VersionNumber);

      MajorVersion = v.Major;
      MinorVersion = v.Minor;
      BuildVersion = v.Build;
      RevisionVersion = v.Revision;

      Logger.LogMessage(MessageImportance.High, "Major: " + MajorVersion);
      Logger.LogMessage(MessageImportance.High, "Minor: " + MinorVersion);
      Logger.LogMessage(MessageImportance.High, "Build: " + BuildVersion);
      Logger.LogMessage(MessageImportance.High, "Revision: " + RevisionVersion);

      return true;
    }

    [Required]
    public string VersionNumber { get; set; }

    [Output]
    public int MajorVersion { get; set; }

    [Output]
    public int MinorVersion { get; set; }

    [Output]
    public int BuildVersion { get; set; }

    [Output]
    public int RevisionVersion { get; set; }

    public ITaskLogger Logger {
      get {
        lock (Lock) {
          return _logger ?? (_logger = new MSBuildTaskLogger(this));
        }
      }
      internal set {
        lock (Lock) {
          if (value == null) throw new ArgumentNullException("value");
          _logger = value;
        }
      }
    }
  }
}