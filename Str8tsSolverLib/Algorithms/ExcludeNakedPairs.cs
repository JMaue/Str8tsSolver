﻿namespace Str8tsSolverLib
{
  internal class ExcludeNakedPairs : IAlgorithm
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
          var nextTry = str8t.CellsOptions (o);
          //for (int i = 0; i < cnt; i++)
          //{
          //  nextTry = ReplaceFirst(nextTry, ' ', o[i]);
          //}
          if (Str8t.IsValid(nextTry) && IsValid(board, str8t, nextTry))
          {
            candidates.Add(o);
            //ccess = true;
          }
        }
        if (candidates.Count > 0)
        {
          // list of candidates per cell
          List<char>[] candidates4Cells = new List<char>[cnt];
          int idxOfCell = 0;
          for (int i = 0; i < cnt; i++)
          {
            candidates4Cells[i] = new List<char>();
            candidates.ForEach(o => candidates4Cells[i].Add(o[i]));
            str8t.Members[i].Candidates = candidates4Cells[i];
            var emptyCells = str8t.Cells;
            idxOfCell = emptyCells.IndexOf(' ', idxOfCell);
            var cell = str8t.Members[idxOfCell];
            cell.Candidates = candidates4Cells[i];

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

    //public static string ReplaceFirst(string text, char search, char replace)
    //{
    //  int pos = text.IndexOf(search);
    //  if (pos < 0)
    //  {
    //    return text;
    //  }
    //  return text.Substring(0, pos) + replace + text.Substring(pos + 1);
    //}
  }

}
