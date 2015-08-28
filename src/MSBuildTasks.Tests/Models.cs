using FluentValidation;
using FluentValidation.Results;

namespace DavidLievrouw.MSBuildTasks {
  public class Models {
    public static readonly ValidationResult ValidationResultSuccess = new ValidationResult();
    public static readonly ValidationResult ValidationResultFailure = new ValidationResult(new[] {new ValidationFailure("FakeProp", "FakeError")});
    public static readonly ValidationException ValidationException = new ValidationException(ValidationResultFailure.Errors);
  }
}