using Moq.Protected;

namespace UnittestDemo.MoqTests;

[TestFixture]
public class TestClassProtected
{
  /// <summary>
  /// This test if the CallingMethod calls the ProtectedMethod.
  /// But it does not test the working of the ProtectedMethod!
  /// </summary>
  [Test]
  public void CallingMethodShouldCall_CallsProtectedMethod()
  {
    // Arrange
    var sutMock = new Mock<MyAbstractClass>();
    sutMock.Protected()
      .Setup<string?>("ProtectedMethod", ItExpr.IsAny<string>())
      .Returns("any string")
      .Verifiable();

    // Act
    sutMock.Object.CallingMethod(new Baz());

    // Assert
    sutMock.Verify();
  }

  /// <summary>
  /// This test the working of the ProtectedMethod.
  /// But it does not test if the CallingMethod calls the ProtectedMethod!
  /// </summary>
  [TestCase(null, "")]
  [TestCase("", "")]
  [TestCase("ABC", "")]
  [TestCase("abc", "")]
  [TestCase("XYZ", "")]
  [TestCase("def", "def")]
  [TestCase("DEF", "DEF")]
  public void ProtectedFunctionShouldScrubInvalidNames(string name, string expectedValue)
  {
    //Arrange
    var sutMock = new Mock<MyAbstractClass>{ CallBase = true };

    //Act
    var result = sutMock.Object.CallingMethod(new Baz{ Name = name });

    //Assert
    Assert.That(result, Is.EqualTo(expectedValue));
  }

  [Test]
  public void DoWhenCalledCallsMyAbstractMethod()
  {
    // Arrange
    var sutMock = new Mock<MyAbstractClass>();

    // Act
    sutMock.Object.Do();

    // Assert
    sutMock.Verify(x => x.MyAbstractMethod());
  }
}
