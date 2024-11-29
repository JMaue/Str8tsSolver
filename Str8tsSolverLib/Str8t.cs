using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Str8tsSolverLib
{
  public abstract class Str8t
  {
    internal Board _board;
    public readonly int _x;
    protected readonly List<int> _y;
    public void SetRow(Row r) => _row = r;
    protected Row _row;

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
    public bool Contains(Cell c) => Members.Contains(c);

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
      var nakedPairs = new Dictionary<string, int>();
      for (int i=0; i<Members.Count; i++)
      {
        for (int j=i+1; j<Members.Count; j++)
        {
          if (Members[i].Candidates.Count == Members[j].Candidates.Count && Members[i].Candidates.Count <= noOfEmptyCells)
          {
            if (Members[i].Candidates.All(Members[j].Candidates.Contains))
            {
              var key = string.Join("", Members[i].Candidates.Select(c => (char)c));
              if (nakedPairs.ContainsKey(key))
                nakedPairs[key]++;
              else
                nakedPairs[key] = 1;
            }
          }
        }
      }
     
      foreach (var np in nakedPairs)
      {
        if (np.Key.Length == np.Value)
          return np.Key.ToCharArray().ToList();
      }
      return new List<char>();
    }

    public List<char> GetValuesInRowOrCol()
    {
      var rc = _row.GetDecidedValues();

      foreach (var s in _board.Str8ts.Where(s => s is VStr8t && _x == s._x))
      {
        if (s != this)
        {
          rc.AddRange(s.GetNakedPairs());
          rc.AddRange(s.CertainCells());
        }
      }
      return rc;
    }

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

    public abstract List<Str8t> GetPerpendicularStr8ts(Cell m);

    public List<char> CertainCellsByIntersection ()
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

    public List<char> CertainCellsBySize ()
    {
      var rc = new List<char>();
      var options = new List<int>();
      Members.ForEach(m => options.AddRange(m.Candidates));
      options = options.Distinct().ToList();
      var solved = new List<int> ();
      Members.Where(m => m.Value != ' ').ToList().ForEach(x => solved.Add(x.Value));
      options.AddRange(solved);
      if (!options.Any())
        return new List<char>();

      var min = options.Min();
      var max = options.Max();
      if (max - min + 1 == Len)
        return Enumerable.Range(min, max - min + 1).Select(i => (char)(i)).ToList();

      if (Enumerable.Range(min, max - min+1).Any(i => options.Contains(i) == false))
        return new List<char>();

      var certain = new List<char[]>();
      for (var s=min; s<=max-Len+1; s++)
      {
        var attempt = Enumerable.Range(s, Len).Select(i => (char)(i)).ToList();
        if (solved.All(x => attempt.Contains((char)(x))))
          certain.Add(attempt.ToArray());
      }
      return Str8tsSolver.FindIntersection(certain);
    }

    public List<char> CertainCells()
    {
      var rc = new List<char>();
      rc.AddRange(CertainCellsByIntersection());
      rc.AddRange(CertainCellsBySize());
      return rc;
    }

    public abstract bool IsHorizontal { get; }
    public abstract bool IsVertical { get; }
  }

  public class HStr8t : Str8t
  {
    public HStr8t(int r, int c, Board b) : base(r, c, b) { }
    public override (int, int) CellPos(int pos) => (_x, _y[pos]);
    public override string ToString()
    {
      var cells = string.Join("", _y.Select(c => _board._board[_x, c] == ' ' ? '.' : _board._board[_x,c]));
      var pos = $"{_x}:{string.Join("-", _y)}";
      return $"H{pos}:{cells}";
    }

    public override bool IsHorizontal => true;
    public override bool IsVertical => false;

    public override string Cells => string.Join("", _y.Select(c => _board._board[_x, c]));
    public override List<Str8t> PerpendicularStr8ts() => Members.Where(m => m.Vertical != null).Select(m => m.Vertical).ToList();

    public override List<Str8t> GetPerpendicularStr8ts(Cell c)
    {
      var str8ts = new List<Str8t>();
      var row = _board.Rows.Where(r => r.IsVertical && r.Idx == c.Col).FirstOrDefault();
      if (row != null)
        str8ts.AddRange(row.Str8ts);
      return str8ts;
    }
  }

  public class VStr8t : Str8t
  {
    public VStr8t(int r, int c, Board b) : base(r, c, b) { }

    public override (int, int) CellPos(int pos) => (_y[pos], _x);
    public override string ToString()
    {
      var cells = string.Join("", _y.Select(c => _board._board[c, _x] == ' ' ? '.' : _board._board[c, _x]));
      var pos = $"{_x}:{string.Join("-", _y)}";
      return $"V{pos}:{cells}";
    }
    public override string Cells => string.Join("", _y.Select(c => _board._board[c, _x]));
    public override List<Str8t> PerpendicularStr8ts() => Members.Where(m => m.Horizontal != null).Select(m => m.Horizontal).ToList();
    public override bool IsHorizontal => false;
    public override bool IsVertical => true;
   
    public override List<Str8t> GetPerpendicularStr8ts(Cell c)
    {
      var str8ts = new List<Str8t>();
      var row = _board.Rows.Where(r => r.IsHorizontal && r.Idx == c.Row).FirstOrDefault();
      if (row != null)
        str8ts.AddRange(row.Str8ts);
      return str8ts;
    }
  }
}
