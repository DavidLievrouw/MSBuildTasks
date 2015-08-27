using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using FakeItEasy;
using Microsoft.Build.Framework;
using NUnit.Framework;

namespace DavidLievrouw.MSBuildTasks {
  [TestFixture]
  public class EncryptForLocalMachineScopeTests {
    ITaskLogger _taskLogger;
    EncryptForLocalMachineScope _sut;
    
    [SetUp]
    public void SetUp() {
      _sut = new EncryptForLocalMachineScope();

      _taskLogger = _taskLogger.Fake();
      _sut.Logger = _taskLogger;
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
    public void GivenNoPurposes_EncryptsCorrectly() {
      _sut.StringToEncrypt = "The string to encrypt!";
      _sut.Purposes = null;

      var result = _sut.Execute();
      var actual = _sut.EncryptedString;

      Assert.That(result, Is.True);
      Assert.That(actual, Is.Not.Null);
      Assert.That(string.IsNullOrWhiteSpace(actual), Is.False);
    }

    [Test]
    public void IgnoresNullEmptyOrWhitespacePurposes() {
      _sut.StringToEncrypt = "The string to encrypt!";
      _sut.Purposes = new[] {"David", null, "Lievrouw", string.Empty, " "};
      _sut.Execute();
      var actual1 = _sut.EncryptedString;

      _sut.Purposes = new[] {"David", "Lievrouw"};
      _sut.Execute();
      var actual2 = _sut.EncryptedString;

      var decryptedString1 = ManuallyDecrypt(actual1, "David;Lievrouw");
      var decryptedString2 = ManuallyDecrypt(actual2, "David;Lievrouw");

      Assert.That(decryptedString1, Is.EqualTo(decryptedString2));
    }

    [Test]
    public void GivenStringWithPurposes_Encrypts() {
      _sut.StringToEncrypt = "The string to encrypt!";
      _sut.Purposes = new[] {"David", "Lievrouw"};

      var result = _sut.Execute();
      var actual = _sut.EncryptedString;

      Assert.That(result, Is.True);
      Assert.That(actual, Is.Not.Null);
      Assert.That(string.IsNullOrWhiteSpace(actual), Is.False);
    }

    [Test]
    public void ProducesDecryptableString() {
      _sut.StringToEncrypt = "The string to encrypt!";
      _sut.Purposes = new[] {"David", "Lievrouw"};

      _sut.Execute();
      var actual = _sut.EncryptedString.Trim();

      var decryptedString = ManuallyDecrypt(actual, "David;Lievrouw");
      Assert.That(decryptedString, Is.EqualTo(_sut.StringToEncrypt));
    }

    [Test]
    public void LogsOnStartAndEnd() {
      _sut.StringToEncrypt = "The string to encrypt!";
      _sut.Execute();

      A.CallTo(() => _taskLogger.LogMessage(MessageImportance.High, "Encrypting by machine key: The string to encrypt!")).MustHaveHappened();
      A.CallTo(() => _taskLogger.LogMessage(MessageImportance.High, "Encrypted successfully.")).MustHaveHappened();
    }

    static string ManuallyDecrypt(string encryptedString, string entropyString) {
      var cypher = Convert.FromBase64String(encryptedString);
      var entropy = entropyString == null
        ? null
        : Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(entropyString)).ToArray();
      var userData = ProtectedData.Unprotect(cypher, entropy, DataProtectionScope.LocalMachine);
      var bom = Encoding.UTF8.GetPreamble();
      if (userData.Take(bom.Length).SequenceEqual(bom)) {
        userData = userData.Skip(bom.Length).ToArray();
      }
      return Encoding.UTF8.GetString(userData);
    }
  }
}