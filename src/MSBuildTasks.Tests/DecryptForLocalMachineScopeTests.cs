using System;
using System.Collections.Generic;
using System.Text;
using DavidLievrouw.MSBuildTasks.Crypto;
using FakeItEasy;
using Microsoft.Build.Framework;
using NUnit.Framework;

namespace DavidLievrouw.MSBuildTasks {
  [TestFixture]
  public class DecryptForLocalMachineScopeTests {
    ITaskLogger _taskLogger;
    IEntropyCreator _entropyCreator;
    IDataProtector _dataProtector;
    IDataProtectorFactory _dataProtectorFactory;
    DecryptForLocalMachineScope _sut;

    [SetUp]
    public void SetUp() {
      _sut = new DecryptForLocalMachineScope();

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
      _sut.StringToDecrypt = string.Empty;
      Assert.Throws<InvalidOperationException>(() => _sut.Execute());
    }

    [Test]
    public void WhenWhitespaceInputStringIsGiven_Throws() {
      _sut.StringToDecrypt = " ";
      Assert.Throws<InvalidOperationException>(() => _sut.Execute());
    }

    [Test]
    public void UnprotectsUserData_UsingEntropy() {
      _sut.StringToDecrypt = Convert.ToBase64String(Encoding.UTF8.GetBytes("CYPHERSTRING!"));

      var entropy = new byte[] {1, 2, 3};
      ConfigureEntropyCreator_ToReturn(entropy);

      ConfigureDataProtectorCreator_ToReturn(entropy, _dataProtector);

      var cypherToDecrypt = Convert.FromBase64String(_sut.StringToDecrypt);
      var expectedUserData = Encoding.UTF8.GetBytes("UserDataString!");
      ConfigureDataProtector_Unprotect_ToReturn(cypherToDecrypt, expectedUserData);

      var expectedResult = Encoding.UTF8.GetString(expectedUserData);

      var result = _sut.Execute();
      var actual = _sut.DecryptedString;

      Assert.That(result, Is.True);
      Assert.That(actual, Is.Not.Null);
      Assert.That(actual, Is.EqualTo(expectedResult));
      A.CallTo(() => _dataProtectorFactory.Create(A<byte[]>.That.IsSameSequenceAs(entropy)))
       .MustHaveHappened();
      A.CallTo(() => _dataProtector.Unprotect(A<byte[]>.That.IsSameSequenceAs(cypherToDecrypt)))
       .MustHaveHappened();
    }

    [Test]
    public void LogsOnStartAndEnd() {
      _sut.StringToDecrypt = Convert.ToBase64String(Encoding.UTF8.GetBytes("CYPHERSTRING!"));
      _sut.Execute();

      A.CallTo(() => _taskLogger.LogMessage(MessageImportance.High, "Decrypting: " + _sut.StringToDecrypt)).MustHaveHappened();
      A.CallTo(() => _taskLogger.LogMessage(MessageImportance.High, "Decrypted successfully.")).MustHaveHappened();
    }

    void ConfigureEntropyCreator_ToReturn(byte[] entropy) {
      A.CallTo(() => _entropyCreator.CreateEntropy(A<IEnumerable<string>>._))
       .Returns(entropy);
    }

    void ConfigureDataProtector_Unprotect_ToReturn(byte[] cypher, byte[] userData) {
      A.CallTo(() => _dataProtector.Unprotect(A<byte[]>.That.IsSameSequenceAs(cypher)))
       .Returns(userData);
    }

    void ConfigureDataProtectorCreator_ToReturn(byte[] entropy, IDataProtector dataProtector) {
      A.CallTo(() => _dataProtectorFactory.Create(A<byte[]>.That.IsSameSequenceAs(entropy)))
       .Returns(dataProtector);
    }
  }
}