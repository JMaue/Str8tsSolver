using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Str8tsSolverLib.Algorithms
{
  public class ExcludeWings : IAlgorithm
  {
    public bool Solve(Board board, Str8t str8t)
    {
      bool progress = false;

      // from columns
      var grid = board.IsolateSureCandidates(false);
      for (char digit = '1'; digit <= '9'; digit++)
      {
        var (keep, remove) = FindWingsInColumns(grid, digit);
        if (keep.Any())
        {
          board.TxtOut?.WriteLine($"ExcludeWings {digit} in columns {string.Join(",", keep)} from rows {string.Join(",", remove)}");
          // remove <digit> from all cells in rows <remove> that are not in columns <keep>
          foreach (var r in remove)
          {
            for (int c = 0; c < 9; c++)
            {
              if (keep.Contains(c))
                continue;
              if (board._grid[r, c].Candidates.Contains(digit))
              {
                board._grid[r, c].Candidates.Remove(digit);
                board.TxtOut?.WriteLine($"ExcludeWings remove {digit} from {r},{c}");

                progress = true;
              }
            }
          }
        }
      }

      return progress;

      // from rows
      grid = board.IsolateSureCandidates(true);
      for (char digit = '1'; digit <= '9'; digit++)
      {
        var (keep, remove) = FindWingsInRows(grid, digit);
        if (keep.Any())
        {
          board.TxtOut?.WriteLine($"ExcludeWings {digit} in rows {string.Join(",", keep)} from columns {string.Join(",", remove)}");
          // remove <digit> from all cells in columns <remove> that are not in rows <keep>
          foreach (var c in remove)
          {
            for (int r = 0; r < 9; r++)
            {
              if (keep.Contains(r))
                continue;
              if (board._grid[r, c].Candidates.Contains(digit))
              {
                board._grid[r, c].Candidates.Remove(digit);
                board.TxtOut?.WriteLine($"ExcludeWings remove {digit} from {r},{c}");

                progress = true;
              }
            }
          }
        }
      }

      return progress;
    }
    /*
    public (List<int>, List<int>) FindWingsInColumns(Cell[,] grid, char digit)
    {
      //digit = '5';
      var keepInCols = new List<int>();
      var removeInRows = new List<int>();
      var candidatesPerColumn = new Dictionary<int, List<int>>();
      for (int c = 0; c < 9; c++)
      {
        // extract all columns that contain <digit> as a sure candidate
        var col = new List<int>();
        for (int r = 0; r < 9; r++)
        {
          if (grid[r, c].Candidates.Contains (digit))
            col.Add(r);
        }
        if (col.Count > 1)
          candidatesPerColumn.Add(c, col);
      }
      if (candidatesPerColumn.Any())
      {
        // find the smallest pairs:
                               // size, (column, candidateRows)
        var wingCandidatesBySize = new Dictionary<int, List<(int, string)>>();
        foreach (var kvp in candidatesPerColumn)
        {
          var col = kvp.Key;
          var size = kvp.Value.Count;
          if (wingCandidatesBySize.ContainsKey(size))
          {
            wingCandidatesBySize[size].Add((col, IntList2Str(kvp.Value)));
          }
          else
          {
            wingCandidatesBySize.Add(size, new List<(int, string)> { (col, IntList2Str(kvp.Value)) });
          }
        }
        foreach (var size in wingCandidatesBySize.Keys)
        {
          if (size != wingCandidatesBySize[size].Count)
            continue; // not a wing

          // check if the columns are the same
          var wingColumns = wingCandidatesBySize[size].Select(c => c.Item1).Distinct().ToList();
          var wingCandidates = wingCandidatesBySize[size].Select(c => c.Item2).ToList();

          // check if all candidates are the same
          bool isWing = wingCandidates.All(c => c == wingCandidates[0]);
          if (isWing)
          {
            keepInCols = wingColumns;
            removeInRows = wingCandidates[0].Select(c => int.Parse(c.ToString())).ToList();

            return (keepInCols, removeInRows);
          }
        }

      }
      return (keepInCols, removeInRows);
    }  */

    public (List<int>, List<int>) FindWingsInColumns(Cell[,] grid, char digit)
    {
     // digit = '2';
      var keepInCols = new List<int>();
      var removeInRows = new List<int>();
      var candidatesPerColumn = new Dictionary<int, List<int>>();
      for (int c = 0; c < 9; c++)
      {
        // extract all rows that contain <digit> as a sure candidate
        var col = new List<int>();
        for (int r = 0; r < 9; r++)
        {
          if (grid[r, c].Candidates.Contains(digit))
            col.Add(r);
        }
        if (col.Count > 1)
          candidatesPerColumn.Add(c, col);
      }
      var noOfCols = candidatesPerColumn.Count;
      if (noOfCols > 1)
      {
        var cols = candidatesPerColumn.Keys.Select(r => (char)r).ToArray();
        for (int n = 2; n <= noOfCols; n++)
        {
          foreach (var o in Permutations.Permute(cols, 0, n))
          {
            // test if that selection of rows is a wing
            var rows = new List<int>();
            foreach (var col in o)
            {
              rows.AddRange(candidatesPerColumn[col]);
            }
            if (rows.Distinct().Count() == n)
            {
              // we have a wing
              keepInCols = o.ToList().Select(c => (int)c).ToList();
              removeInRows = rows.Distinct().ToList();

              return (keepInCols, removeInRows);
            }
          }
        }


      }
      return (keepInCols, removeInRows);
    }

    public (List<int>, List<int>) FindWingsInRows(Cell[,] grid, char digit)
    {
      //digit = '5';
      var keepInRows = new List<int>();
      var removeInCols = new List<int>();
      var candidatesPerRow = new Dictionary<int, List<int>>();
      for (int r = 0; r < 9; r++)
      {
        // extract all rows that contain <digit> as a sure candidate
        var row = new List<int>();
        for (int c = 0; c < 9; c++)
        {
          if (grid[r, c].Candidates.Contains(digit))
            row.Add(r);
        }
        if (row.Count > 1)
          candidatesPerRow.Add(r, row);
      }
      var noOfRows = candidatesPerRow.Count;
      if (noOfRows > 1)
      {
        var rows = candidatesPerRow.Keys.Select(r=> (char)r).ToArray();
        for (int n=2; n<=noOfRows; n++)
        {
          foreach (var o in Permutations.Permute(rows, 0, n))
          {
            // test if that selection of rows is a wing
            var cols = new List<int>();
            foreach (var row in o)
            {
              cols.AddRange(candidatesPerRow[row]);
            }
            if (cols.Distinct().Count() == n)
            {
              // we have a wing
              keepInRows = o.ToList().Select(r=>(int)r).ToList();
              removeInCols = cols.Distinct().ToList();

              return (keepInRows, removeInCols);
            }
          }
        }


      }
      return (keepInRows, removeInCols);
    }

    private string IntList2Str (List<int> list) => string.Join("", list.Order());

  }
}
