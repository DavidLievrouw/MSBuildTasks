namespace DavidLievrouw.MSBuildTasks.Crypto {
  public interface IDataProtector {
    byte[] Protect(byte[] userData);
    byte[] Unprotect(byte[] userData);
  }
}