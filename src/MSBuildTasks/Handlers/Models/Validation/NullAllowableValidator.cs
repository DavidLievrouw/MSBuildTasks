using FluentValidation;
using FluentValidation.Results;

namespace DavidLievrouw.MSBuildTasks.Handlers.Models.Validation {
  public abstract class NullAllowableValidator<T> : AbstractValidator<T> {
    public bool IsNullAllowed { get; set; }

    public override ValidationResult Validate(T instance) {
      if (IsNullAllowed) {
        return instance == null
          ? new ValidationResult()
          : base.Validate(instance);
      }
      return instance == null
        ? new ValidationResult(new[] {new ValidationFailure(typeof(T).Name, typeof(T).Name + " instance cannot be null.")})
        : base.Validate(instance);
    }
  }
}