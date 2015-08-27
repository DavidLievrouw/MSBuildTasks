using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using FakeItEasy;
using Microsoft.Build.Framework;
using NUnit.Framework;

namespace DavidLievrouw.MSBuildTasks {
  [TestFixture]
  public class DecryptForLocalMachineScopeTests {
    ITaskLogger _taskLogger;
    DecryptForLocalMachineScope _sut;

    [SetUp]
    public void SetUp() {
      _sut = new DecryptForLocalMachineScope();

      _taskLogger = _taskLogger.Fake();
      _sut.Logger = _taskLogger;
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
    public void GivenNoPurposes_EncryptsCorrectly() {
      const string expected = "The decrypted string!!";
      var encryptedString = ManuallyEncrypt(expected);
      _sut.StringToDecrypt = encryptedString;
      _sut.Purposes = null;

      var result = _sut.Execute();
      var actual = _sut.DecryptedString;

      Assert.That(result, Is.True);
      Assert.That(actual, Is.Not.Null);
      Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void IgnoresNullEmptyOrWhitespacePurposes() {
      const string expected = "The decrypted string!!";
      var encryptedString = ManuallyEncrypt(expected, "David;Lievrouw");
      _sut.StringToDecrypt = encryptedString;

      _sut.Purposes = new[] {"David", null, "Lievrouw", string.Empty, " "};
      _sut.Execute();
      var actual1 = _sut.DecryptedString;
      _sut.Purposes = new[] {"David", "Lievrouw"};
      _sut.Execute();
      var actual2 = _sut.DecryptedString;

      Assert.That(actual1, Is.EqualTo(actual2));
    }

    [Test]
    public void GivenStringWithPurposes_Encrypts() {
      const string expected = "The decrypted string!!";
      var encryptedString = ManuallyEncrypt(expected, "David;Lievrouw");
      _sut.StringToDecrypt = encryptedString;
      _sut.Purposes = new[] {"David", "Lievrouw"};

      var result = _sut.Execute();
      var actual = _sut.DecryptedString;

      Assert.That(result, Is.True);
      Assert.That(actual, Is.Not.Null);
      Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void LogsOnStartAndEnd() {
      const string expected = "The decrypted string!!";
      var encryptedString = ManuallyEncrypt(expected, "David;Lievrouw");
      _sut.StringToDecrypt = encryptedString;
      _sut.Purposes = new[] {"David", "Lievrouw"};

      _sut.Execute();

      A.CallTo(() => _taskLogger.LogMessage(MessageImportance.High, "Decrypting: " + encryptedString)).MustHaveHappened();
      A.CallTo(() => _taskLogger.LogMessage(MessageImportance.High, "Decrypted successfully.")).MustHaveHappened();
    }

    static string ManuallyEncrypt(string stringtoEncrypt, string entropyString = null) {
      var userData = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(stringtoEncrypt)).ToArray();
      entropyString = (entropyString ?? string.Empty).Trim();
      var entropy = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(entropyString)).ToArray();
      var cypher = ProtectedData.Protect(userData, entropy, DataProtectionScope.LocalMachine);
      return Convert.ToBase64String(cypher);
    }
  }
}