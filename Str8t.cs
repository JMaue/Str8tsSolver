using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Str8tsSolver
{
  public  class Board
  {
    private static readonly char[] _delims = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', '#'];

    internal readonly char[,] _board;
    internal Cell[,] _grid;   
    public List<Str8t> Str8ts { get; private set; }

    public Board(char[,] board)
    {
      _board = board;
      Str8ts = new List<Str8t>();
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

      _grid = new Cell[9, 9];
      for (int x = 0; x < 9; x++)
      {
        for (int y = 0; y < 9; y++)
        {
          _grid[x, y] = new Cell (x, y, (r, c) => _board[r, c]);
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

  public abstract class Str8t
  {
    internal Board _board;
    public readonly int _x;
    protected readonly List<int> _y;
    protected Str8t(int x, int y, Board board)
    {
      _x = x;
      _y = [y];
      _board = board;
      Members = new List<Cell>();
    }
    public int Len => _y.Count;
    public int StartPos => _y.Min();
    public int EndPos => _y.Min() + Len;
    public void Append(int y) => _y.Add(y);
    public List<Cell> Members { get; set; }
    public abstract (int, int) CellPos(int pos);

    public abstract string Cells { get; }

    public abstract List<Str8t> PerpendicularStr8ts();

    public bool Contains(int x, int y) => x == _x && _y.Contains(y);

    public static bool IsValid(string val)
    {
      var is1 = Enumerable.Range(0, val.Length).Select(i => Convert.ToByte(val[i])).Order().ToList();
      bool isValid = true;
      for (int i = 1; i < is1.Count; i++)
      {
        isValid = isValid && is1[i] - is1[i - 1] == 1;
      }
      return isValid;
    }
  
    public bool IsSolvable()
    {
      var spaces = Cells.Count(c => c == ' ');
      var values = Cells.Where(c => c != ' ').ToList().Count();
      if (values == 0)
        return true;  
      var minVal = Cells.Where(c => c != ' ').Min();
      var maxVal = Cells.Where(c => c != ' ').Max();
      return maxVal - minVal + 1 <= Len;  // 1 .. 6 (5) : +1 = 6 : <= 
    }

    public bool IsSolved() => Cells.Count(c => c == ' ') == 0;

    public List<char> GetNakedPairs ()
    {
      var noOfEmptyCells = Cells.Count(c => c == ' ');
      var nakedPairs = new List<char>();
      for (int i=0; i<Members.Count; i++)
      {
        for (int j=i+1; j<Members.Count; j++)
        {
          if (Members[i].Candidates.Count == Members[j].Candidates.Count && Members[i].Candidates.Count <= noOfEmptyCells)
          {
            if (Members[i].Candidates.All(Members[j].Candidates.Contains))
            {
              nakedPairs.AddRange(Members[i].Candidates.Select (c => (char)c));
            }
          }
        }
      }

      return nakedPairs;
    }

    public abstract List<char> GetValuesInRowOrCol();

    public bool IsValidInRowOrColumn(string val)
    {
      var invalds = GetValuesInRowOrCol();
      var chars = val.ToCharArray();

      return !chars.Any(c => invalds.Contains(c) && !Cells.Contains(c));
    }

    internal void ChangeBoard(Board b)
    {
      _board = b;
    }

    public abstract Str8t GetPerpendicularStr8t(Cell m);

    public List<char> CertainCells ()
    {
      // permutate candidates
      var options = new List<List<char>>();
      for (int i=0; i<Members.Count; i++)
      {
        if (Members[i].Value != ' ')
          options.Add(new List<char> { Members[i].Value });
        else if (Members[i].Candidates.Count > 0)
          options.Add(Members[i].Candidates.Select(c => (char)c).ToList());
      }
      var candidates = new List<char[]>();
      foreach (var o in Permutations.Permute(options))
      {
        var nextTry = Cells;
        for (int i = 0; i < o.Length; i++)
        {
          nextTry = Str8tsSolver.ReplaceFirst(nextTry, ' ', o[i]);
        }
        if (IsValid(nextTry))
        {
          candidates.Add(o);
        }
      }

      // find intersection of all options o: o[i] == o[j] for all i,j
      return Str8tsSolver.FindIntersection(candidates);
    }
  }

  public class HStr8t : Str8t
  {
    public HStr8t(int r, int c, Board b) : base(r, c, b) { }
    public override (int, int) CellPos(int pos) => (_x, _y[pos]);
    public override string ToString() => $"H{_x}:{Cells}";

    public override string Cells => string.Join("", _y.Select(c => _board._board[_x, c]));
    public override List<Str8t> PerpendicularStr8ts() => Members.Where(m=>m.Vertical != null).Select(m=>m.Vertical).ToList();

    public override List<char> GetValuesInRowOrCol()
    {
      var rc = Enumerable.Range(0, 9)
        .Where(i => _board._board[_x, i] >= '1' && _board._board[_x, i] <= '9')
        .Select(i =>_board._board[_x, i]).ToList();

      rc.AddRange(Enumerable.Range(0, 9)
        .Where(i => _board._board[_x, i] >= 'A' && _board._board[_x, i] <= 'I')
        .Select(i => (char)(_board._board[_x, i] - 'A' + '1')));

      foreach (var s in _board.Str8ts.Where(s => s is HStr8t && _x == s._x))
      {
        if (s != this)
          rc.AddRange(s.GetNakedPairs());
      }
      return rc;
    }

    public override Str8t GetPerpendicularStr8t(Cell c) => c.Vertical;
  }

  public class VStr8t : Str8t
  {
    public VStr8t(int r, int c, Board b) : base(r, c, b) { }

    public override (int, int) CellPos(int pos) => (_y[pos], _x);
    public override string ToString() => $"V{_x}:{Cells}";
    public override string Cells => string.Join("", _y.Select(c => _board._board[c, _x]));

    public override List<Str8t> PerpendicularStr8ts() => Members.Where(m=>m.Horizontal != null).Select(m => m.Horizontal).ToList();

    public override List<char> GetValuesInRowOrCol()
    {
      var rc = Enumerable.Range(0, 9)
       .Where(i => _board._board[i, _x] >= '1' && _board._board[i, _x] <= '9')
       .Select(i => _board._board[i, _x]).ToList();

      rc.AddRange(Enumerable.Range(0, 9)
        .Where(i => _board._board[i, _x] >= 'A' && _board._board[i, _x] <= 'I')
        .Select(i => (char)(_board._board[i, _x] - 'A' + '1')));

      foreach (var s in _board.Str8ts.Where(s => s is VStr8t && _x == s._x))
      {
        if (s != this)
          rc.AddRange(s.GetNakedPairs());
      }
      return rc;
    }

    public override Str8t GetPerpendicularStr8t(Cell c) => c.Horizontal;
  }

  public class Cell
  {
    public static char[] ValidCells = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
    public static char[] BlackCells = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I' };
    public int Col { get; set; }
    public int Row { get; set; }

    private Func<int, int, char> _getChar;

    public char[,] Presentation { get
      {
        var rc = new char[3, 3]
        {
          { ' ', ' ', ' ' },
          { ' ', ' ', ' ' },
          { ' ', ' ', ' ' }
        };
        var c = Value;
        if (ValidCells.Contains(c))
          rc[1, 1] = c;
        else if (BlackCells.Contains(c))
          rc[1, 1] = (char)(c - 'A' + '1');
        else
        {
          if (Candidates.Any())
          {
            for (int i = 1; i <= 9; i++)
            {
              if (Candidates.Contains((char)(i + '0')))
                rc[(i-1) / 3, (i-1) % 3] = '*';
            }
          }
        }
        return rc;
      } 
    }

    public bool IsBlack => Value == '#' || BlackCells.Contains(Value);

    public Cell (int r, int c, Func<int, int, char> ch)
    {
      Row = r;
      Col = c;
      _getChar = ch;
    }

    public char Value => _getChar(Row, Col);

    public Str8t? Horizontal { get; set; }
    public Str8t? Vertical { get; set; }

    public List<int> Candidates = new List<int>();

    internal void UpdateCandidates(List<int> list)
    {
      if (Candidates.Count == 0 && Value == ' ')
      {
        Candidates.AddRange(list.Distinct());
        return;
      }

      Candidates.RemoveAll(c => !list.Contains(c));
    }
  }
}
