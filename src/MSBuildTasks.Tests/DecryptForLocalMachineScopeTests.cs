using System;
using System.Collections.Generic;
using System.Text;
using DavidLievrouw.Utils.Crypto;
using FakeItEasy;
using Microsoft.Build.Framework;
using NUnit.Framework;

namespace DavidLievrouw.MSBuildTasks {
  [TestFixture]
  public class DecryptForLocalMachineScopeTests {
    ITaskLogger _taskLogger;
    ILocalMachineScopeStringEncryptor _localMachineScopeStringEncryptor;
    DecryptForLocalMachineScope _sut;

    [SetUp]
    public void SetUp() {
      _sut = new DecryptForLocalMachineScope();

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
      _sut.StringToDecrypt = string.Empty;
      Assert.Throws<InvalidOperationException>(() => _sut.Execute());
    }

    [Test]
    public void WhenWhitespaceInputStringIsGiven_Throws() {
      _sut.StringToDecrypt = " ";
      Assert.Throws<InvalidOperationException>(() => _sut.Execute());
    }

    [Test]
    public void DecryptsUserData() {
      _sut.StringToDecrypt = "{EncryptedString}";
      _sut.Purposes = new[] {"My", "Purposes"};

      const string expectedResult = "The decrypted string!";
      ConfigureLocalMachineScopeStringEncryptor_ToReturn(expectedResult);

      var result = _sut.Execute();
      var actual = _sut.DecryptedString;

      Assert.That(result, Is.True);
      Assert.That(actual, Is.Not.Null);
      Assert.That(actual, Is.EqualTo(expectedResult));
      A.CallTo(() => _localMachineScopeStringEncryptor.Decrypt(_sut.StringToDecrypt, _sut.Purposes))
       .MustHaveHappened();
    }

    [Test]
    public void LogsOnStartAndEnd() {
      _sut.StringToDecrypt = Convert.ToBase64String(Encoding.UTF8.GetBytes("CYPHERSTRING!"));
      _sut.Execute();

      A.CallTo(() => _taskLogger.LogMessage(MessageImportance.High, "Decrypting: " + _sut.StringToDecrypt)).MustHaveHappened();
      A.CallTo(() => _taskLogger.LogMessage(MessageImportance.High, "Decrypted successfully.")).MustHaveHappened();
    }

    void ConfigureLocalMachineScopeStringEncryptor_ToReturn(string result) {
      A.CallTo(() => _localMachineScopeStringEncryptor.Decrypt(A<string>._, A<IEnumerable<string>>._))
       .Returns(result);
    }
  }
}