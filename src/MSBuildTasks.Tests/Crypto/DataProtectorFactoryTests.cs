using NUnit.Framework;

namespace DavidLievrouw.MSBuildTasks.Crypto {
  [TestFixture]
  public class DataProtectorFactoryTests {
    DataProtectorFactory _sut;

    [SetUp]
    public void SetUp() {
      _sut = new DataProtectorFactory();
    }

    [Test]
    public void ConstructorTests() {
      Assert.That(_sut.NoDependenciesAreOptional());
    }

    [Test]
    public void CreatesNewDataProtector() {
      var entropy = new byte[] {1, 2, 3};
      var actual = _sut.Create(entropy);

      Assert.That(actual, Is.Not.Null);
      Assert.That(actual, Is.InstanceOf<DataProtector>());
    }
  }
}