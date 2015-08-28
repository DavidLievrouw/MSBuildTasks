using System;
using System.Text;
using DavidLievrouw.MSBuildTasks.Handlers;
using DavidLievrouw.MSBuildTasks.Handlers.Models;
using DavidLievrouw.Utils;
using FakeItEasy;
using FluentValidation;
using Microsoft.Build.Framework;
using NUnit.Framework;

namespace DavidLievrouw.MSBuildTasks {
  [TestFixture]
  public class DecryptForLocalMachineScopeTests {
    ITaskLogger _taskLogger;
    IQueryHandler<DecryptForLocalMachineScopeRequest, string> _decryptForLocalMachineScopeQueryHandler;
    DecryptForLocalMachineScope _sut;

    [SetUp]
    public void SetUp() {
      _sut = new DecryptForLocalMachineScope();

      _taskLogger = _taskLogger.Fake();
      _sut.Logger = _taskLogger;
      _decryptForLocalMachineScopeQueryHandler = _decryptForLocalMachineScopeQueryHandler.Fake();
      _sut.DecryptForLocalMachineScopeQueryHandler = _decryptForLocalMachineScopeQueryHandler;
    }

    [Test]
    public void UsesLocalDefault_WhenNoOverrideOfTheQueryHandlerIsProvided() {
      var actual = new DecryptForLocalMachineScope().DecryptForLocalMachineScopeQueryHandler;
      Assert.That(actual, Is.Not.Null);
      Assert.That(actual, Is.InstanceOf<ValidationAwareQueryHandler<DecryptForLocalMachineScopeRequest, string>>());
    }

    [Test]
    public void WhenValidationFails_Throws() {
      A.CallTo(() => _decryptForLocalMachineScopeQueryHandler.Handle(A<DecryptForLocalMachineScopeRequest>._))
       .Throws(Models.ValidationException);
      Assert.Throws<ValidationException>(() => _sut.Execute());
    }

    [Test]
    public void DecryptsUserData() {
      _sut.StringToDecrypt = "{EncryptedString}";
      _sut.Purposes = new[] {"My", "Purposes"};
      var expectedRequest = new DecryptForLocalMachineScopeRequest {
        StringToDecrypt = _sut.StringToDecrypt,
        Purposes = _sut.Purposes
      };

      const string expectedResult = "The decrypted string!";
      ConfigureQueryHandler_ToReturn(expectedResult);

      var result = _sut.Execute();
      var actual = _sut.DecryptedString;

      Assert.That(result, Is.True);
      Assert.That(actual, Is.Not.Null);
      Assert.That(actual, Is.EqualTo(expectedResult));
      A.CallTo(() => _decryptForLocalMachineScopeQueryHandler.Handle(
        A<DecryptForLocalMachineScopeRequest>.That.Matches(req => req.HasSamePropertyValuesAs(expectedRequest))))
       .MustHaveHappened();
    }

    [Test]
    public void GivenNoInputString_LogsOnStart() {
      _sut.StringToDecrypt = null;
      _sut.Execute();
      A.CallTo(() => _taskLogger.LogMessage(MessageImportance.High, "Decrypting: [NULL]")).MustHaveHappened();
    }

    [Test]
    public void LogsOnStartAndEnd() {
      _sut.StringToDecrypt = Convert.ToBase64String(Encoding.UTF8.GetBytes("CYPHERSTRING!"));
      _sut.Execute();

      A.CallTo(() => _taskLogger.LogMessage(MessageImportance.High, "Decrypting: " + _sut.StringToDecrypt)).MustHaveHappened();
      A.CallTo(() => _taskLogger.LogMessage(MessageImportance.High, "Decrypted successfully.")).MustHaveHappened();
    }

    void ConfigureQueryHandler_ToReturn(string result) {
      A.CallTo(() => _decryptForLocalMachineScopeQueryHandler.Handle(A<DecryptForLocalMachineScopeRequest>._))
       .Returns(result);
    }
  }
}