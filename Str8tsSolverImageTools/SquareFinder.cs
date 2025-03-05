using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Emgu.CV.Flann.Index3D;
using Point = System.Drawing.Point;

namespace Str8tsSolverImageTools
{
  public class SquareFinder
  {
    List<Point>[] cornerCandidates = new List<Point>[4];

    public SquareFinder(List<Point> contour)
    {
      for (int i = 0; i < 4; i++)
        cornerCandidates[i] = new List<Point>();

      var minX = contour.Select(p => p.X).Min();
      var maxX = contour.Select(p => p.X).Max();
      var minY = contour.Select(p => p.Y).Min();
      var maxY = contour.Select(p => p.Y).Max();
      var meanX = (minX + maxX) / 2;
      var meanY = (minY + maxY) / 2;
      foreach (var point in contour)
      {
        if (point.X < meanX && point.Y < meanY)
        {
          cornerCandidates[0].Add(point);
        }
        if (point.X > meanX && point.Y < meanY)
        {
          cornerCandidates[1].Add(point);
        }
        if (point.X < meanX && point.Y > meanY)
        {
          cornerCandidates[3].Add(point);
        }
        if (point.X > meanX && point.Y > meanY)
        {
          cornerCandidates[2].Add(point);
        }
      }
    }

    public Point ValidateCorner(List<Point> cornerCandidates)
    {
      if (cornerCandidates.Count == 0)
        return Point.Empty;
      var meanX = cornerCandidates.Select(p => p.X).Average();
      var meanY = cornerCandidates.Select(p => p.Y).Average();
      var varianceX = cornerCandidates.Select(p => Math.Pow(p.X - meanX, 2)).Sum() / cornerCandidates.Count;
      var varianceY = cornerCandidates.Select(p => Math.Pow(p.Y - meanY, 2)).Sum() / cornerCandidates.Count;
      var stdDevX = Math.Sqrt(varianceX);
      var stdDevY = Math.Sqrt(varianceY);
      var corner = cornerCandidates.OrderBy(p => Math.Abs(p.X - meanX) / stdDevX + Math.Abs(p.Y - meanY) / stdDevY).First();
      return corner;
    }

    public List<Point> GetValidSquare()
    {
      var upperLeft = ValidateCorner(cornerCandidates[0]);
      var upperRight = ValidateCorner(cornerCandidates[1]);
      var lowerRight = ValidateCorner(cornerCandidates[2]);
      var lowerLeft = ValidateCorner(cornerCandidates[3]);

      var ret = new List<Point> { upperLeft, upperRight, lowerRight, lowerLeft };
      if (ret.All(c => !c.IsEmpty))
        return ret;

      // not all corners are valid, so go ahead and try fix it with the sides
      if (ret.Where(c => c.IsEmpty).Count() == 1)
      {
        var emptyIndex = ret.FindIndex(c => c.IsEmpty);
        var diagIdx = (emptyIndex + 2) % 4;
        var s1 = new Side(ret[diagIdx], ret[(emptyIndex + 1) % 4]);
        var s2 = new Side(ret[diagIdx], ret[(emptyIndex + 3) % 4]);
        var dx = s1.DX + s2.DX;
        var dy = s1.DY + s2.DY;

        ret[emptyIndex] = new Point(ret[diagIdx].X + dx, ret[diagIdx].Y + dy);

        return ret;
      }

      return new List<Point>();
    }
  }

  public class Side
  {
    public Point A { get; }
    public Point B { get; }
    public Side(Point a, Point b)
    {
      A = a;
      B = b;
    }

    public int DX => B.X - A.X;
    public int DY => B.Y - A.Y;

    public List<Point> Split(int parts)
    {
      var res = new List<Point>();
      for (int i = 0; i <= parts; i++)
      {
        res.Add(new Point(A.X + i * DX / parts, A.Y + i * DY / parts));
      }
      return res;
    }
  }

  public class Square
  {
    public Point UpperLeft { get; set; }
    public Point UpperRight { get; set; }
    public Point LowerRight { get; set; }
    public Point LowerLeft { get; set; }

    public Rect Rect => new Rect(UpperLeft.X, UpperRight.Y, UpperRight.X - UpperLeft.X, LowerRight.Y - UpperRight.Y);
    public (int, int) Center => ((UpperLeft.X + UpperRight.X + LowerLeft.X + LowerRight.X) / 4, (UpperLeft.Y + UpperRight.Y + LowerLeft.Y + LowerRight.Y) / 4);

    public void ScaleToView(double scaleX, double scaleY)
    {
      UpperLeft = new Point((int)(UpperLeft.X * scaleX), (int)(UpperLeft.Y * scaleY));
      UpperRight = new Point((int)(UpperRight.X * scaleX), (int)(UpperRight.Y * scaleY));
      LowerLeft = new Point((int)(LowerLeft.X * scaleX), (int)(LowerLeft.Y * scaleY));
      LowerRight = new Point((int)(LowerRight.X * scaleX), (int)(LowerRight.Y * scaleY));
    }

    public Square[,] SplitIntoCells(int row, int cols)
    {
      var res = new Square[9, 9];
      var leftSide = new Side(UpperLeft, LowerLeft).Split(9);
      var rightSide = new Side(UpperRight, LowerRight).Split(9);
      for (int r = 0; r < row; r++)
      {
        var upperSide = new Side(leftSide[r], rightSide[r]).Split(9);
        var lowerSide = new Side(leftSide[r + 1], rightSide[r + 1]).Split(9);

        for (int c = 0; c < cols; c++)
        {
          var upperLeft = upperSide[c];
          var upperRight = upperSide[c + 1];
          var lowerLeft = lowerSide[c];
          var lowerRight = lowerSide[c + 1];
          res[r, c] = new Square { UpperLeft = upperLeft, UpperRight = upperRight, LowerLeft = lowerLeft, LowerRight = lowerRight };
        }
      }

      return res;
    }
  }
}