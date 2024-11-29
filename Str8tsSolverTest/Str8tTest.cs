using System.Collections.Generic;
using Str8tsSolverLib;
using NUnit.Framework;

namespace Str8tsSolverTest
{
  public class Str8tTests
  {
    private char[,] _board = new char[,]
      { // devilish, 
        { '1', ' ', ' ', '#', ' ', ' ', ' ', ' ', ' ' },
        { '#', ' ', ' ', ' ', '2', '#', ' ', ' ', '#' },
        { '#', '9', ' ', ' ', ' ', '5', '#', ' ', ' ' },
        { ' ', ' ', ' ', ' ', ' ', ' ', '3', ' ', '#' },
        { 'G', '#', '6', '3', ' ', ' ', ' ', '#', '#' },
        { '#', ' ', '8', ' ', ' ', ' ', ' ', ' ', ' ' },
        { ' ', ' ', ' ', ' ', '#', ' ', ' ', ' ', ' ' },
        { ' ', ' ', '4', ' ', ' ', ' ', ' ', '7', 'A' },
        { ' ', '2', '#', ' ', ' ', '#', ' ', ' ', 'F' },
      };

    [Test]
    public void CertainCellsBySize_NoOptions_ReturnsEmptyList()
    {
      var board = new Board(_board);
      board.ReadBoard();
      var str8t = board.Str8ts[0];

      var result = str8t.CertainCellsBySize();

      Assert.IsEmpty(result);
    }

    [Test]
    public void CertainCellsBySize_2ValidOptions_ReturnsCertainCells()
    {
      var board = new Board(_board);
      board.ReadBoard();
      var str8t = board.Str8ts[0];
      str8t.Members[1].Candidates = new List<int> { '2', '3' };
      str8t.Members[2].Candidates = new List<int> { '2', '3' };

      var result = str8t.CertainCellsBySize();
      Assert.AreEqual(3, result.Count);
      Assert.Contains('1', result);
      Assert.Contains('2', result);
      Assert.Contains('3', result);
    }

    [Test]
    public void CertainCellsBySize_3ValidOptions_ReturnsCertainCells()
    {
      var board = new Board(_board);
      board.ReadBoard();
      var str8t = board.Str8ts[2];
      str8t.Members[0].Candidates = new List<int> { '1', '3', '4', '5'};
      str8t.Members[1].Candidates = new List<int> { '1', '3', '4', '5' };
      str8t.Members[2].Candidates = new List<int> { '1', '3', '4', '5' };

      var result = str8t.CertainCellsBySize();

      Assert.AreEqual(3, result.Count);
      Assert.Contains('2', result);
      Assert.Contains('3', result);
      Assert.Contains('4', result);
    }
  }
}
