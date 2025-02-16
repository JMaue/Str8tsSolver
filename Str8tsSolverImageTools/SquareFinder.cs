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
  }
}