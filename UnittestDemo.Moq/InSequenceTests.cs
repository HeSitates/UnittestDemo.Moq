using DemoClasses.Interfaces;

namespace UnittestDemo.MoqTests;

[Parallelizable(ParallelScope.All)]
public class InSequenceTests
{
  [Test]
  public void TestWithRefClass()
  {
    // Arrange
    var mock = CreateMockNoSpecificValues();
    var refClass = new RefClass(mock.Object);

    // Act
    refClass.Work();

    // Assert
    mock.Verify();
  }

  [Test]
  public void TestWithTestClass()
  {
    // Arrange
    var mock = CreateMockNoSpecificValues();
    var testClass = new TestClass(mock.Object);

    // Act
    testClass.Work();

    // Assert
    mock.Verify();
  }

  [Test]
  public void TestWithTestClassWithSpecificValues()
  {
    // Arrange
    var mock = CreateMockWithSpecificValues();
    var testClass = new TestClass(mock.Object);

    // Act
    testClass.Work();

    // Assert
    mock.Verify();
  }

  private static Mock<ITarget> CreateMockNoSpecificValues()
  {
    var mock = new Mock<ITarget>(MockBehavior.Strict);
    var seq = new MockSequence();
    mock.InSequence(seq).Setup(m => m.A(It.IsAny<string>()));
    mock.InSequence(seq).Setup(m => m.A(It.IsAny<string>()));
    mock.InSequence(seq).Setup(m => m.B(It.IsAny<int>()));
    mock.InSequence(seq).Setup(m => m.B(It.IsAny<int>()));
    return mock;
  }

  private static Mock<ITarget> CreateMockWithSpecificValues()
  {
    var mock = new Mock<ITarget>(MockBehavior.Strict);
    var seq = new MockSequence();
    mock.InSequence(seq).Setup(m => m.A("A"));
    mock.InSequence(seq).Setup(m => m.A("B"));
    mock.InSequence(seq).Setup(m => m.B(1));
    mock.InSequence(seq).Setup(m => m.B(0));
    return mock;
  }

  private class RefClass
  {
    private readonly ITarget _target;

    public RefClass(ITarget target)
    {
      _target = target;
    }

    public void Work()
    {
      _target.A("A");
      _target.A("B");
      _target.B(0);
      _target.B(1);
    }
  }

  private class TestClass
  {
    private readonly ITarget _target;

    public TestClass(ITarget target)
    {
      _target = target;
    }

    public void Work()
    {
      _target.A("A");
      _target.A("B");
      _target.B(1);
      _target.B(0);
    }
  }
}
