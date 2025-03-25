using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Str8tsSolver.Layouts
{
  public class ResponsiveGridLayoutManager : ILayoutManager
  {
    private ResponsiveGridLayout _layout;
    bool _isLandscape = false;

    public ResponsiveGridLayoutManager (ResponsiveGridLayout layout) 
    {
      _layout = layout;
    }

    public Size Measure (double widthConstraint, double heightConstraint)
    {
      _isLandscape = widthConstraint > heightConstraint;
      foreach (var child in _layout)
      {
        var current = child.Measure (widthConstraint, heightConstraint);
      }
      return new Size (widthConstraint, heightConstraint);
    }

    public Size ArrangeChildren (Rect bounds)
    {
      if (_isLandscape)
      {
        var child1 = _layout[0];
        child1.Arrange(new Rect(0, 0, 80, 40));

        var child2 = _layout[1];
        child2.Arrange(new Rect(0, 50, 80, 40));

        var child3 = _layout[2];
        child3.Arrange(new Rect(0, 100, 80, 40));

        var child4 = _layout[3];
        child4.Arrange(new Rect(0, 150, 80, 40));

        for (int i=4; i<=6; i++)
        {
          var child = _layout[i];
          child.Arrange(new Rect(100, 0, 360, 480));
        }
      }
      else
      {
        var child1 = _layout[0];
        child1.Arrange(new Rect(0, 0, 80, 40));
        var child2 = _layout[1];
        child2.Arrange(new Rect(90, 0, 80, 40));
        var child3 = _layout[2];
        child3.Arrange(new Rect(180, 0, 80, 40));
        var child4 = _layout[3];
        child4.Arrange(new Rect(270, 0, 80, 40));

        for (int i = 4; i <= 6; i++)
        {
          var child = _layout[i];
          child.Arrange(new Rect(0, 50, 360, 480));
        }
      }

      return bounds.Size;
    }
  }
}
