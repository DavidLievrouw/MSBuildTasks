using System.Collections.Generic;
using DavidLievrouw.MSBuildTasks.Handlers.Models;
using DavidLievrouw.Utils.Crypto;
using FakeItEasy;
using NUnit.Framework;

namespace DavidLievrouw.MSBuildTasks.Handlers {
  [TestFixture]
  public class EncryptForLocalMachineScopeQueryHandlerTests {
    ILocalMachineScopeStringEncryptor _localMachineScopeStringEncryptor;
    EncryptForLocalMachineScopeQueryHandler _sut;

    [SetUp]
    public void SetUp() {
      _localMachineScopeStringEncryptor = _localMachineScopeStringEncryptor.Fake();
      _sut = new EncryptForLocalMachineScopeQueryHandler(_localMachineScopeStringEncryptor);
    }

    [Test]
    public void ConstructorTests() {
      Assert.That(_sut.NoDependenciesAreOptional());
    }

    [Test]
    public void DelegatesControlToStringEncryptor() {
      var request = new EncryptForLocalMachineScopeRequest {
        StringToEncrypt = "The user data",
        Purposes = new[] {"My", "Purposes"}
      };
      const string expectedResult = "{Cypher}";
      ConfigureStringEncryptor_ToReturn(expectedResult);

      var actual = _sut.Handle(request).Result;

      Assert.That(actual, Is.Not.Null);
      Assert.That(actual, Is.EqualTo(expectedResult));
      A.CallTo(() => _localMachineScopeStringEncryptor.Encrypt(request.StringToEncrypt, request.Purposes))
       .MustHaveHappened();
    }

    void ConfigureStringEncryptor_ToReturn(string result) {
      A.CallTo(() => _localMachineScopeStringEncryptor.Encrypt(A<string>._, A<IEnumerable<string>>._))
       .Returns(result);
    }
  }
}