using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Str8tsSolverLib
{
  public class Board
  {
    private static readonly char[] _delims = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', '#'];

    internal readonly char[,] _board;
    internal Cell[,] _grid;
    public List<Str8t> Str8ts { get; private set; }
    public List<Row> Rows { get; private set; }

    public Board(char[,] board, ITxtOut? txtOut = null)
    {
      _board = board;
      TxtOut = txtOut;
      Str8ts = new List<Str8t>();
      Rows = new List<Row>();
    }

    // Definiere den Delegate
    public delegate void SolvingProgressHandler(string currStr8t);
    public event SolvingProgressHandler SolvingProgress;

    public delegate void PositionSolvedHandler(int x, int y, char newValue);
    public event PositionSolvedHandler PositionSolved;

    public delegate void PuzzleSolvedHandler(bool success);
    public event PuzzleSolvedHandler PuzzleSolved;

    public ITxtOut? TxtOut;

    public Board Clone()
    {
      var b = new char[9, 9];
      for (int x = 0; x < 9; x++)
      {
        for (int y = 0; y < 9; y++)
        {
          b[x, y] = _board[x, y];
        }
      }

      var clone = new Board(b);
      clone.ReadBoard();
      return clone;
    }

    private Cell[,] CloneGrid()
    {
      var grid = new Cell[9, 9];
      for (int x = 0; x < 9; x++)
      {
        for (int y = 0; y < 9; y++)
        {
          grid[x, y] = _grid[x, y].Clone();
        }
      }
      return grid;
    }

    public Cell[,] IsolateSureCandidatesInColumns()
    {
      var grid = CloneGrid();
      var dict = new Dictionary<int, List<char>>();
      for (int i = 0; i < 9; i++)
      {
        var row = Rows.Find(r => r.IsVertical && r.Idx == i);
        var certainCandidates = row.Str8ts.SelectMany (s=>s.CertainCells()).ToList();

        certainCandidates.AddRange (row.GetCertainCandidatesFromSize());
        var cc = certainCandidates.Distinct();

        for (int r = 0; r < 9; r++)
        {
          grid[r, i].Candidates.RemoveAll(c => !cc.Contains(c));
        }
      }

      return grid;
    }

    public Cell[,] IsolateSureCandidatesInRow()
    {
      var grid = CloneGrid();
      var dict = new Dictionary<int, List<char>>();
      for (int i = 0; i < 9; i++)
      {
        var row = Rows.Find(r => r.IsHorizontal && r.Idx == i);
        var certainCandidates = row.Str8ts.SelectMany(s => s.CertainCells()).ToList();

        certainCandidates.AddRange(row.GetCertainCandidatesFromSize());
        var cc = certainCandidates.Distinct();

        for (int c = 0; c < 9; c++)
        {
          grid[i, c].Candidates.RemoveAll(r => !cc.Contains(r));
        }
      }

      return grid;
    }

    public List<Str8t> ReadHorizontalStr8ts(char[,] board) =>
      ReadStr8ts(board, (x, y) => board[x, y], (x, y) => new HStr8t(x, y, this));

    public List<Str8t> ReadVerticalStr8ts(char[,] board) =>
      ReadStr8ts(board, (x, y) => board[y, x], (x, y) => new VStr8t(x, y, this));

    private List<Str8t> ReadStr8ts(char[,] board, Func<int, int, char> select, Func<int, int, Str8t> create)
    {
      var str8ts = new List<Str8t>();
      for (int x = 0; x < 9; x++)
      {
        Str8t? str8t = null;
        for (int y = 0; y < 9; y++)
        {
          var c = select(x, y);
          if (_delims.Contains(c))
          {
            str8t = addStr8t(str8t);
            continue;
          }

          if (str8t == null)
            str8t = create(x, y);
          else
            str8t.Append(y);
        }

        addStr8t(str8t);
      }

      return str8ts;

      Str8t? addStr8t(Str8t? str8t)
      {
        if (str8t == null) return str8t;
        str8ts.Add(str8t);
        str8t = null;

        return str8t;
      }
    }

    public void ReadBoard()
    {
      var hStr8ts = ReadHorizontalStr8ts(_board);
      var vStr8ts = ReadVerticalStr8ts(_board);
      hStr8ts.RemoveAll(s => s.Cells.Length < 2);
      vStr8ts.RemoveAll(s => s.Cells.Length < 2);
      Str8ts.AddRange(hStr8ts);
      Str8ts.AddRange(vStr8ts);

      ReadGrid(hStr8ts, vStr8ts);
      ReadRows();
    }

    private void ReadRows()
    {
      foreach (var s in Str8ts)
      {
        var row = Rows.FirstOrDefault(r => r.Idx == s._x && r.IsHorizontal == s.IsHorizontal);
        if (row == null)
        {
          row = s.IsHorizontal ? new HRow(s._x) : new VRow(s._x);
          Rows.Add(row);
        }

        row.AddStr8t(s);
        s.SetRow(row);
        row.SetCells(this);
      }
    }

    private void ReadGrid(List<Str8t> hStr8ts, List<Str8t> vStr8ts)
    {
      _grid = new Cell[9, 9];
      for (int x = 0; x < 9; x++)
      {
        for (int y = 0; y < 9; y++)
        {
          _grid[x, y] = new Cell(x, y, (r, c) => _board[r, c]);
          var horizontal = hStr8ts.FirstOrDefault(s => s.Contains(x, y));
          var vertical = vStr8ts.FirstOrDefault(s => s.Contains(y, x));
          _grid[x, y].Horizontal = horizontal;
          _grid[x, y].Vertical = vertical;

          if (horizontal != null)
            horizontal.Members.Add(_grid[x, y]);
          if (vertical != null)
            vertical.Members.Add(_grid[x, y]);
        }
      }
    }

    internal void UpdateCell(Str8t str8t, int pos, int val)
    {
      (int x, int y) = str8t.CellPos(pos);
      _board[x, y] = (char) (val + '0');
      if (PositionSolved != null)
        PositionSolved(x, y, _board[x, y]);
    }

    internal void UpdateCell(Str8t str8t, int pos, char val)
    {
      (int x, int y) = str8t.CellPos(pos);
      _board[x, y] = val;
      if (PositionSolved != null)
        PositionSolved(x, y, _board[x, y]);
      _grid[x, y].Candidates.Clear();
    }

    internal void UpdateCells(Str8t str8t, string val)
    {
      for (int pos = 0; pos < val.Length; pos++)
      {
        UpdateCell(str8t, pos, val[pos]);
      }
    }

    private char GetValue(int x, int y)
    {
      var v = _board[x, y];
      if (Cell.ValidCells.Contains(v))
        return v;
      if (Cell.BlackCells.Contains(v))
        return (char) (v - 'A' + '1');

      return ' ';
    }

    public bool IsValid()
    {
      bool isValid = true;
      for (int x = 0; x < 9; x++)
      {
        var cellValues = Enumerable.Range(0, 9).Select(y => GetValue(x, y)).ToList();
        isValid = isValid && Cell.ValidCells.All(c => cellValues.Count(c1 => c1 == c) <= 1);
      }

      if (!isValid)
        return false;

      for (int y = 0; y < 9; y++)
      {
        var cellValues = Enumerable.Range(0, 9).Select(x => GetValue(x, y)).ToList();
        isValid = isValid && Cell.ValidCells.All(c => cellValues.Count(c1 => c1 == c) <= 1);
      }

      return isValid;
    }

    public bool IsSolved => Str8ts.All(s => s.IsSolved());

    //public void PrintBoard()
    //{
    //  if (TxtOut == null)
    //    return;

    //  for (int x = 0; x < 9; x++)
    //  {
    //    for (int y = 0; y < 9; y++)
    //    {
    //      char c = _board[x, y] == ' ' ? '.' : _board[x, y];
    //      TxtOut.Write(c);
    //    }

    //    TxtOut.WriteLine();
    //  }

    //  TxtOut.WriteLine();
    //}

    public void PrintBoard(bool big = true)
    {
      PrintBoard(_grid);
    }

    public void PrintBoard(Cell[,] grid)
    {
      if (TxtOut == null)
        return;

      for (int x = 0; x < 9; x++)
      {
        TxtOut.WriteLine(" ----- ----- ----- ----- ----- ----- ----- ----- ----- ");
        for (int r = 0; r < 3; r++)
        {
          TxtOut.Write('|');
          for (int y = 0; y < 9; y++)
          {
            var c = grid[x, y].Presentation;
            if (grid[x, y].IsBlack)
            {
              TxtOut.SetColors(Color.Black, Color.White);
              //Console.ForegroundColor = ConsoleColor.Black;
              //Console.BackgroundColor = ConsoleColor.White;
            }
            else
            {
              TxtOut.SetColors(Color.White, Color.Black);
              //Console.ForegroundColor = ConsoleColor.White;
              //Console.BackgroundColor = ConsoleColor.Black;
            }

            TxtOut.Write(' ');
            for (int i = 0; i < 3; i++)
              TxtOut.Write(c[r, i]);
            TxtOut.Write(' ');
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            TxtOut.Write('|');
          }

          TxtOut.WriteLine();
        }
      }

      TxtOut.WriteLine(" ----- ----- ----- ----- ----- ----- ----- ----- ----- ");
      TxtOut.WriteLine();
    }

    // for unit tests only
    public void AssignCandidates(int r, int c, List<int> candidates)
    {
      if (candidates.Count == 0)
        return;
      var cell = _grid[r, c];
      cell.Candidates.Clear();
      cell.Candidates.AddRange(candidates.Select(c=>(char)c));
    }

    public bool Finish()
    {
      var rc = IsSolved && IsValid();
      PuzzleSolved?.Invoke(rc);
      return rc;
    }

    internal void ReportProgress(string v)
    {
      SolvingProgress?.Invoke(v);
    }
  }
}
