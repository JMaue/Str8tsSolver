using System.Linq;

namespace Str8tsSolverLib
{
  internal class PermuteCandidates : IAlgorithm
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
        var options = new List<List<char>>();
        foreach (var m in str8t.Members.Where(c => c.Value == ' '))
        {
          var c = m.Candidates.Select(c => (char)c).ToList();

          var perp = str8t.GetPerpendicularStr8ts(m);
          if (perp.Any())
          {
            var certainCells = new List<char>();
            perp.ForEach(p => certainCells.AddRange(p.Contains(m) ? p.GetNakedPairs(m) : p.CertainCells()));
            c.RemoveAll(certainCells.Contains);
          }
          options.Add(c);
        }
        var candidates = new List<char[]>();
        foreach (var o in Permutations.Permute (options))
        {
          var nextTry = str8t.CellsOptions(o);
          //var nextTry = str8t.Cells;
          //for (int i = 0; i < cnt; i++)
          //{
          //  nextTry = Str8tsSolver.ReplaceFirst(nextTry, ' ', o[i]);
          //}
          if (Str8t.IsValid(nextTry) && str8t.IsValidInRowOrColumn(nextTry) && IsValid(board, str8t, nextTry))
          {
            candidates.Add(o);
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
            var emptyCells = str8t.Cells;
            idxOfCell = emptyCells.IndexOf(' ', idxOfCell);
            var cell = str8t.Members[idxOfCell++];
            success |= cell.UpdateCandidates(candidates4Cells[i]);

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
  }

}
