namespace Str8tsSolverLib
{
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

}
