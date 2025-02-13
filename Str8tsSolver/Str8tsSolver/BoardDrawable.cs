using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Str8tsSolver
{
  public class BoardDrawable : IDrawable
  {
    private int _x;
    private int _y;
    public void UpdatePosition (int  x, int y)
    {
      _x = x;
      _y = y;
    }
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
      canvas.StrokeColor = Colors.Red;
      canvas.StrokeSize = 4;
      var rect = new RectF(_x + 10, _y + 10, 20, 20);
      canvas.DrawRectangle (rect);
    }
  }
}
