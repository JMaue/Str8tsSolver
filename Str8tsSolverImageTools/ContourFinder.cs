using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV;

using Point = System.Drawing.Point;

namespace Str8tsSolverImageTools
{
  //public enum ShowIntermediateResults : Int16
  //{
  //  LabelCorner = 1,
  //  CornerCycle = 2,
  //  DrawAllContours = 4,
  //  DrawRois = 8,
  //  ShowOcrResults = 16
  //}

  public class ContourFinder : IDisposable
  {
    List<Point> _contour;

    public delegate void NumberDetectedHandler(int x, int y, char value);
    public event NumberDetectedHandler NumberDetected;

    public ContourFinder()
    {
    }

    public void Dispose()
    {
    }

    public Int16 ShowIntermediates { get; set; } = 0;

    public List<Point> FindExternalContour(byte[] rawBytes, out int width, out int height)
    {
      Mat? image = BytesArrayToMat(rawBytes);
      width = image != null ? image.Width : 0;
      height = image != null ? image.Height : 0;
      return FindExternalContour(ref image);
    }

    public List<Point> FindExternalContour(ref Mat? img)
    {
      if (img == null)
        return new List<Point>();

      var rc = new List<Point>();
      var imgX = img.Rows;
      var imgY = img.Cols;
      var minSquareSize = Math.Min(imgX, imgY) * 0.7;

      // convert image to gray
      using (Mat gray = new Mat())
      {
        CvInvoke.CvtColor(img, gray, ColorConversion.Bgr2Gray);
        var graySmooth = gray.ToImage<Gray, byte>().SmoothGaussian(5, 5, 1.5, 1.5);

        // detect edges using the Canny-Algorithm 
        using (Mat cannyEdges = new Mat())
        {
          CvInvoke.Canny(graySmooth, cannyEdges, 50, 150);
          using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
          {
            CvInvoke.FindContours(cannyEdges, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);
            for (int i = 0; i < contours.Size; i++)
            {
              using VectorOfPoint contour = contours[i];
              using VectorOfPoint approxContour = new VectorOfPoint();
              CvInvoke.ApproxPolyDP(contour, approxContour, CvInvoke.ArcLength(contour, true) * 0.08, true);

              // check if contour has the expected minimal size (70% of the image size)
              var maxX = 0;
              var maxY = 0;
              var minX = imgX;
              var minY = imgY;
              foreach (var point in approxContour.ToArray())
              {
                maxX = Math.Max(maxX, point.X);
                maxY = Math.Max(maxY, point.Y);
                minX = Math.Min(minX, point.X);
                minY = Math.Min(minY, point.Y);
              }
              if (maxX - minX >= minSquareSize && maxY - minY >= minSquareSize)
              {
                if ((ShowIntermediates & (Int16)ShowIntermediateResults.DrawAllContours) != 0)
                  CvInvoke.DrawContours(img, contours, i, new MCvScalar(0, 0, 255), 2);

                foreach (var point in approxContour.ToArray())
                {
                  if ((ShowIntermediates & (Int16) ShowIntermediateResults.CornerCycle) != 0)
                  {
                    var d = imgY > 1000 ? 15 : 5;
                    CvInvoke.Circle(img, point, d, new MCvScalar(0, 255, 0), 5);
                  }

                  rc.Add(point);
                }
                if (rc.Count == 4)
                  break;
              }
            }
          }
        }
      }

      return FindCornerPoints(rc);
    }

    public List<Point> FindCornerPoints(List<Point> contour)
    {
      if (contour.Count != 4)
        return new List<Point>();

      Point upperLeft = new Point(0, 0);
      Point upperRight = new Point(0, 0);
      Point lowerLeft = new Point(0, 0);
      Point lowerRight = new Point(0, 0);

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
          upperLeft = point;
        }
        if (point.X > meanX && point.Y < meanY)
        {
          upperRight = point;
        }
        if (point.X < meanX && point.Y > meanY)
        {
          lowerLeft = point;
        }
        if (point.X > meanX && point.Y > meanY)
        {
          lowerRight = point;
        }
      }

      _contour = new List<Point> { upperLeft, upperRight, lowerRight, lowerLeft };
      return _contour;
    }


    public Mat? BytesArrayToMat(byte[]  rawBytes)
    {
      Mat mat = new Mat();
      CvInvoke.Imdecode(rawBytes, ImreadModes.Color, mat);
      return mat;
    }

    public bool IsScreenShot(List<Point> contour)
    {
      var corners = FindCornerPoints(contour);
      Point upperLeft = corners[0];
      Point upperRight = corners[1];
      Point lowerRight = corners[2];
      Point lowerLeft = corners[3];

      return upperLeft.Y == upperRight.Y && lowerLeft.Y == lowerRight.Y && upperLeft.X == lowerLeft.X && upperRight.X == lowerRight.X;
    }
  }
}

