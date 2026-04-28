using System.Xml.Linq;
using Xunit;

namespace AgriMarket.Tests.Resources;

public class SharedResourceResxTests
{
    private static readonly string ProjectRoot = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));

    private static HashSet<string> GetResxKeys(string filePath)
    {
        var doc = XDocument.Load(filePath);
        return doc.Descendants("data")
            .Select(d => d.Attribute("name")?.Value)
            .Where(n => n != null)
            .ToHashSet()!;
    }

    [Fact]
    public void BothResxFiles_ContainSameKeys()
    {
        var enPath = Path.Combine(ProjectRoot, "AgriMarket.Resources", "SharedResource.resx");
        var etPath = Path.Combine(ProjectRoot, "AgriMarket.Resources", "SharedResource.et.resx");

        var enKeys = GetResxKeys(enPath);
        var etKeys = GetResxKeys(etPath);

        var missingInEt = enKeys.Except(etKeys).OrderBy(k => k).ToList();
        var missingInEn = etKeys.Except(enKeys).OrderBy(k => k).ToList();

        Assert.True(missingInEt.Count == 0,
            $"Keys in EN but missing in ET: {string.Join(", ", missingInEt)}");
        Assert.True(missingInEn.Count == 0,
            $"Keys in ET but missing in EN: {string.Join(", ", missingInEn)}");
    }
}
