using Str8tsSolverImageTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Str8tsSolverTest
{
  internal class ImagingToolTests
  {
    private BoardFinder _boardFinder;
    [SetUp]
    public void Setup()
    {
      _boardFinder = new BoardFinder();
    }

    public static IEnumerable<string> GetTestFiles()
    {
      return Directory.GetFiles(@"..\\..\\..\\Samples\\", "*.*");
    }

    [Test]
    [TestCaseSource(nameof(GetTestFiles))]
    public void ContourFinderTest(string file)
    {
      var contour = _boardFinder.FindExternalContour(file);
      var count = contour.Count();
      var msg = $"File: {file}: found {count} corner points";
      if (count != 4)
         Assert.Warn(msg);
      Assert.IsTrue(count >= 4, msg);
    }

    [Test]
    [TestCaseSource(nameof(GetTestFiles))]
    public void IsScreenshotTest(string file)
    {
      var contour = _boardFinder.FindExternalContour(file);
      bool isScreenshot = _boardFinder.IsScreenShot(contour);
      Assert.AreEqual(file.Contains("ex"), isScreenshot);
    }
  }
}
