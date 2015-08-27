using System.Linq;
using System.Text;
using NUnit.Framework;

namespace DavidLievrouw.MSBuildTasks.Crypto {
  [TestFixture]
  public class EntropyCreatorTests {
    EntropyCreator _sut;

    [SetUp]
    public void SetUp() {
      _sut = new EntropyCreator();
    }

    [Test]
    public void ConstructorTests() {
      Assert.That(_sut.NoDependenciesAreOptional());
    }

    [Test]
    public void GivenNullPurposes_ReturnsEmptyEntropy() {
      var expected = new byte[] {};
      var actual = _sut.CreateEntropy(null);
      Assert.That(actual.SequenceEqual(expected));
    }

    [Test]
    public void GivenEmptyPurposes_ReturnsEmptyEntropy() {
      var expected = new byte[] {};
      var actual = _sut.CreateEntropy(new[] {null, "", " "});
      Assert.That(actual.SequenceEqual(expected));
    }

    [Test]
    public void IgnoresNullEmptyAndWhitespacePurposes() {
      var expected = CalculateEntropyLocally("David;Lievrouw");
      var actual = _sut.CreateEntropy(new[] {null, "David", "", " ", "Lievrouw", null});
      Assert.That(actual.SequenceEqual(expected));
    }

    [Test]
    public void GivenValidPurposes_ReturnsEntropy() {
      var expected = CalculateEntropyLocally("David;Lievrouw");
      var actual = _sut.CreateEntropy(new[] {"David", "Lievrouw"});
      Assert.That(actual.SequenceEqual(expected));
    }

    static byte[] CalculateEntropyLocally(string entropyString) {
      return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(entropyString)).ToArray();
    }
  }
}