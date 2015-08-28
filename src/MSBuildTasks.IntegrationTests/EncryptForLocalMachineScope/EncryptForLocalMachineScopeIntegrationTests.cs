using System;
using System.Collections.Generic;
using System.Text;
using DavidLievrouw.Utils.Crypto;
using FakeItEasy;
using NUnit.Framework;

namespace DavidLievrouw.MSBuildTasks {
  [TestFixture, Category("Integration")]
  public class EncryptForLocalMachineScopeIntegrationTests {
    ITaskLogger _taskLogger;
    EncryptForLocalMachineScope _sut;

    [SetUp]
    public void SetUp() {
      _sut = new EncryptForLocalMachineScope();

      _taskLogger = A.Fake<ITaskLogger>();
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
      _sut.StringToEncrypt = "The string to encrypt! $µ£";
      _sut.Purposes = null;

      var result = _sut.Execute();
      var actual = _sut.EncryptedString;

      Assert.That(result, Is.True);
      Assert.That(actual, Is.Not.Null);
      Assert.That(string.IsNullOrWhiteSpace(actual), Is.False);
    }

    [Test]
    public void IgnoresNullEmptyOrWhitespacePurposes() {
      _sut.StringToEncrypt = "The string to encrypt! $µ£";
      _sut.Purposes = new[] {"David", null, "Lievrouw", string.Empty, " "};
      _sut.Execute();
      var actual1 = _sut.EncryptedString;

      _sut.Purposes = new[] {"David", "Lievrouw"};
      _sut.Execute();
      var actual2 = _sut.EncryptedString;

      var decryptedString1 = ManuallyDecrypt(actual1, new[] { "David", "Lievrouw" });
      var decryptedString2 = ManuallyDecrypt(actual2, new[] { "David", "Lievrouw" });

      Assert.That(decryptedString1, Is.EqualTo(decryptedString2));
    }

    [Test]
    public void GivenStringWithPurposes_Encrypts() {
      _sut.StringToEncrypt = "The string to encrypt! $µ£";
      _sut.Purposes = new[] {"David", "Lievrouw"};

      var result = _sut.Execute();
      var actual = _sut.EncryptedString;

      Assert.That(result, Is.True);
      Assert.That(actual, Is.Not.Null);
      Assert.That(string.IsNullOrWhiteSpace(actual), Is.False);
    }

    [Test]
    public void ProducesDecryptableString() {
      _sut.StringToEncrypt = "The string to encrypt! $µ£";
      _sut.Purposes = new[] {"David", "Lievrouw"};

      _sut.Execute();
      var actual = _sut.EncryptedString.Trim();

      var decryptedString = ManuallyDecrypt(actual, new[] { "David", "Lievrouw" });
      Assert.That(decryptedString, Is.EqualTo(_sut.StringToEncrypt));
    }

    static string ManuallyDecrypt(string encryptedString, IEnumerable<string> purposes = null) {
      var entropyCreator = new EntropyCreator();
      var entropy = entropyCreator.CreateEntropy(purposes);
      var protector = new DataProtector(entropy);

      var cypher = Convert.FromBase64String(encryptedString);
      var userData = protector.Unprotect(cypher);

      return Encoding.UTF8.GetString(userData);
    }
  }
}