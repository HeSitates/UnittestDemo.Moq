// Based on: https://github.com/Moq/moq4/wiki/Quickstart

using System.Text.RegularExpressions;
using DemoClasses.Interfaces;

namespace UnittestDemo.MoqTests;

[Parallelizable(ParallelScope.All)]
internal class MoqExampleTests
{
  [TestCase("ping", true)]
  [TestCase("pang", false)]
  public void TryParseWithDifferentValues(string value, bool expectedResult)
  {
    var iFooMock = new Mock<IFoo>();

    // out arguments
    var outString = "ack";

    // TryParse will return true, and the out argument will return "ack", lazy evaluated
    iFooMock.Setup(foo => foo.TryParse("ping", out outString)).Returns(true);
    Assert.Multiple(() =>
    {
      Assert.That(iFooMock.Object.TryParse(value, out outString), Is.EqualTo(expectedResult));

      // Note that although TryParse does return false, outString is always "ack".
      Assert.That(outString, Is.EqualTo("ack"));
    });
  }

  [TestCase("reset", typeof(InvalidOperationException))]
  [TestCase("", typeof(ArgumentException))]
  public void DoSomethingThrowsExceptions(string value, Type expectedException)
  {
    var iFooMock = new Mock<IFoo>();

    // throwing when invoked with specific parameters
    iFooMock.Setup(foo => foo.DoSomething("ping")).Returns(true);
    iFooMock.Setup(foo => foo.DoSomething("reset")).Throws<InvalidOperationException>();
    iFooMock.Setup(foo => foo.DoSomething(string.Empty)).Throws(new ArgumentException("Command went wrong", nameof(value)));

    var ex = Assert.Throws(expectedException, () => iFooMock.Object.DoSomething(value));
    if (ex is not ArgumentException aex)
    {
      return;
    }

    Assert.Multiple(() =>
    {
      Assert.That(aex.Message, Does.StartWith("Command went wrong"));
      Assert.That(aex.ParamName, Is.EqualTo("value"));
    });
  }

  [TestCase("ping", true)]
  [TestCase("anything else", false)]
  public void DoSomethingDoesNotThrowExceptions(string value, bool expectedResult = false)
  {
    var iFooMock = new Mock<IFoo>();

    // throwing when invoked with specific parameters
    iFooMock.Setup(foo => foo.DoSomething("ping")).Returns(true);
    iFooMock.Setup(foo => foo.DoSomething("reset")).Throws<InvalidOperationException>();
    iFooMock.Setup(foo => foo.DoSomething(string.Empty)).Throws(new ArgumentException("Command went wrong", nameof(value)));

    Assert.That(iFooMock.Object.DoSomething(value), Is.EqualTo(expectedResult));
  }

  [Test]
  public void SimpleTestWithRefArgument()
  {
    var iFooMock = new Mock<IFoo>();

    // ref arguments
    var instance = new Bar();

    // Only matches if the ref argument to the invocation is the same instance
    // ReSharper disable once AccessToModifiedClosure
    iFooMock.Setup(foo => foo.Submit(ref instance)).Returns(true);

    Assert.That(iFooMock.Object.Submit(ref instance), Is.True);

    var instance2 = new Bar();
    Assert.That(iFooMock.Object.Submit(ref instance2), Is.False);
  }

  [Test]
  public void SimpleTestWithRefArgumentAndAnyInstance()
  {
    var iFooMock = new Mock<IFoo>();

    // ref arguments
    var instance = new Bar();

    // Only matches if the ref argument to the invocation is the same instance
    // ReSharper disable once AccessToModifiedClosure
    iFooMock.Setup(foo => foo.Submit(ref It.Ref<Bar>.IsAny)).Returns(true);

    Assert.That(iFooMock.Object.Submit(ref instance), Is.True);

    var instance2 = new Bar();
    Assert.That(iFooMock.Object.Submit(ref instance2), Is.True);
  }

  [TestCase("ping", "ping")]
  [TestCase("PING", "ping")]
  [TestCase("PANG", "pang")]
  [TestCase("", "")]
  public void DoSomethingStringyShouldReturnInputInLowerCase(string value, string expectedResult)
  {
    var iFooMock = new Mock<IFoo>();

    // access invocation arguments when returning a value
    iFooMock.Setup(x => x.DoSomethingStringy(It.IsAny<string>())).Returns((string s) => s.ToLower());

    Assert.That(iFooMock.Object.DoSomethingStringy(value), Is.EqualTo(expectedResult));
  }

