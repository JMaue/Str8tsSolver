using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Str8tsSolverLib
{
  public abstract class Row
  {
    List<Str8t> _str8ts;
    public int Idx { get; private set; }
    protected Cell[] _cells = new Cell[9];
    public Row(int idx)
    {
      Idx = idx;
      _str8ts = new List<Str8t>();
    }

    public abstract void SetCells(Board b);

    public abstract bool IsHorizontal { get; }
    public abstract bool IsVertical { get; }

    public void AddStr8t(Str8t str8t) => _str8ts.Add(str8t);
    public List<Str8t> Str8ts => _str8ts;

    public List<char> GetDecidedValues()
    {
      var rc = Enumerable.Range(0, 9)
                 .Where(i => _cells[i].Value >= '1' && _cells[i].Value <= '9')
                 .Select(i => _cells[i].Value).ToList();
      rc.AddRange(Enumerable.Range(0, 9)
                 .Where(i => _cells[i].Value >= 'A' && _cells[i].Value <= 'I')
                 .Select(i => (char)(_cells[i].Value - 'A' + '1')));
      return rc;
    }

    public bool WouldEraseCandidates(char[] values, List<int> positions)
    {
      foreach (int i in positions)
      {
        if (_cells[i].Value == ' ')
        {
          if (_cells[i].WouldEraseCandidates(values))
            return true;
        }
      }
      return false;
    }
  
    public List<char> GetCertainCandidatesFromSize()
    {
      if (NoOfBlackCells == 1 && NoOfStr8ts == 2)
      {
        if (Str8ts[0].Len == 2 || Str8ts[0].Len == 6)
          return new List<char> { '2', '4', '5', '6', '8' };
        if (Str8ts[0].Len == 3 || Str8ts[0].Len == 5)
          return new List<char> { '2', '3', '5', '7', '8' };
        if (Str8ts[0].Len == 4) // || Str8ts[0].Len == 4)
          return new List<char> { '2', '3', '4', '6', '7', '8' };
      }
      if (NoOfBlackCells == 2 && NoOfStr8ts == 3)
        return new List<char> { '1', '2', '5', '8', '7', '9' };

      // todo : 3 black cells and 2 rows
      return new List<char>();
    }

    public int NoOfBlackCells => _cells.Count(c => c.IsBlack);
    public int NoOfStr8ts => _str8ts.Count;
  }

  public class HRow : Row
  {
    public HRow(int idx) : base(idx) { }

    public int Row => Idx;
    public override bool IsHorizontal => true;
    public override bool IsVertical => false;
    public override void SetCells(Board b)
    {
      for (int i = 0; i < 9; i++)
      {
        _cells[i] = b._grid[Row, i];
      }
    }

    public override string ToString() => $"HRow {Idx}; {string.Join(";", Str8ts)}";
  }

  public class VRow : Row
  {
    public VRow(int idx) : base(idx) { }

    public int Col => Idx;
    public override bool IsHorizontal => false;
    public override bool IsVertical => true;
    public override void SetCells(Board b)
    {
      for (int i = 0; i < 9; i++)
      {
        _cells[i] = b._grid[i, Col];
      }
    }

    public override string ToString() => $"VRow {Idx}; {string.Join(";",Str8ts)}";
  }
}
