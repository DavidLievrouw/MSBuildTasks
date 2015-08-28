using System;
using System.Threading.Tasks;
using DavidLievrouw.Utils;
using FluentValidation;

namespace DavidLievrouw.MSBuildTasks.Handlers {
  public class ValidationAwareQueryHandler<TArg, TResult> : IQueryHandler<TArg, TResult> {
    readonly IValidator<TArg> _validator;
    readonly IQueryHandler<TArg, TResult> _innerQueryHandler;

    public ValidationAwareQueryHandler(IValidator<TArg> validator, IQueryHandler<TArg, TResult> innerQueryHandler) {
      if (validator == null) throw new ArgumentNullException("validator");
      if (innerQueryHandler == null) throw new ArgumentNullException("innerQueryHandler");
      _validator = validator;
      _innerQueryHandler = innerQueryHandler;
    }

    public async Task<TResult> Handle(TArg query) {
      _validator.ValidateAndThrow(query);
      return await _innerQueryHandler.Handle(query);
    }
  }
}