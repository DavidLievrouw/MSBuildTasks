using NUnit.Framework;

namespace DavidLievrouw.MSBuildTasks.Handlers.Models.Validation {
  [TestFixture]
  public class NullAllowableValidatorTests {
    public class DefaultNullAllowableValidatorTests : NullAllowableValidatorTests {
      DefaultNullAllowableValidator _sut;

      [SetUp]
      public void SetUp() {
        _sut = new DefaultNullAllowableValidator();
      }

      [Test]
      public void NonNullValue_IsValid() {
        var someValidObject = new object();
        var actualResult = _sut.Validate(someValidObject);
        Assert.That(actualResult.IsValid, Is.True);
      }

      [Test]
      public void NullValue_IsInvalid() {
        object nullObj = null;
        var actualResult = _sut.Validate(nullObj);
        Assert.That(actualResult.IsValid, Is.False);
      }

      class DefaultNullAllowableValidator : NullAllowableValidator<object> {}
    }

    public class NullAllowed_IsTrue_ValidatorTests : NullAllowableValidatorTests {
      FakeNullAllowableValidator _sut;

      [SetUp]
      public void SetUp() {
        _sut = new FakeNullAllowableValidator();
      }

      [Test]
      public void NonNullValue_IsValid() {
        var someValidObject = new object();
        var actualResult = _sut.Validate(someValidObject);
        Assert.That(actualResult.IsValid, Is.True);
      }

      [Test]
      public void NullValue_IsValid() {
        object nullObj = null;
        var actualResult = _sut.Validate(nullObj);
        Assert.That(actualResult.IsValid, Is.True);
      }

      class FakeNullAllowableValidator : NullAllowableValidator<object> {
        public FakeNullAllowableValidator() {
          IsNullAllowed = true;
        }
      }
    }

    public class NullAllowed_IsFalse_ValidatorTests : NullAllowableValidatorTests {
      FakeNullDisallowedValidator _sut;

      [SetUp]
      public void SetUp() {
        _sut = new FakeNullDisallowedValidator();
      }

      [Test]
      public void NonNullValue_IsValid() {
        var someValidObject = new object();
        var actualResult = _sut.Validate(someValidObject);
        Assert.That(actualResult.IsValid, Is.True);
      }

      [Test]
      public void NullValue_IsInvalid() {
        object nullObj = null;
        var actualResult = _sut.Validate(nullObj);
        Assert.That(actualResult.IsValid, Is.False);
      }

      class FakeNullDisallowedValidator : NullAllowableValidator<object> {
        public FakeNullDisallowedValidator() {
          IsNullAllowed = false;
        }
      }
    }
  }
}