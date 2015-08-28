using FluentValidation;

namespace DavidLievrouw.MSBuildTasks.Handlers.Models.Validation {
  public class DecryptForLocalMachineScopeRequestValidator
    : NullAllowableValidator<DecryptForLocalMachineScopeRequest>,
      IDecryptForLocalMachineScopeRequestValidator {
    public DecryptForLocalMachineScopeRequestValidator() {
      RuleFor(request => request.StringToDecrypt)
        .NotNull()
        .WithMessage("No valid string to decrypt was specified.")
        .Must(str => !string.IsNullOrWhiteSpace(str))
        .WithMessage("No valid string to decrypt was specified.");
    }
  }
}