using System.Collections.Concurrent;
using Plugin.Maui.OCR;
using Str8tsSolverImageTools;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Plugin.Maui.OCR.OcrResult;
using OcrElement = Str8tsSolverImageTools.OcrElement;

namespace Str8tsSolver
{
  public static class OcrElementExt 
  {
     public static string ToString (this OcrElement ocrElement) => $"{ocrElement.X},{ocrElement.Y}";
  }

  public class BoardDrawable : IDrawable
  {
    //private Point? _upperLeft;
    //private Point? _lowerLeft;
    //private Point? _upperRight;
    //private Point? _lowerRight;
    private Square _board;
    private Square[,] _cells = new Square[9, 9];

    private int _counter = 0;
    private string _message;

    private double _scaleX;
    private double _scaleY;

    private List<GridValue> _digits = new List<GridValue>();
    private ConcurrentBag<GridValue> _gridVals = new ConcurrentBag<GridValue>();
    private List<string> _valid = ["1", "2", "3", "4", "5", "6", "/", "8", "9"];

    private List<string> l1 = new List<string>();
    private List<string> l2 = new List<string>();

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
      canvas.StrokeColor = Colors.Blue;
      canvas.StrokeSize = 2;

      // draw the _counter value as text
      canvas.FontColor = Colors.Black;
      canvas.FontSize = 20;

      if (_digits != null && _digits.Count > 0)
      {
        canvas.FontColor = Colors.Green;
        l1.Clear();
        foreach (var digit in _digits)
        {
          //var y = 30 + digit.X * _scaleX;
          //var x = 30 + _upperRight.Value.X - (digit.Y * _scaleY);
          //l1.Add($"´{digit.Text}:{(int)x},{(int)y}");
          //canvas.DrawString(digit.Text, (float)x, (float)y, HorizontalAlignment.Center);

          var (x, y) = _cells[digit.X, digit.Y].Center;
          //var y = _cells[digit.X, digit.Y].UpperLeft.Y + 5;
          canvas.DrawString($"{digit.Value}", (float)x, (float)y, HorizontalAlignment.Center);
        }
      }
      if (_gridVals.Count > 0)
      {
        canvas.FontColor = Colors.Red;
        l2.Clear();
        foreach (var gv in _gridVals)
        {
          //var (x, y) = ViewHelper.GridPos2ViewCoo(gv.X, gv.Y, _upperLeft, _upperRight, _lowerLeft, _lowerRight);
          //l2.Add($"{gv.Value} ({gv.X}, {gv.Y}) : {x},{y}");
          //canvas.DrawString(gv.Value.ToString(), (float)x, (float)y, HorizontalAlignment.Center);

          var (x, y) = _cells[gv.X, gv.Y].Center;
          canvas.DrawString(gv.Value.ToString(), (float)x, (float)y, HorizontalAlignment.Center);
        }
      }
      //else
      //{
      //  var x = dirtyRect.X + dirtyRect.Width / 2;
      //  var y = dirtyRect.Y + dirtyRect.Height / 2;
      //  var msg = _message == null || _message.Length == 0 ? _counter.ToString() : _message;
      //  canvas.DrawString(msg, x, y, HorizontalAlignment.Center);
      //}

      if (_board == null)
      {
        canvas.DrawRectangle (dirtyRect);
        return;
      }

      canvas.StrokeColor = Colors.Green;
      // draw 4 lines that outline the rectangle
      DrawLine(_board.UpperLeft, _board.UpperRight);
      DrawLine(_board.UpperRight, _board.LowerRight);
      DrawLine(_board.LowerRight, _board.LowerLeft);
      DrawLine(_board.LowerLeft, _board.UpperLeft);

      for (int i = 0; i < 9; i++)
      {
        for (int j = 0; j < 9; j++)
        {
          DrawLine(_cells[i, j].UpperLeft, _cells[i, j].UpperRight);
          DrawLine(_cells[i, j].UpperRight, _cells[i, j].LowerRight);
          DrawLine(_cells[i, j].LowerRight, _cells[i, j].LowerLeft);
          DrawLine(_cells[i, j].LowerLeft, _cells[i, j].UpperLeft);
        }
      }
      void DrawLine(System.Drawing.Point a, System.Drawing.Point b) => canvas.DrawLine((float)a.X, (float)a.Y, (float)b.X, (float)b.Y);
    }
    
    internal void SetBoardContour (List<System.Drawing.Point> corners, double viewWidth, double viewHeight, int imageWidth, int imageHeight)
    {
      if (corners.Count != 4)
      {
        //_upperLeft = null;
        //_upperRight = null;
        //_lowerRight = null;
        //_lowerLeft = null;
        _board = null;
        return;
      }

      // Berechnen der Skalierungsfaktoren
      _scaleX = viewWidth / imageWidth;
      _scaleY = viewHeight / imageHeight;

      _board = new Square { UpperLeft = corners[0], UpperRight = corners[1], LowerRight = corners[2], LowerLeft = corners[3] };
      _board.ScaleToView(_scaleX, _scaleY);
      //_upperLeft = new Point(corners[0].X * _scaleX, corners[0].Y * _scaleY);
      //_upperRight = new Point(corners[1].X * _scaleX, corners[1].Y * _scaleY);
      //_lowerRight = new Point(corners[2].X * _scaleX, corners[2].Y * _scaleY);
      //_lowerLeft = new Point(corners[3].X * _scaleX, corners[3].Y * _scaleY);

      _cells = _board.SplitIntoCells(9, 9);
    }

    internal void SetBoard(char[,] board)
    {
      for (int r=0; r<9; r++)
      {
        for (int c=0; c<9; c++)
        {
          if (board[r,c] != ' ')
            _digits.Add(new GridValue { X = r, Y = c, Value = board[r,c] }); 
        }
      }
    }
    internal void UpdatePosition(OcrResult ocr)
    {
      _message = string.Empty;

      //_digits = OcrResultValidation.PickValidElements(ocr); //, corners);
    }
 
    internal void InvalidatePosition (int counter, string msg = "")
    {
      _counter = counter;
      //_upperLeft = null;
      //_upperRight = null;
      //_lowerRight = null;
      //_lowerLeft = null;
      _board = null;
      _message = msg;
      _digits?.Clear();
    }

    internal void PositionSolved(int x, int y, char newValue)
    {
      _gridVals.Add(new GridValue { X = x, Y = y, Value = newValue });
    }
  }
}
