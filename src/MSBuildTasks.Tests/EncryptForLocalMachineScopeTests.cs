using System;
using System.Collections.Generic;
using DavidLievrouw.Utils.Crypto;
using FakeItEasy;
using Microsoft.Build.Framework;
using NUnit.Framework;

namespace DavidLievrouw.MSBuildTasks {
  [TestFixture]
  public class EncryptForLocalMachineScopeTests {
    ITaskLogger _taskLogger;
    ILocalMachineScopeStringEncryptor _localMachineScopeStringEncryptor;
    EncryptForLocalMachineScope _sut;

    [SetUp]
    public void SetUp() {
      _sut = new EncryptForLocalMachineScope();

      _taskLogger = _taskLogger.Fake();
      _sut.Logger = _taskLogger;
      _localMachineScopeStringEncryptor = _localMachineScopeStringEncryptor.Fake();
      _sut.LocalMachineScopeStringEncryptor = _localMachineScopeStringEncryptor;
    }

    [Test]
    public void WhenNoInputStringIsGiven_Throws() {
      Assert.Throws<InvalidOperationException>(() => _sut.Execute());
    }

    [Test]
    public void WhenEmptyInputStringIsGiven_Throws() {
      _sut.StringToEncrypt = string.Empty;
      Assert.Throws<InvalidOperationException>(() => _sut.Execute());
    }

    [Test]
    public void WhenWhitespaceInputStringIsGiven_Throws() {
      _sut.StringToEncrypt = " ";
      Assert.Throws<InvalidOperationException>(() => _sut.Execute());
    }

    [Test]
    public void EncryptsUserData() {
      _sut.StringToEncrypt = "The string to encrypt!";
      _sut.Purposes = new[] {"My", "Purposes"};

      const string expectedResult = "{EncryptedString}";
      ConfigureLocalMachineScopeStringEncryptor_ToReturn(expectedResult);

      var result = _sut.Execute();
      var actual = _sut.EncryptedString;

      Assert.That(result, Is.True);
      Assert.That(actual, Is.Not.Null);
      Assert.That(actual, Is.EqualTo(expectedResult));
      A.CallTo(() => _localMachineScopeStringEncryptor.Encrypt(_sut.StringToEncrypt, _sut.Purposes))
       .MustHaveHappened();
    }

    [Test]
    public void LogsOnStartAndEnd() {
      _sut.StringToEncrypt = "The string to encrypt!";
      _sut.Execute();

      A.CallTo(() => _taskLogger.LogMessage(MessageImportance.High, "Encrypting: The string to encrypt!")).MustHaveHappened();
      A.CallTo(() => _taskLogger.LogMessage(MessageImportance.High, "Encrypted successfully.")).MustHaveHappened();
    }

    void ConfigureLocalMachineScopeStringEncryptor_ToReturn(string result) {
      A.CallTo(() => _localMachineScopeStringEncryptor.Encrypt(A<string>._, A<IEnumerable<string>>._))
       .Returns(result);
    }
  }
}