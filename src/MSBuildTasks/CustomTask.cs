﻿using System;
using System.Runtime.CompilerServices;
using Microsoft.Build.Utilities;

[assembly: InternalsVisibleTo("DLMSBuildTasks.Tests")]
[assembly: InternalsVisibleTo("DLMSBuildTasks.IntegrationTests")]

namespace DavidLievrouw.MSBuildTasks {
  public abstract class CustomTask : Task {
    ITaskLogger _logger;
    static readonly object Lock = new object();

    /// <remarks>
    /// Property injection, with a local default, is needed, because MSBuild requires a default constructor.
    /// </remarks>
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