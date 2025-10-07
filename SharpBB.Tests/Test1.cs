using System.Text.Json;

namespace SharpBB.Tests;

[TestClass]
public sealed class Test1
{
    [TestMethod]
    public void TestMethod1()
    {
        List<object> stuff =
        [
            new
            {
                From = 0,
                To = 100, 
                Tag = "something"
            }
        ]; 
        Console.WriteLine(JsonSerializer.Serialize(stuff));
    }
}