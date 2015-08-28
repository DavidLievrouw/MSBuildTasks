using NUnit.Framework;

namespace DavidLievrouw.MSBuildTasks.Handlers.Models.Validation {
  [TestFixture]
  public class EncryptForLocalMachineScopeRequestValidatorTests {
    EncryptForLocalMachineScopeRequestValidator _sut;

    [SetUp]
    public void SetUp() {
      _sut = new EncryptForLocalMachineScopeRequestValidator();
    }

    [Test]
    public void ConstructorTests() {
      Assert.That(_sut.NoDependenciesAreOptional());
    }

    [Test]
    public void NullValue_IsInvalid() {
      var actualResult = _sut.Validate((EncryptForLocalMachineScopeRequest)null);
      Assert.That(actualResult.IsValid, Is.False);
    }

    [Test]
    public void NullString_IsInvalid() {
      var input = new EncryptForLocalMachineScopeRequest {
        StringToEncrypt = null,
        Purposes = new[] {"My", "Purposes"}
      };
      var actualResult = _sut.Validate(input);
      Assert.That(actualResult.IsValid, Is.False);
    }

    [Test]
    public void EmptyString_IsInvalid() {
      var input = new EncryptForLocalMachineScopeRequest {
        StringToEncrypt = string.Empty,
        Purposes = new[] {"My", "Purposes"}
      };
      var actualResult = _sut.Validate(input);
      Assert.That(actualResult.IsValid, Is.False);
    }

    [Test]
    public void WhitespaceString_IsValid() {
      var input = new EncryptForLocalMachineScopeRequest {
        StringToEncrypt = " ",
        Purposes = new[] {"My", "Purposes"}
      };
      var actualResult = _sut.Validate(input);
      Assert.That(actualResult.IsValid, Is.True);
    }

    [Test]
    public void NullPurposes_IsValid() {
      var input = new EncryptForLocalMachineScopeRequest {
        StringToEncrypt = "UserData",
        Purposes = null
      };
      var actualResult = _sut.Validate(input);
      Assert.That(actualResult.IsValid, Is.True);
    }

    [Test]
    public void EmptyPurposes_IsValid() {
      var input = new EncryptForLocalMachineScopeRequest {
        StringToEncrypt = "UserData",
        Purposes = new string[] {}
      };
      var actualResult = _sut.Validate(input);
      Assert.That(actualResult.IsValid, Is.True);
    }

    [Test]
    public void FullRequest_IsValid() {
      var input = new EncryptForLocalMachineScopeRequest {
        StringToEncrypt = "UserData",
        Purposes = new[] {"My", "Purposes"}
      };
      var actualResult = _sut.Validate(input);
      Assert.That(actualResult.IsValid, Is.True);
    }
  }
}