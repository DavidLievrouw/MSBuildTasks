using System;
using FakeItEasy;
using Microsoft.Build.Framework;
using NUnit.Framework;

namespace DavidLievrouw.MSBuildTasks {
  [TestFixture]
  public class GetVersionPartsTests {
    ITaskLogger _taskLogger;
    GetVersionParts _sut;

    [SetUp]
    public void SetUp() {
      _sut = new GetVersionParts();

      _taskLogger = _taskLogger.Fake();
      _sut.Logger = _taskLogger;
    }

    [Test]
    public void WhenNoVersionIsGiven_Throws() {
      Assert.Throws<InvalidOperationException>(() => _sut.Execute());
    }

    [Test]
    public void WhenEmptyVersionIsGiven_Throws() {
      _sut.VersionNumber = string.Empty;
      Assert.Throws<InvalidOperationException>(() => _sut.Execute());
    }

    [Test]
    public void WhenWhitespaceVersionIsGiven_Throws() {
      _sut.VersionNumber = " ";
      Assert.Throws<InvalidOperationException>(() => _sut.Execute());
    }

    [Test]
    public void WhenGivenInputIsNotAVersion_Throws() {
      _sut.VersionNumber = "This_is_not_a_version_number!";
      Assert.Throws<ArgumentException>(() => _sut.Execute());
    }

    [Test]
    public void WhenGivenInputIsValid_ExtractsVersionParts() {
      _sut.VersionNumber = "14.2.3.18224";
      var actual = _sut.Execute();

      Assert.That(actual, Is.True);
      Assert.That(_sut.MajorVersion, Is.EqualTo(14));
      Assert.That(_sut.MinorVersion, Is.EqualTo(2));
      Assert.That(_sut.BuildVersion, Is.EqualTo(3));
      Assert.That(_sut.RevisionVersion, Is.EqualTo(18224));
    }

    [Test]
    public void WhenGivenInputIsValid_ShouldLogVersionNumberParts() {
      _sut.VersionNumber = "14.2.3.18224";
      _sut.Execute();

      A.CallTo(() => _taskLogger.LogMessage(MessageImportance.High, "Major: 14")).MustHaveHappened();
      A.CallTo(() => _taskLogger.LogMessage(MessageImportance.High, "Minor: 2")).MustHaveHappened();
      A.CallTo(() => _taskLogger.LogMessage(MessageImportance.High, "Build: 3")).MustHaveHappened();
      A.CallTo(() => _taskLogger.LogMessage(MessageImportance.High, "Revision: 18224")).MustHaveHappened();
    }
  }
}