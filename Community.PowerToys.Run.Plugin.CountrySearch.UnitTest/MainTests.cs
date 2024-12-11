using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerToys_Run_CountrySearch;
using Wox.Plugin;

namespace PowerToys_Run_CountrySearch_UnitTest;

[TestClass]
public class MainTests
{
    private Main _subject = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _subject = new Main();
        _subject.InitPluginPath(Path.Join("."));
        _subject.InitDatabase(Path.Join("Resources", "countries.json"));
    }

    [TestMethod]
    public void Query_should_be_empty()
    {
        var results = _subject.Query(new Query(""));
        Assert.AreEqual(0, results.Count);
    }

    [TestMethod]
    public void Query_should_return_country_with_domain()
    {
        var results = _subject.Query(new Query("d1"));
        Assert.AreEqual(1, results.Count);
        Assert.AreEqual("id1", results[0].Title);
    }
}