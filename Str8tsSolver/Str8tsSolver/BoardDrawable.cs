using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace Str8tsSolver
{
  public class BoardDrawable : IDrawable
  {
    private Microsoft.Maui.Graphics.Point? _upperLeft;
    private Microsoft.Maui.Graphics.Point? _lowerLeft;
    private Microsoft.Maui.Graphics.Point? _upperRight;
    private Microsoft.Maui.Graphics.Point? _lowerRight;

    private int _counter = 0;

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
      canvas.StrokeColor = Colors.Blue;
      canvas.StrokeSize = 4;

      // draw the _counter value as text
      canvas.FontColor = Colors.Black;
      canvas.FontSize = 20;
      var x = dirtyRect.X + dirtyRect.Width / 2;
      var y = dirtyRect.Y + dirtyRect.Height / 2;
      canvas.DrawString(_counter.ToString(), x, y, HorizontalAlignment.Center);

      if (_upperLeft == null || _upperRight == null || _lowerRight == null || _lowerLeft == null)
      {
        canvas.DrawRectangle (dirtyRect);
        return;
      }

      canvas.StrokeColor = Colors.Green;
      // draw 4 lines that outline the rectangle
      canvas.DrawLine(_upperLeft.Value, _upperRight.Value);
      canvas.DrawLine(_upperRight.Value, _lowerRight.Value);
      canvas.DrawLine(_lowerRight.Value, _lowerLeft.Value);
      canvas.DrawLine(_lowerLeft.Value, _upperLeft.Value);
    }

    internal void UpdatePosition(List<System.Drawing.Point> corners, double viewWidth, double viewHeight, int imageWidth, int imageHeight, int counter)
    {
      _counter = counter;
      if (corners.Count != 4)
      {
        _upperLeft = null;
        _upperRight = null;
        _lowerRight = null;
        _lowerLeft = null;
        return;
      }

      // Berechnen der Skalierungsfaktoren
      var scaleX = viewWidth / imageWidth;
      var scaleY = viewHeight / imageHeight;

      _upperLeft = new Microsoft.Maui.Graphics.Point(corners[0].X * scaleX, corners[0].Y * scaleY);
      _upperRight = new Microsoft.Maui.Graphics.Point(corners[1].X * scaleX, corners[1].Y * scaleY);
      _lowerRight = new Microsoft.Maui.Graphics.Point(corners[2].X * scaleX, corners[2].Y * scaleY);
      _lowerLeft = new Microsoft.Maui.Graphics.Point(corners[3].X * scaleX, corners[3].Y * scaleY);
    }
  }
}
