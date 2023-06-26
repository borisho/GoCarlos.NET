using GoCarlos.NET.UI.Services;

namespace GoCarlos.NET.Tests.Services;

[TestClass]
public class EgdServiceTest
{
    private readonly EgdService _egdService;

    public EgdServiceTest()
    {
        _egdService = new EgdService();
    }

    [TestMethod]
    public void CheckConnectivity()
    {
        // Arrange

        // Act
        _egdService.SearchByPin("");

        // Assert
    }

    [TestMethod]
    public void GetJson_()
    {
        // Arrange

        // Act

        // Assert
        Assert.AreEqual(0, 0);
    }
}
