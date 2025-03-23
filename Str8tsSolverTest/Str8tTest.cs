using System.Collections.Generic;
using Str8tsSolverLib;
using NUnit.Framework;

namespace Str8tsSolverTest
{
  public class Str8tTests
  {
    private char[,] _board = new char[,]
      { // devilish, 
        { '1', ' ', ' ', '#', ' ', ' ', ' ', ' ', '9' },
        { '#', ' ', ' ', '2', ' ', '#', ' ', ' ', '#' },
        { '#', '9', ' ', ' ', ' ', '5', '#', ' ', ' ' },
        { ' ', ' ', ' ', ' ', ' ', ' ', '3', ' ', '#' },
        { 'G', '#', '6', '3', ' ', ' ', ' ', '#', '#' },
        { ' ', ' ', '7', ' ', '#', ' ', ' ', '4', ' ' },
        { ' ', ' ', '8', ' ', '#', '3', ' ', ' ', ' ' },
        { ' ', ' ', '4', ' ', ' ', ' ', ' ', '7', 'A' },
        { ' ', '2', '#', ' ', ' ', '#', ' ', ' ', 'F' },
      };

    [Test]
    public void GetNakedPairs_2Cells_True()
    {
      var board = new Board(_board);
      board.ReadBoard();
      var str8t = board.Str8ts[0];
      str8t.Members[1].Candidates = new List<int> { '2', '3' };
      str8t.Members[2].Candidates = new List<int> { '2', '3' };
      var np = str8t.GetNakedPairs();
      Assert.AreEqual(2, np.Count);
      Assert.IsTrue(np.Contains('2'));
      Assert.IsTrue(np.Contains('3'));
    }

    [Test]
    public void GetNakedPairs_3Cells_True()
    {
      var board = new Board(_board);
      board.ReadBoard();
      var str8t = board.Str8ts[2];
      str8t.Members[0].Candidates = new List<int> { '1', '3', '4' };
      str8t.Members[1].Candidates = new List<int> { '1', '3', '4' };
      str8t.Members[3].Candidates = new List<int> { '1', '3', '4' };

      var np = str8t.GetNakedPairs();
      Assert.AreEqual(3, np.Count);
      Assert.IsTrue(np.Contains('1'));
      Assert.IsTrue(np.Contains('3'));
      Assert.IsTrue(np.Contains('4'));
    }

    [Test]
    public void GetNakedPairs_4Cells_True()
    {
      var board = new Board(_board);
      board.ReadBoard();
      var str8t = board.Str8ts[1];
      str8t.Members[0].Candidates = new List<int> { '5', '8', '7', '6' };
      str8t.Members[1].Candidates = new List<int> { '5', '8', '7', '6' };
      str8t.Members[2].Candidates = new List<int> { '8', '7', '6', '5' };
      str8t.Members[3].Candidates = new List<int> { '6', '5', '8', '7' };

      var np = str8t.GetNakedPairs();
      Assert.AreEqual(4, np.Count);
    }

    [Test]
    public void CertainCellsBySize_RangeFullyEnclosed_ReturnsAll()
    {
      var board = new Board(_board);
      board.ReadBoard();
      var str8t = board.Str8ts[4];  // '9', ' ', ' ', ' ', '5'
      var result = str8t.CertainCellsBySize();
      Assert.AreEqual(5, result.Count);
      Assert.IsTrue(result.Contains('9'));
      Assert.IsTrue(result.Contains('8'));
      Assert.IsTrue(result.Contains('7'));
      Assert.IsTrue(result.Contains('6'));
      Assert.IsTrue(result.Contains('5'));
    }

    [Test]
    public void CertainCellsBySize_NoOptions_Oneplus3()
    {
      var board = new Board(_board);
      board.ReadBoard();
      var str8t = board.Str8ts[0];

      var result = str8t.CertainCellsBySize();

      Assert.AreEqual(3, result.Count);
      Assert.IsTrue (result.Contains('1'));
      Assert.IsTrue (result.Contains('2'));
      Assert.IsTrue (result.Contains('3'));
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
      var str8t = board.Str8ts[27];

      var result = str8t.CertainCellsBySize();
      Assert.AreEqual(4, result.Count);
      Assert.Contains('5', result);
      Assert.Contains('6', result);
      Assert.Contains('4', result);
      Assert.Contains('7', result);
    }

    [Test]
    public void CertainCellsBySize_BYSize_ReturnsCertainCells()
    {
      var board = new Board(_board);
      board.ReadBoard();
      var str8t = board.Str8ts[2];
      str8t.Members[0].Candidates = new List<int> { '1', '3', '4' };
      str8t.Members[1].Candidates = new List<int> { '1', '3', '4' };
      str8t.Members[2].Candidates = new List<int> { '1', '3', '4' };

      var result = str8t.CertainCellsBySize();

      Assert.AreEqual(4, result.Count);
      Assert.Contains('1', result);
      Assert.Contains('2', result);
      Assert.Contains('3', result);
      Assert.Contains('4', result);
    }

    [Test]
    public void CertainCellsBySize_BYSize_2plus5()
    {
      var board = new Board(_board);
      board.ReadBoard();
      var str8t = board.Str8ts[2];
      var result = str8t.CertainCellsBySize();

      Assert.AreEqual(3, result.Count);
      Assert.Contains('2', result);
      Assert.Contains('3', result);
      Assert.Contains('4', result);
    }

    [Test]
    public void CertainCellsBySize_BYSize_8plus4()
    {
      var board = new Board(_board);
      board.ReadBoard();
      var str8t = board.Str8ts[2];
      var result = str8t.CertainCellsBySize();

      Assert.AreEqual(3, result.Count);
      Assert.Contains('2', result);
      Assert.Contains('3', result);
      Assert.Contains('4', result);
    }

    [Test]
    public void CertainCellsBySize_BYSize_8and4()
    {
      var board = new Board(_board);
      board.ReadBoard();
      var str8t = board.Str8ts[10];
      var result = str8t.CertainCellsBySize();

      Assert.AreEqual(3, result.Count);
      Assert.Contains('8', result);
      Assert.Contains('7', result);
      Assert.Contains('6', result);
    }

    [Test]
    public void CertainCellsBySize_BYSize_7and4()
    {
      var board = new Board(_board);
      board.ReadBoard();
      var str8t = board.Str8ts[8];
      var result = str8t.CertainCellsBySize();

      Assert.AreEqual(2, result.Count);
      Assert.Contains('7', result);
      Assert.Contains('6', result);
    }

    [Test]
    public void CertainCellsBySize_BYSize_3and4()
    {
      var board = new Board(_board);
      board.ReadBoard();
      var str8t = board.Str8ts[11];
      var result = str8t.CertainCellsBySize();

      Assert.AreEqual(2, result.Count);
      Assert.Contains('3', result);
      Assert.Contains('4', result);
    }
  }
}
