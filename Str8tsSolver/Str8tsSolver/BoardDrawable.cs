﻿using System.Collections.Concurrent;
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

  public enum SolverState
  {
    None,
    Scanning,
    Scanned,
    Analyzed,
    Solving,
    Finished
  }

  public class BoardDrawable : IDrawable
  {
    private SolverState _state = SolverState.None;

    private Square _board;
    private Square[,] _cells = new Square[9, 9];
    private bool _solved = false;
    private int _solvingProgress = 0;
    private int _counter = 0;

    private double _scaleX;
    private double _scaleY;

    private List<GridValue> _digits = new List<GridValue>();
    private ConcurrentBag<GridValue> _gridVals = new ConcurrentBag<GridValue>();

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
      if (_state == SolverState.None)
      {
        return;
      }

      canvas.FontSize = 20;
      canvas.StrokeSize = 2;

      if (_state == SolverState.Scanning)
      {
        canvas.FontSize = 10;
        canvas.FontColor = Colors.CornflowerBlue;
        canvas.DrawString($"Scanning {_counter}", 10, 10, HorizontalAlignment.Left);

        return;
      }

      if (_state == SolverState.Solving)
      {
        canvas.FontColor = Colors.CornflowerBlue;
        var chars = new char[] { '|', '/', '-', '\\' };
        var idx = _solvingProgress % 4;
        canvas.DrawString($"{chars[idx]}", 10, 10, HorizontalAlignment.Left);
      }
      if (_state == SolverState.Finished || _state == SolverState.Solving)
      {
        canvas.FontColor = _solved ? Colors.Green : Colors.Red;
        foreach (var gv in _gridVals)
        {
          canvas.DrawString(gv.Value.ToString(), _cells[gv.X, gv.Y].Rect, HorizontalAlignment.Center, VerticalAlignment.Center);
        }

        return;
      }

      if (_state == SolverState.Scanned)
      {
        if (_board != null)
        {
          canvas.StrokeColor = _solved ? Colors.Green : Colors.DarkOrange;
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
        }
      }

      if (_state == SolverState.Analyzed)
      {
        if (_digits != null && _digits.Count > 0)
        {
          canvas.FontColor = Colors.Red;
          foreach (var digit in _digits)
          {
            canvas.DrawString($"{digit.Value}", _cells[digit.X, digit.Y].Rect, HorizontalAlignment.Center, VerticalAlignment.Center);
          }
        }

        return;
      }

      if (_board == null)
      {
        canvas.DrawRectangle(dirtyRect);
        return;
      }

      void DrawLine(System.Drawing.Point a, System.Drawing.Point b) => canvas.DrawLine((float)a.X, (float)a.Y, (float)b.X, (float)b.Y);
    }
    
    internal void SetBoardContour (List<System.Drawing.Point> corners, double viewWidth, double viewHeight, int imageWidth, int imageHeight)
    {
      if (corners.Count != 4)
      {
        _board = null;
        return;
      }

      // Berechnen der Skalierungsfaktoren
      _scaleX = viewWidth / imageWidth;
      _scaleY = viewHeight / imageHeight;

      _board = new Square { UpperLeft = corners[0], UpperRight = corners[1], LowerRight = corners[2], LowerLeft = corners[3] };
      _board.ScaleToView(_scaleX, _scaleY);
      _cells = _board.SplitIntoCells(9, 9);

      _state = SolverState.Scanned;
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
      _state = SolverState.Analyzed;
    }
 
    internal void InvalidatePosition (int counter, string msg = "")
    {
      _state = SolverState.Scanning;
      _solved = false;
      _counter = counter;
      _board = null;
      _digits?.Clear();
      _gridVals.Clear();
    }

    internal void PositionSolved(int x, int y, char newValue)
    {
      _gridVals.Add(new GridValue { X = x, Y = y, Value = newValue });
      _state = SolverState.Solving;
    }

    internal void PuzzleSolved (bool solved)
    {
      _solved = solved;
      _state = SolverState.Finished;
    }

    internal void SolvingProgress(string currStr8t)
    {
      _state = SolverState.Solving;
      _solvingProgress++;
    }
  }
}
