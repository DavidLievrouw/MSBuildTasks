using System.Collections.Generic;
using DavidLievrouw.MSBuildTasks.Handlers.Models;
using DavidLievrouw.Utils.Crypto;
using FakeItEasy;
using NUnit.Framework;

namespace DavidLievrouw.MSBuildTasks.Handlers {
  [TestFixture]
  public class DecryptForLocalMachineScopeQueryHandlerTests {
    ILocalMachineScopeStringEncryptor _localMachineScopeStringEncryptor;
    DecryptForLocalMachineScopeQueryHandler _sut;

    [SetUp]
    public void SetUp() {
      _localMachineScopeStringEncryptor = _localMachineScopeStringEncryptor.Fake();
      _sut = new DecryptForLocalMachineScopeQueryHandler(_localMachineScopeStringEncryptor);
    }

    [Test]
    public void ConstructorTests() {
      Assert.That(_sut.NoDependenciesAreOptional());
    }

    [Test]
    public void DelegatesControlToStringEncryptor() {
      var request = new DecryptForLocalMachineScopeRequest {
        StringToDecrypt = "{TheCypher}",
        Purposes = new[] {"My", "Purposes"}
      };
      const string expectedResult = "The user data!";
      ConfigureStringEncryptor_ToReturn(expectedResult);

      var actual = _sut.Handle(request).Result;

      Assert.That(actual, Is.Not.Null);
      Assert.That(actual, Is.EqualTo(expectedResult));
      A.CallTo(() => _localMachineScopeStringEncryptor.Decrypt(request.StringToDecrypt, request.Purposes))
       .MustHaveHappened();
    }

    void ConfigureStringEncryptor_ToReturn(string result) {
      A.CallTo(() => _localMachineScopeStringEncryptor.Decrypt(A<string>._, A<IEnumerable<string>>._))
       .Returns(result);
    }
  }
}