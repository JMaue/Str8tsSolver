using Microsoft.Maui.Controls;
using Plugin.Maui.OCR;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using static Plugin.Maui.OCR.OcrResult;

namespace Str8tsSolver
{
  public static class OcrElementExt 
  {
     public static string ToString (this OcrElement ocrElement) => $"{ocrElement.X},{ocrElement.Y}";
  }

  public class BoardDrawable : IDrawable
  {
    private Microsoft.Maui.Graphics.Point? _upperLeft;
    private Microsoft.Maui.Graphics.Point? _lowerLeft;
    private Microsoft.Maui.Graphics.Point? _upperRight;
    private Microsoft.Maui.Graphics.Point? _lowerRight;

    private int _counter = 0;
    private string _message;

    private double _scaleX;
    private double _scaleY;

    private List<OcrElement> _digits;
    private List<string> _valid = ["1", "2", "3", "4", "5", "6", "/", "8", "9"];

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
      canvas.StrokeColor = Colors.Blue;
      canvas.StrokeSize = 2;

      // draw the _counter value as text
      canvas.FontColor = Colors.Black;
      canvas.FontSize = 20;

      if (_digits != null && _digits.Count > 0)
      {
        var l = new List<string>();
        canvas.FontColor = Colors.Green;
        foreach (var digit in _digits)
        {
          var y = 30 + digit.X * _scaleX;
          var x = 30 + _upperRight.Value.X - (digit.Y * _scaleY);
          l.Add($"´{digit.Text}:{(int)x},{(int)y}");
          canvas.DrawString(digit.Text, (float)x, (float)y, HorizontalAlignment.Center);
        }
      }
      else
      {
        var x = dirtyRect.X + dirtyRect.Width / 2;
        var y = dirtyRect.Y + dirtyRect.Height / 2;
        var msg = _message == null || _message.Length == 0 ? _counter.ToString() : _message;
        canvas.DrawString(msg, x, y, HorizontalAlignment.Center);
      }

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

    internal void UpdatePosition(List<System.Drawing.Point> corners, double viewWidth, double viewHeight, int imageWidth, int imageHeight, int counter, OcrResult ocr)
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
      _scaleX = viewWidth / imageWidth;
      _scaleY = viewHeight / imageHeight;

      _upperLeft = new Microsoft.Maui.Graphics.Point(corners[0].X * _scaleX, corners[0].Y * _scaleY);
      _upperRight = new Microsoft.Maui.Graphics.Point(corners[1].X * _scaleX, corners[1].Y * _scaleY);
      _lowerRight = new Microsoft.Maui.Graphics.Point(corners[2].X * _scaleX, corners[2].Y * _scaleY);
      _lowerLeft = new Microsoft.Maui.Graphics.Point(corners[3].X * _scaleX, corners[3].Y * _scaleY);

      _message = string.Empty;

      _digits = ocr.Elements.Where(e => _valid.Contains(e.Text.Trim())).ToList();
    }
 
    internal void InvalidatePosition (int counter, string msg = "")
    {
      _counter = counter;
      _upperLeft = null;
      _upperRight = null;
      _lowerRight = null;
      _lowerLeft = null;
      _message = msg;
      _digits?.Clear();
    }
  }
}