  [Test]
  public void DoSomethingStringyThrowsExceptionWhenInputValueIsNull()
  {
    var iFooMock = new Mock<IFoo>();

    // access invocation arguments when returning a value
    iFooMock.Setup(x => x.DoSomethingStringy(It.IsAny<string>())).Returns((string s) => s.ToLower());

    Assert.Throws<NullReferenceException>(() => iFooMock.Object.DoSomethingStringy(null!));
  }

  [Test]
  public void LazyEvaluatingReturnValue()
  {
    var iFooMock = new Mock<IFoo>();

    // lazy evaluating return value
    var count = 1;

    // ReSharper disable once AccessToModifiedClosure
    iFooMock.Setup(foo => foo.GetCount()).Returns(() => count);

    Assert.That(iFooMock.Object.GetCount(), Is.EqualTo(1));

    count = 2;
    Assert.That(iFooMock.Object.GetCount(), Is.EqualTo(2));
  }

  [Test]
  public async Task SimpleDoSomethingAsyncTest()
  {
    var iFooMock = new Mock<IFoo>();

    // async methods (see below for more about async):
    iFooMock.Setup(foo => foo.DoSomethingAsync().Result).Returns(true);
    Assert.That(iFooMock.Object.DoSomethingAsync().Result, Is.True);

    var result = await iFooMock.Object.DoSomethingAsync();
    Assert.That(result, Is.True);
  }

  [TestCase(-1, false)]
  [TestCase(0, true)]
  [TestCase(1, false)]
  [TestCase(2, true)]
  public void TestWithMatchingFunc(int value, bool expectedResult)
  {
    var iFooMock = new Mock<IFoo>();

    // matching Func<int>, lazy evaluated
    iFooMock.Setup(foo => foo.Add(It.Is<int>(i => i % 2 == 0))).Returns(true);

    Assert.That(iFooMock.Object.Add(value), Is.EqualTo(expectedResult));
  }

  [TestCase(-1, false)]
  [TestCase(0, true)]
  [TestCase(10, true)]
  [TestCase(11, false)]
  public void TestWithMatchingRange(int value, bool expectedResult)
  {
    var iFooMock = new Mock<IFoo>();
    iFooMock.Setup(foo => foo.Add(It.IsInRange(0, 10, Moq.Range.Inclusive))).Returns(true);

    Assert.That(iFooMock.Object.Add(value), Is.EqualTo(expectedResult));
  }

  [TestCase("", null)]
  [TestCase("a", "foo")]
  [TestCase("abcd", "foo")]
  [TestCase("D", "foo")]
  [TestCase("CaB", "foo")]
  [TestCase("xYz", null)]
  public void TestWithMatchingRegEx(string value, string expectedResult)
  {
    var iFooMock = new Mock<IFoo>();
    iFooMock.Setup(x => x.DoSomethingStringy(It.IsRegex("[a-d]+", RegexOptions.IgnoreCase))).Returns("foo");

    Assert.That(iFooMock.Object.DoSomethingStringy(value), Is.EqualTo(expectedResult));
  }

  [Test]
  public void SimplePropertyMock()
  {
    var iFooMock = new Mock<IFoo>();
    iFooMock.Setup(foo => foo.Name).Returns("bar");

    Assert.That(iFooMock.Object.Name, Is.EqualTo("bar"));
  }

  [Test]
  public void RecursivePropertyMock()
  {
    var iFooMock = new Mock<IFoo>();
    iFooMock.Setup(foo => foo.Bar.Baz.Name).Returns("baz");

    var sut = iFooMock.Object;
    Assert.That(sut.Bar, Is.Not.Null);
    Assert.That(sut.Bar.Baz, Is.Not.Null);
    Assert.That(sut.Bar.Baz.Name, Is.EqualTo("baz"));
  }

  [Test]
  public void VerifyPropertyMock()
  {
    var iFooMock = new Mock<IFoo>();

    // expects an invocation to set the value to "foo"
    iFooMock.SetupSet(foo => foo.Name = "foo");

    Assert.That(iFooMock.Object.Name, Is.Null);
    iFooMock.Object.Name = "foo";
    iFooMock.VerifyAll();
    iFooMock.VerifySet(foo => foo.Name = "foo", Times.Once);
  }

  [Test]
  public void VerifyPropertyViaConstructor()
  {
    var mock = new Mock<Person>(MockBehavior.Strict, "Simpson");

    Assert.That(mock.Object.LastName, Is.Not.Null);
    Assert.That(mock.Object.LastName, Is.EqualTo("Simpson"));
    mock.Object.LastName = "foo";
    mock.VerifyAll();
    //// mock.VerifySet(p => p.LastName = "Simpson", Times.Once);
  }
}
