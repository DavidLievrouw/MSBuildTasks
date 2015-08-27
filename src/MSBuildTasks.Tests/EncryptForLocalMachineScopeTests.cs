using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DavidLievrouw.MSBuildTasks.Crypto;
using FakeItEasy;
using Microsoft.Build.Framework;
using NUnit.Framework;

namespace DavidLievrouw.MSBuildTasks {
  [TestFixture]
  public class EncryptForLocalMachineScopeTests {
    ITaskLogger _taskLogger;
    IEntropyCreator _entropyCreator;
    IDataProtector _dataProtector;
    IDataProtectorFactory _dataProtectorFactory;
    EncryptForLocalMachineScope _sut;

    [SetUp]
    public void SetUp() {
      _sut = new EncryptForLocalMachineScope();

      _taskLogger = _taskLogger.Fake();
      _sut.Logger = _taskLogger;
      _entropyCreator = _entropyCreator.Fake();
      _sut.EntropyCreator = _entropyCreator;
      _dataProtectorFactory = _dataProtectorFactory.Fake();
      _sut.DataProtectorFactory = _dataProtectorFactory;
      _dataProtector = _dataProtector.Fake();
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
    public void ProtectsUserData_UsingEntropy() {
      _sut.StringToEncrypt = "The string to encrypt!";

      var entropy = new byte[] {1, 2, 3};
      ConfigureEntropyCreator_ToReturn(entropy);

      ConfigureDataProtectorCreator_ToReturn(entropy, _dataProtector);

      var expectedUserData = Encoding.UTF8.GetBytes(_sut.StringToEncrypt);
      var expectedCypher = new byte[] {4, 5, 6, 7};
      ConfigureDataProtector_Protect_ToReturn(expectedUserData, expectedCypher);

      var expectedResult = Convert.ToBase64String(expectedCypher);

      var result = _sut.Execute();
      var actual = _sut.EncryptedString;

      Assert.That(result, Is.True);
      Assert.That(actual, Is.Not.Null);
      Assert.That(actual, Is.EqualTo(expectedResult));
      A.CallTo(() => _dataProtectorFactory.Create(A<byte[]>.That.IsSameSequenceAs(entropy)))
        .MustHaveHappened();
      A.CallTo(() => _dataProtector.Protect(A<byte[]>.That.IsSameSequenceAs(expectedUserData)))
        .MustHaveHappened();
    }
  
    [Test]
    public void LogsOnStartAndEnd() {
      _sut.StringToEncrypt = "The string to encrypt!";
      _sut.Execute();

      A.CallTo(() => _taskLogger.LogMessage(MessageImportance.High, "Encrypting: The string to encrypt!")).MustHaveHappened();
      A.CallTo(() => _taskLogger.LogMessage(MessageImportance.High, "Encrypted successfully.")).MustHaveHappened();
    }

    void ConfigureEntropyCreator_ToReturn(byte[] entropy) {
      A.CallTo(() => _entropyCreator.CreateEntropy(A<IEnumerable<string>>._))
       .Returns(entropy);
    }

    void ConfigureDataProtector_Protect_ToReturn(byte[] userData, byte[] cypher) {
      A.CallTo(() => _dataProtector.Protect(A<byte[]>.That.IsSameSequenceAs(userData)))
       .Returns(cypher);
    }

    void ConfigureDataProtectorCreator_ToReturn(byte[] entropy, IDataProtector dataProtector) {
      A.CallTo(() => _dataProtectorFactory.Create(A<byte[]>.That.IsSameSequenceAs(entropy)))
       .Returns(dataProtector);
    }
  }
}