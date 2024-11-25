using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Str8tsSolver
{
  public abstract class Row
  {
    List<Str8t> _str8ts;
    public int Idx { get; private set; }

    public Row(int idx)
    {
      Idx = idx;
      _str8ts = new List<Str8t>();
    }

    public abstract bool IsHorizontal { get; }
    public abstract bool IsVertical { get; }

    public void AddStr8t(Str8t str8t) => _str8ts.Add(str8t);
    public List<Str8t> Str8ts => _str8ts;
  }

  public class HRow : Row
  {
    public HRow(int idx) : base(idx) { }

    public int Col => Idx;
    public override bool IsHorizontal => true;
    public override bool IsVertical => false;
  }

  public class VRow : Row
  {
    public VRow(int idx) : base(idx) { }

    public int Row => Idx;
    public override bool IsHorizontal => false;
    public override bool IsVertical => true;
  }
}
