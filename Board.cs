using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Str8tsSolver
{
  public class Board
  {
    private static readonly char[] _delims = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', '#'];

    internal readonly char[,] _board;
    internal Cell[,] _grid;
    public List<Str8t> Str8ts { get; private set; }
    public List<Row> Rows { get; private set; }

    public Board(char[,] board)
    {
      _board = board;
      Str8ts = new List<Str8t>();
      Rows = new List<Row>();
    }

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
      clone.ReadStr8ts();
      return clone;
    }

    public List<Str8t> ReadHorizontalStr8ts(char[,] board) => ReadStr8ts(board, (x, y) => board[x, y], (x, y) => new HStr8t(x, y, this));
    public List<Str8t> ReadVerticalStr8ts(char[,] board) => ReadStr8ts(board, (x, y) => board[y, x], (x, y) => new VStr8t(x, y, this));
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

    public void ReadStr8ts()
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
          row = s.IsHorizontal ? new HRow(s._x) : new HRow(s._x);
          Rows.Add(row);
        }
        row.AddStr8t(s);
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
      _board[x, y] = (char)(val + '0');
    }

    internal void UpdateCell(Str8t str8t, int pos, char val)
    {
      (int x, int y) = str8t.CellPos(pos);
      _board[x, y] = val;
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
        return (char)(v - 'A' + '1');

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
   
    public void PrintBoard()
    {
      for (int x = 0; x < 9; x++)
      {
        for (int y = 0; y < 9; y++)
        {
          char c = _board[x, y] == ' ' ? '.' : _board[x, y];
          Console.Write(c);
        }
        Console.WriteLine();
      }
      Console.WriteLine();
    }

    public void PrintBoard(bool big = true)
    {
      for (int x = 0; x < 9; x++)
      {
        Console.WriteLine(" ----- ----- ----- ----- ----- ----- ----- ----- ----- ");
        for (int r = 0; r < 3; r++)
        {
          Console.Write('|');
          for (int y = 0; y < 9; y++)
          {
            var c = _grid[x, y].Presentation;
            if (_grid[x, y].IsBlack)
            {
              Console.ForegroundColor = ConsoleColor.Black;
              Console.BackgroundColor = ConsoleColor.White;
            }
            else
            {
              Console.ForegroundColor = ConsoleColor.White;
              Console.BackgroundColor = ConsoleColor.Black;
            }
            Console.Write(' ');
            for (int i = 0; i < 3; i++)
              Console.Write(c[r, i]);
            Console.Write(' ');
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write('|');
          }
          Console.WriteLine();
        }
      }
      Console.WriteLine(" ----- ----- ----- ----- ----- ----- ----- ----- ----- ");
      Console.WriteLine();
    }
  }
}
