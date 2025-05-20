using Str8tsSolverLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Str8tsSolverTest
{
  public class BoardTestBase
  {
    ITxtOut _txtOut;

    [SetUp]
    public void Setup()
    {
      _txtOut = new ConsoleTxtOut();
    }

    protected char[,] LoadBoardFromFile(string filePath)
    {
      var board = new char[9, 9];
      var lines = File.ReadAllLines(filePath);

      for (int i = 0; i < lines.Length; i++)
      {
        for (int j = 0; j < lines[i].Length; j++)
        {
          var v = lines[i][j];
          board[i, j] = v == '.' ? ' ' : v;
        }
      }

      return board;
    }

    protected bool Solve(char[,] b)
    {
      var board = new Board(b, _txtOut);
      board.ReadBoard();
      board.PrintBoard(true);
      var (solved, iterations) = Str8tsSolver.Solve(board, _txtOut);
      return solved;
    }
  }
}
