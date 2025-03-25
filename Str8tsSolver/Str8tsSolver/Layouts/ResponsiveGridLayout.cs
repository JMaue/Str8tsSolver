using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Str8tsSolver.Layouts
{
  public class ResponsiveGridLayout : Layout
  {
    protected override ILayoutManager CreateLayoutManager()
    {
      return new ResponsiveGridLayoutManager(this);
    }
  }
}
