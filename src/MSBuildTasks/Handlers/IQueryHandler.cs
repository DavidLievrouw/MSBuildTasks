using System.Threading.Tasks;

namespace DavidLievrouw.MSBuildTasks.Handlers {
  public interface IQueryHandler<in TArg, TResult> {
    Task<TResult> Handle(TArg request);
  }
}