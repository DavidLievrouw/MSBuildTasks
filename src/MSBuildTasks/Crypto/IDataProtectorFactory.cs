namespace DavidLievrouw.MSBuildTasks.Crypto {
  public interface IDataProtectorFactory {
    IDataProtector Create(byte[] entropy);
  }
}