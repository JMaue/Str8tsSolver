using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Str8tsSolverTest
{
  internal class ExtremeTests : BoardTestBase
  {
    [SetUp]
    public void Setup()
    {
      base.Setup();
    }
  
    public static IEnumerable<string> GetExtremeTestFiles()
    {
      //return Directory.GetFiles(@"..\\..\\..\\WeeklyExtremes\", "board_01.txt");
      return Directory.GetFiles(@"..\\..\\..\\WeeklyExtremes\", "board_*.txt");
    }

    public static IEnumerable<string> GetTestFiles()
    {
      return Directory.GetFiles(@"..\\..\\..\\Samples_derwesten\", "board_*.txt");
    }

    [Test]
    [TestCaseSource(nameof(GetTestFiles))]
    public void SolveBoards(string file)
    {
      var board = LoadBoardFromFile(file);
      var solved = Solve(board);
      Assert.IsTrue(solved);
    }

    [Test]
    [TestCaseSource(nameof(GetExtremeTestFiles))]
    public void SolveExtremeBoards(string file)
    {
      var board = LoadBoardFromFile(file);
      var solved = Solve(board);
      Assert.IsTrue(solved);
    }
  }
}
