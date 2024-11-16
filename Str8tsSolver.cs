using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Str8tsSolver
{
  interface IAlgorithm
  {
    bool Solve(Board board, Str8t str8t);
    //bool IsValid(Board board, Str8t str8t, int pos, int val);
  }

  public static class Str8tsSolver
  {
    public static bool Solve(Board board, out int iterations)
    {
      iterations = 0;
      var algorithms = new List<IAlgorithm>
      {
        //new SingleGapInStr8t(),
        new CellOptionsAlgo()
      };
      bool progress;
      do
      {
        progress = false;
        foreach (var str8t in board.Str8ts)
        {
          if (!str8t.IsSolved())
          {
            foreach (var alg in algorithms)
            {
              progress |= alg.Solve(board, str8t);
            }
          }
        }
        board.PrintBoard();
       
        if (progress)
          iterations++;

      } while (progress);

      return board.IsSolved;
    }
  }

  internal class SingleGapInStr8t : IAlgorithm
  {
    public bool Solve(Board board, Str8t str8t)
    {
      bool success = false;
      if (str8t.Cells.Count(c => c == ' ') == 1)
      {
        int pos = str8t.Cells.IndexOf(' ');
        var options = new List<int>();
        for (int i = 1; i <= 9; i++)
        {
          var nextTry = str8t.Cells.Replace(' ', (char)(i + '0'));
          if (Str8t.IsValid(nextTry) && IsValid(board, str8t, pos, i))
          {
            options.Add(i);
          }
          {
            var b = board.Clone();
            b.UpdateCell(str8t, pos, i);
            if (b.IsValid())
              options.Add(i);
          }
        }
        if (options.Count == 1)
        {
          board.UpdateCell(str8t, pos, options[0]);
          success = true;
        }
      }

      return success;
    }

    public bool IsValid(Board board, Str8t str8t, int pos, int val)
    {
      var b = board.Clone();
      b.UpdateCell(str8t, pos, val);
      return b.IsValid();
    }
  }

  internal class CellOptionsAlgo : IAlgorithm
  {
    public bool IsValid(Board board, Str8t str8t, string val)
    {
      var b = board.Clone();
      b.UpdateCells(str8t, val);
      var perp = str8t.PerpendicularStr8ts();
      perp.ForEach(p => p.ChangeBoard(b));
      bool rc = perp.All(s => s.IsSolvable()) && b.IsValid();
      perp.ForEach(p => p.ChangeBoard(board));
      return rc;
    }

    public bool Solve(Board board, Str8t str8t)
    {
      bool success = false;
      var cnt = str8t.Cells.Count(c => c == ' ');
      if (cnt > 0)
      {
        List<int> pos = Enumerable.Range(0, str8t.Len).Where(i => str8t.Cells[i] == ' ').Select(x=>x).ToList();
        var options = Cell.ValidCells.ToList();
        options.RemoveAll(str8t.Cells.Contains);

        var candidates = new List<char[]>();
        foreach (var o in Permutations.Permute (options.ToArray(), 0, pos.Count))
        {
          var nextTry = str8t.Cells;
          for (int i = 0; i < cnt; i++)
          {
            nextTry = ReplaceFirst(nextTry, ' ', o[i]);
          }
          if (Str8t.IsValid(nextTry) && IsValid(board, str8t, nextTry))
          {
            candidates.Add(o);
            //ccess = true;
          }
        }
        if (candidates.Count > 0)
        {
          for (int i = 0; i < cnt; i++)
          {
            var firstValue = candidates[0][i];
            bool allSame = candidates.All(o => o[i] == firstValue);
            if (allSame)
            {
              board.UpdateCell(str8t, pos[i], firstValue);
              success = true;
            }
          }
        }
      }

      return success;
    }

    public static string ReplaceFirst(string text, char search, char replace)
    {
      int pos = text.IndexOf(search);
      if (pos < 0)
      {
        return text;
      }
      return text.Substring(0, pos) + replace + text.Substring(pos + 1);
    }
  }

}
