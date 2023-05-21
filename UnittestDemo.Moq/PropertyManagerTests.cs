using DemoClasses.Interfaces;

namespace UnittestDemo.MoqTests;

internal class PropertyManagerTests
{
  [Test]
  public void StupPropertyWithSetupGet()
  {
    var mock = new Mock<IPropertyManager>();
    var nameUser = new PropertyManagerConsumer(mock.Object);
    mock.SetupGet(m => m.FirstName).Returns("Someone");
    mock.SetupGet(m => m.LastName).Returns("Nice");

    var name = nameUser.GetName();

    Assert.That(name, Is.EqualTo("Someone Nice"));
  }

  [Test]
  public void VerifyPropertyWithVerifyGet()
  {
    var mock = new Mock<IPropertyManager>();
    var nameUser = new PropertyManagerConsumer(mock.Object);

    nameUser.GetName();

    mock.VerifyGet(m => m.FirstName, Times.Once);
  }

  [Test]
  public void VerifyPropertyIsSet_WithSpecificValue_WithSetupSet()
  {
    var mock = new Mock<IPropertyManager>();
    var nameUser = new PropertyManagerConsumer(mock.Object);

    mock.SetupSet(m => m.FirstName = "Bart").Verifiable();

    nameUser.ChangeNames("Bart", "Simpson");

    mock.Verify();
  }

  [Test]
  public void VerifyPropertyIsSet_WithSpecificValue_WithVerifySet()
  {
    var mock = new Mock<IPropertyManager>();
    var nameUser = new PropertyManagerConsumer(mock.Object);

    nameUser.ChangeNames("Homer", "Simpson");

    mock.VerifySet(m => m.FirstName = "Homer");
  }

  [Test]
  public void Verify()
  {
    var mock = new Mock<IPropertyManager>();
    var nameUser = new PropertyManagerConsumer(mock.Object);

    nameUser.ChangeFirstName("Homer");

    // we are verifying that ChangeFirstName sends the correct string to ChangeFirstName
    mock.Verify(m => m.ChangeFirstName(It.Is<string>(a => a == "Homer")), Times.Once);
  }

  [Test]
  public void TrackPropertyWithSetUpProperty()
  {
    var mock = new Mock<IPropertyManager>();

    mock.SetupProperty(m => m.FirstName);
    mock.Object.FirstName = "Ni!";

    Assert.That(mock.Object.FirstName, Is.EqualTo("Ni!"));

    mock.Object.FirstName = "der wechselnden";
    Assert.That(mock.Object.FirstName, Is.EqualTo("der wechselnden"));
  }

  [Test]
  public void InitializeTrackPropertyWithSetUpProperty()
  {
    var mock = new Mock<IPropertyManager>();
    _ = new PropertyManagerConsumer(mock.Object);

    mock.SetupProperty(m => m.FirstName, "ManBearPig");

    Assert.That(mock.Object.FirstName, Is.EqualTo("ManBearPig"));
  }

  [Test]
  public void InitializeTrackPropertyWithSetUpGet()
  {
    var mock = new Mock<IPropertyManager>();
    _ = new PropertyManagerConsumer(mock.Object);

    // You can't change the property later with setupGet, but with setupProperty you can.
    // mock.SetupProperty(m => m.FirstName, "Regina");
    mock.SetupGet(m => m.FirstName).Returns("Regina");

    Assert.That(mock.Object.FirstName, Is.EqualTo("Regina"));

    mock.Object.FirstName = "Floyd";
    Assert.That(mock.Object.FirstName, Is.Not.EqualTo("Floyd"));
  }

  [Test]
  public void TrackAllPropertiesWithSetupAllProperties()
  {
    var mock = new Mock<IPropertyManager>();

    // mock.SetupProperty(m => m.FirstName);

    // Comment this and uncomment SetupProperty, the assertion fails
    mock.SetupAllProperties();

    mock.Object.FirstName = "Robert";
    mock.Object.LastName = "Paulson";

    Assert.That(mock.Object.FirstName, Is.EqualTo("Robert"));
    Assert.That(mock.Object.LastName, Is.EqualTo("Paulson"));
  }
}
