using System.Threading.Tasks;

namespace DavidLievrouw.MSBuildTasks.Handlers {
  public interface ICommandHandler<in TCommand> {
    Task Handle(TCommand command);
  }
}