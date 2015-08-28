using System;
using System.Threading.Tasks;
using DavidLievrouw.Utils;
using FluentValidation;

namespace DavidLievrouw.MSBuildTasks.Handlers {
  public class ValidationAwareCommandHandler<TCommand> : ICommandHandler<TCommand> {
    readonly ICommandHandler<TCommand> _innerHandler;
    readonly IValidator<TCommand> _validator;

    public ValidationAwareCommandHandler(IValidator<TCommand> validator, ICommandHandler<TCommand> innerHandler) {
      if (validator == null) throw new ArgumentNullException("validator");
      if (innerHandler == null) throw new ArgumentNullException("innerHandler");
      _validator = validator;
      _innerHandler = innerHandler;
    }

    public async Task Handle(TCommand command) {
      _validator.ValidateAndThrow(command);
      await _innerHandler.Handle(command);
    }
  }
}