using AssertExLib;
using FakeItEasy;
using FluentValidation;
using FluentValidation.Results;
using NUnit.Framework;

namespace DavidLievrouw.MSBuildTasks.Handlers {
  [TestFixture]
  public class ValidationAwareQueryHandlerTests {
    [Test]
    public void ConstructorTests() {
      Assert.That(Extensions.NoDependenciesAreOptional<ValidationAwareQueryHandler<object,object>>(null));
    }

    [Test]
    public void Handle_ValidatesFirst_ThenPassesControlToDecoratedCommandHandler() {
      var validator = A.Fake<IValidator<object>>();
      A.CallTo(() => validator.Validate(A<object>._)).Returns(new ValidationResult(new ValidationFailure[0]));

      var decoratedHandler = A.Fake<IQueryHandler<object, object>>();
      var expected = new object();
      A.CallTo(() => decoratedHandler.Handle(A<object>._)).Returns(expected);

      var sut = new ValidationAwareQueryHandler<object, object>(validator, decoratedHandler);

      var query = new object();
      var actual = sut.Handle(query).Result;

      Assert.That(actual, Is.EqualTo(expected));
      A.CallTo(() => validator.Validate(query)).MustHaveHappened();
      A.CallTo(() => decoratedHandler.Handle(query)).MustHaveHappened();
    }

    [Test]
    public void WhenValidationFails_ThenThrows_DoesNotCallDecoratedCommandHandler() {
      var validator = A.Fake<IValidator<object>>();
      A.CallTo(() => validator.Validate(A<object>._)).Returns(new ValidationResult(new[] {new ValidationFailure("MyProperty", "Some error")}));

      var decoratedHandler = A.Fake<IQueryHandler<object, object>>();

      var sut = new ValidationAwareQueryHandler<object, object>(validator, decoratedHandler);

      var query = new object();
      AssertEx.TaskThrows<ValidationException>(() => sut.Handle(query));

      A.CallTo(() => validator.Validate(query)).MustHaveHappened();
      A.CallTo(() => decoratedHandler.Handle(A<object>._)).MustNotHaveHappened();
    }
  }
}