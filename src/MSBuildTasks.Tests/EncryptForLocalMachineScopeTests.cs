using DavidLievrouw.MSBuildTasks.Handlers;
using DavidLievrouw.MSBuildTasks.Handlers.Models;
using DavidLievrouw.Utils;
using FakeItEasy;
using FluentValidation;
using Microsoft.Build.Framework;
using NUnit.Framework;

namespace DavidLievrouw.MSBuildTasks {
  [TestFixture]
  public class EncryptForLocalMachineScopeTests {
    ITaskLogger _taskLogger;
    IQueryHandler<EncryptForLocalMachineScopeRequest, string> _encryptForLocalMachineScopeQueryHandler;
    EncryptForLocalMachineScope _sut;

    [SetUp]
    public void SetUp() {
      _sut = new EncryptForLocalMachineScope();

      _taskLogger = _taskLogger.Fake();
      _sut.Logger = _taskLogger;
      _encryptForLocalMachineScopeQueryHandler = _encryptForLocalMachineScopeQueryHandler.Fake();
      _sut.EncryptForLocalMachineScopeQueryHandler = _encryptForLocalMachineScopeQueryHandler;
    }

    [Test]
    public void UsesLocalDefault_WhenNoOverrideOfTheQueryHandlerIsProvided() {
      var actual = new EncryptForLocalMachineScope().EncryptForLocalMachineScopeQueryHandler;
      Assert.That(actual, Is.Not.Null);
      Assert.That(actual, Is.InstanceOf<ValidationAwareQueryHandler<EncryptForLocalMachineScopeRequest, string>>());
    }

    [Test]
    public void WhenValidationFails_Throws() {
      A.CallTo(() => _encryptForLocalMachineScopeQueryHandler.Handle(A<EncryptForLocalMachineScopeRequest>._))
       .Throws(Models.ValidationException);
      Assert.Throws<ValidationException>(() => _sut.Execute());
    }

    [Test]
    public void EncryptsUserData() {
      _sut.StringToEncrypt = "The string to encrypt!";
      _sut.Purposes = new[] {"My", "Purposes"};
      var expectedRequest = new EncryptForLocalMachineScopeRequest {
        StringToEncrypt = _sut.StringToEncrypt,
        Purposes = _sut.Purposes
      };

      const string expectedResult = "{EncryptedString}";
      ConfigureQueryHandler_ToReturn(expectedResult);

      var result = _sut.Execute();
      var actual = _sut.EncryptedString;

      Assert.That(result, Is.True);
      Assert.That(actual, Is.Not.Null);
      Assert.That(actual, Is.EqualTo(expectedResult));
      A.CallTo(() => _encryptForLocalMachineScopeQueryHandler.Handle(
        A<EncryptForLocalMachineScopeRequest>.That.Matches(req => req.HasSamePropertyValuesAs(expectedRequest))))
       .MustHaveHappened();
    }

    [Test]
    public void GivenNoInputString_LogsOnStart() {
      _sut.StringToEncrypt = null;
      _sut.Execute();
      A.CallTo(() => _taskLogger.LogMessage(MessageImportance.High, "Encrypting: [NULL]")).MustHaveHappened();
    }

    [Test]
    public void LogsOnStartAndEnd() {
      _sut.StringToEncrypt = "The string to encrypt!";
      _sut.Execute();

      A.CallTo(() => _taskLogger.LogMessage(MessageImportance.High, "Encrypting: The string to encrypt!")).MustHaveHappened();
      A.CallTo(() => _taskLogger.LogMessage(MessageImportance.High, "Encrypted successfully.")).MustHaveHappened();
    }

    void ConfigureQueryHandler_ToReturn(string result) {
      A.CallTo(() => _encryptForLocalMachineScopeQueryHandler.Handle(A<EncryptForLocalMachineScopeRequest>._))
       .Returns(result);
    }
  }
}