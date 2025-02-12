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
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
      canvas.StrokeColor = Colors.Red;
      canvas.StrokeSize = 4;
      var rect = new RectF(dirtyRect.X + 10, dirtyRect.Y + 10, dirtyRect.Width - 20, dirtyRect.Height - 20);
      canvas.DrawRectangle (dirtyRect);
    }
  }
}
