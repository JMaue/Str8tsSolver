﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

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
    public abstract string CellsOptions(char[] options);
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
        if (!Members[i].Candidates.Any())
          continue;

        for (int j=i+1; j<Members.Count; j++)
        {
          if (!Members[j].Candidates.Any())
            continue;

          if (Members[i].Candidates.Count == Members[j].Candidates.Count && Members[i].Candidates.Count <= noOfEmptyCells)
          {
            if (Members[i].Candidates.All(Members[j].Candidates.Contains))
            {
              var key = string.Join("", Members[i].Candidates.Order().Select(c => (char)c));
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
        var connections = new List<int>() {0, 1, 3, 6, 10, 15, 21, 28, 36 };
        // 2=>1, 3=>3, 4=>6, 5=>10, 6=>15, 7=>21, 8=>28, 9=>36
        if (np.Value == connections[np.Key.Length-1])
          return np.Key.ToCharArray().ToList();
      }
      return new List<char>();
    }

    public List<char> GetNakedPairs(Cell m)
    {
      var np = GetNakedPairs();
      if (m.Candidates.All(c=>np.Contains((char)c)) && m.Candidates.Count == np.Count)
      {
        // do not consider the naked pairs if thhe current cell is one of them
        return new List<char>();
      }
      return np;
    }

    public virtual List<char> GetValuesInRowOrCol()
    {
      return _row.GetDecidedValues();
    }


    public bool IsValidInRowOrColumn(string val)
    {
      var invalds = GetValuesInRowOrCol();
      var chars = val.ToCharArray();

      // test if this would remove all candidates from a cell
      var cellPositions = Enumerable.Range(StartPos, Len);
      // consider all cells but the ones in this Str8t
      var cellsToConsider = Enumerable.Range(0, 9).Where(i => !cellPositions.Contains(i)).ToList(); 
      if (_row.WouldEraseCandidates(chars, cellsToConsider))
        return false;

      return !chars.Any(c => invalds.Contains(c) && !Cells.Contains(c));
    }

    internal void ChangeBoard(Board b)
    {
      _board = b;
    }

    public abstract List<Str8t> GetPerpendicularStr8ts(Cell m);
    public abstract Str8t? GetPerpendicularStr8t(Cell m);

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
        var nextTry = CellsOptions (o);
        //for (int i = 0; i < o.Length; i++)
        //{
        //  nextTry = Str8tsSolver.ReplaceFirst(nextTry, ' ', o[i]);
        //}
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
      var cnt = Cells.Count(c => c == ' ');
      if (cnt == 0)
        return Cells.Select(c => c).ToList(); // all cells are solved already

      var solved = new List<int>();
      Members.Where(m => m.Value != ' ').ToList().ForEach(x => solved.Add(x.Value));

      if (solved.Any())
      {
        var min1 = solved.Min();
        var max1 = solved.Max();
        if (max1 - min1 + 1 == Len)
          // two solved cells are at the ends of the Str8t
          return Enumerable.Range(min1, Len).Select(i => (char)i).ToList();
      }

      var rc = GetNakedPairs();
      if (cnt == rc.Count) // all unsolved cells are naked pairs
      {
        rc.AddRange(solved.Select(c => (char)c));
        return rc;
      }

      if (Len == 9)
        return Enumerable.Range(1, Len).Select(i => (char)(i + '0')).ToList(); ;
      
      // one of the solved cells is at the end of the Str8t
      if (solved.Contains('1'))
        return Enumerable.Range(1, Len).Select(i => (char)(i + '0')).ToList();
      if (solved.Contains('9'))
        return Enumerable.Range(9 - Len + 1, Len).Select(i => (char)(i + '0')).ToList();

      if (solved.Contains('2') && Len > 2)
      {
        var possibleCandidates = Enumerable.Range(3, Len-1).Select(i => (char)(i + '0')).ToList();
        possibleCandidates.Add('1');
        rc.AddRange(FindCertainCellsViaPermutation(possibleCandidates));
        return rc.Distinct().ToList();
      }
      if (solved.Contains('8') && Len > 2)
      {
        var possibleCandidates = Enumerable.Range(8 - Len + 1, Len-1).Select(i => (char)(i + '0')).ToList();
        possibleCandidates.Add('9');
        rc.AddRange(FindCertainCellsViaPermutation(possibleCandidates));
        return rc.Distinct().ToList();
      }
      if (solved.Contains('3') && Len > 3)
      {
        var possibleCandidates = Enumerable.Range(4, Len - 1).Select(i => (char)(i + '0')).ToList();
        possibleCandidates.Add('2');
        possibleCandidates.Add('1');
        rc.AddRange(FindCertainCellsViaPermutation(possibleCandidates));
        return rc.Distinct().ToList();


        //rc.AddRange(Enumerable.Range(3, Len - 2).Select(i => (char)(i + '0')));
        //return rc.Distinct().ToList();
      }
      if (solved.Contains('7') && Len > 3)
      {
        var possibleCandidates = Enumerable.Range(7 - Len + 1, Len - 1).Select(i => (char)(i + '0')).ToList();
        possibleCandidates.Add('8');
        possibleCandidates.Add('9');
        rc.AddRange(FindCertainCellsViaPermutation(possibleCandidates));
        return rc.Distinct().ToList();
      }

      rc = FindCertainCellsViaPermutation(rc);
      rc.AddRange(solved.Select(c => (char)c));
      return rc;
    }

    private List<char> FindCertainCellsViaPermutation(List<char> possibleCandidates)
    {
      var rc = new List<char>();
      var options = new List<List<char>>();
      foreach (var m in Members)
      {
        var c = m.Value != ' ' ?
          new List<char> { m.Value } :
          m.Candidates.Any() ?
          m.Candidates.Select(c => (char)c).ToList() :
          possibleCandidates;

        options.Add(c);
      }
      var candidates = new List<char[]>();
      foreach (var o in Permutations.Permute(options))
      {
        if (IsValid(string.Join("", o)))
        {
          candidates.Add(o);
        }
      }
      if (candidates.Count > 0)
      {
        var allCandidates = candidates.SelectMany(c => c).ToList().Distinct();
        foreach (var c in allCandidates)
        {
          if (candidates.All(o => o.Contains(c)))
            rc.Add(c);
        }
        //if (rc.Any())
        //{
        //  return rc;
        //}
      }
      return rc;
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

    public virtual string GetId() => $"{_x}:{string.Join("-", _y)}";

    internal void RemoveUnsureCandidates(List<char> sureCandidates)
    {
      foreach (var m in Members)
      {
        if (m.Value == ' ')
        {
          m.Candidates.RemoveAll(c => !sureCandidates.Contains((char)c));
        }
      }
    }
  }

  public class HStr8t : Str8t
  {
    public HStr8t(int r, int c, Board b) : base(r, c, b) { }

    public override (int, int) CellPos(int pos) => (_x, _y[pos]);
    
    public override string GetId() => $"H{base.GetId()}";
    public override string ToString()
    {
      var cells = string.Join("", _y.Select(c => _board._board[_x, c] == ' ' ? '.' : _board._board[_x,c]));
      var pos = $"{_x}:{string.Join("-", _y)}";
      return $"H{pos}:{cells}";
    }

    public override bool IsHorizontal => true;
    public override bool IsVertical => false;

    public override string Cells => string.Join("", _y.Select(c => _board._board[_x, c]));
    public override string CellsOptions(char[] options)
    {
        int cnt = 0;
        return string.Join("", _y.Select(c =>
        {
          char v = _board._board[_x, c];
          if (cnt >= options.Length)
            return v;
          return v != ' ' ? v : options[cnt++];
        }));
    }

    public override List<Str8t> PerpendicularStr8ts() => Members.Where(m => m.Vertical != null).Select(m => m.Vertical).ToList();

    public override List<char> GetValuesInRowOrCol()
    {
      var rc = base.GetValuesInRowOrCol();

      foreach (var s in _board.Str8ts.Where(s => s is HStr8t && _x == s._x))
      {
        if (s != this)
        {
          rc.AddRange(s.CertainCells());
        }
      }
      return rc;
    }

    public override List<Str8t> GetPerpendicularStr8ts(Cell c)
    {
      var str8ts = new List<Str8t>();
      var row = _board.Rows.Where(r => r.IsVertical && r.Idx == c.Col).FirstOrDefault();
      if (row != null)
        str8ts.AddRange(row.Str8ts);
      return str8ts;
    }

    public override Str8t? GetPerpendicularStr8t(Cell m) => m.Vertical;
  }

  public class VStr8t : Str8t
  {
    public VStr8t(int r, int c, Board b) : base(r, c, b) { }

    public override (int, int) CellPos(int pos) => (_y[pos], _x);
    public override string GetId() => $"V{base.GetId()}";
    public override string ToString()
    {
      var cells = string.Join("", _y.Select(c => _board._board[c, _x] == ' ' ? '.' : _board._board[c, _x]));
      var pos = $"{_x}:{string.Join("-", _y)}";
      return $"V{pos}:{cells}";
    }
    public override string Cells => string.Join("", _y.Select(c => _board._board[c, _x]));
    public override string CellsOptions(char[] options)
    {
      int cnt = 0;
      return string.Join("", _y.Select(c =>
      {
        char v = _board._board[c, _x];
        if (cnt >= options.Length)
          return v;
        return v != ' ' ? v : options[cnt++];
      }));
    }
    public override List<Str8t> PerpendicularStr8ts() => Members.Where(m => m.Horizontal != null).Select(m => m.Horizontal).ToList();
    public override bool IsHorizontal => false;
    public override bool IsVertical => true;

    public override List<char> GetValuesInRowOrCol()
    {
      var rc = base.GetValuesInRowOrCol();

      foreach (var s in _board.Str8ts.Where(s => s is VStr8t && _x == s._x))
      {
        if (s != this)
        {
          //rc.AddRange(s.GetNakedPairs());
          rc.AddRange(s.CertainCells());
        }
      }
      return rc;
    }

    public override List<Str8t> GetPerpendicularStr8ts(Cell c)
    {
      var str8ts = new List<Str8t>();
      var row = _board.Rows.Where(r => r.IsHorizontal && r.Idx == c.Row).FirstOrDefault();
      if (row != null)
        str8ts.AddRange(row.Str8ts);
      return str8ts;
    }

    public override Str8t? GetPerpendicularStr8t(Cell m) => m.Horizontal;
  }
}
