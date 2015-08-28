using NUnit.Framework;

namespace DavidLievrouw.MSBuildTasks.Handlers.Models.Validation {
  [TestFixture]
  public class DecryptForLocalMachineScopeRequestValidatorTests {
    DecryptForLocalMachineScopeRequestValidator _sut;

    [SetUp]
    public void SetUp() {
      _sut = new DecryptForLocalMachineScopeRequestValidator();
    }

    [Test]
    public void ConstructorTests() {
      Assert.That(_sut.NoDependenciesAreOptional());
    }

    [Test]
    public void NullValue_IsInvalid() {
      var actualResult = _sut.Validate((DecryptForLocalMachineScopeRequest) null);
      Assert.That(actualResult.IsValid, Is.False);
    }

    [Test]
    public void NullString_IsInvalid() {
      var input = new DecryptForLocalMachineScopeRequest {
        StringToDecrypt = null,
        Purposes = new[] {"My", "Purposes"}
      };
      var actualResult = _sut.Validate(input);
      Assert.That(actualResult.IsValid, Is.False);
    }

    [Test]
    public void EmptyString_IsInvalid() {
      var input = new DecryptForLocalMachineScopeRequest {
        StringToDecrypt = string.Empty,
        Purposes = new[] {"My", "Purposes"}
      };
      var actualResult = _sut.Validate(input);
      Assert.That(actualResult.IsValid, Is.False);
    }

    [Test]
    public void WhitespaceString_IsInvalid() {
      var input = new DecryptForLocalMachineScopeRequest {
        StringToDecrypt = " ",
        Purposes = new[] {"My", "Purposes"}
      };
      var actualResult = _sut.Validate(input);
      Assert.That(actualResult.IsValid, Is.False);
    }

    [Test]
    public void NullPurposes_IsValid() {
      var input = new DecryptForLocalMachineScopeRequest {
        StringToDecrypt = "{Cypher}",
        Purposes = null
      };
      var actualResult = _sut.Validate(input);
      Assert.That(actualResult.IsValid, Is.True);
    }

    [Test]
    public void EmptyPurposes_IsValid() {
      var input = new DecryptForLocalMachineScopeRequest {
        StringToDecrypt = "{Cypher}",
        Purposes = new string[] {}
      };
      var actualResult = _sut.Validate(input);
      Assert.That(actualResult.IsValid, Is.True);
    }

    [Test]
    public void FullRequest_IsValid() {
      var input = new DecryptForLocalMachineScopeRequest {
        StringToDecrypt = "{Cypher}",
        Purposes = new[] {"My", "Purposes"}
      };
      var actualResult = _sut.Validate(input);
      Assert.That(actualResult.IsValid, Is.True);
    }
  }
}