using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV;

using Point = System.Drawing.Point;

namespace Str8tsSolverImageTools
{
  public class ContourFinder : IDisposable
  {
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
      //Mat image = CvInvoke.Imread(@"D:\Jens\Repositories\Str8tsSolver\Data\20250106_191616.jpg", ImreadModes.Color);
      width = image != null ? image.Width : 0;
      height = image != null ? image.Height : 0;
      return FindExternalContour(ref image);
    }

    public List<Point> FindExternalContour(string fileName)
    {
      Mat image = CvInvoke.Imread(fileName, ImreadModes.Color);
      return FindExternalContour(ref image);
    }

    public List<Point> FindExternalContour(ref Mat? img)
    { 
      if (img == null)
        return new List<Point>();

      var allContourCandidates = new List<List<Point>>();
      var imgX = img.Cols;
      var imgY = img.Rows;
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
              var contourCandidate = new List<Point>();

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

                  contourCandidate.Add(point);
                }
                if (contourCandidate.Count > 2)
                  allContourCandidates.Add(contourCandidate);
              }
            }
          }
        }
      }

      if (allContourCandidates.Count == 0)
        return new List<Point>();

      return FindCornerPoints(allContourCandidates[0]);
    }

    public List<Point> FindCornerPoints(List<Point> contour)
    {
      if (contour.Count < 3)
        return new List<Point>();

      var sf = new SquareFinder(contour);
      return sf.GetValidSquare();
    }

    private Point ValidateCorner(List<Point> cornerCandidates)
    {
      if (cornerCandidates.Count == 0)
        return Point.Empty;
      if (cornerCandidates.Count == 1)
        return cornerCandidates[0];

      float epsilon = 10;
      var dXmax = cornerCandidates.Select(c => c.X).Max();
      var dxMin = cornerCandidates.Select(c => c.X).Min();
      var dYmax = cornerCandidates.Select(c => c.Y).Max();
      var dyMin = cornerCandidates.Select(c => c.Y).Min();
      if (dXmax - dxMin < epsilon && dYmax - dyMin < epsilon)
        return cornerCandidates[0];

      return Point.Empty;
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

