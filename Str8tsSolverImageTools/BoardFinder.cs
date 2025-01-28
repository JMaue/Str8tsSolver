using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV;
using System.Drawing;
using Emgu.CV.OCR;
using System.Diagnostics;

namespace Str8tsSolverImageTools
{
  public enum ShowIntermediateResults : Int16
  {
    LabelCorner = 1,
    CornerCycle = 2,
    DrawAllContours = 4,
    DrawRois = 8,
    ShowOcrResults = 16
  }

  public class BoardFinder : IDisposable
  {
    private readonly string _dataFolder;

    Tesseract? _ocr = new();

    // bottom left coordinates of each of the 9x9 grid cells to write the solution in the OnSolved callback
    readonly Point[,] _points = new Point[9, 9];
    int _ySizeBoard = 0;
    readonly MCvScalar _black = new MCvScalar(0, 0, 0);

    Mat _originalImage;
    List<Point> _contour;

    public delegate void NumberDetectedHandler(int x, int y, char value);
    public event NumberDetectedHandler NumberDetected;

    public BoardFinder(string dataFolder)
    {
      _dataFolder = dataFolder;
      InitOcr(Tesseract.DefaultTesseractDirectory, "eng", OcrEngineMode.TesseractOnly);
    }

    public void Dispose()
    {
      _ocr?.Dispose();
    }

    public Int16 ShowIntermediates { get; set; } = 0;

    private static void TesseractDownloadLangFile(String folder, String lang)
    {
      //String subfolderName = "tessdata";
      //String folderName = System.IO.Path.Combine(folder, subfolderName);
      String folderName = folder;
      if (!Directory.Exists(folderName))
      {
        Directory.CreateDirectory(folderName);
      }
      String dest = Path.Combine(folderName, $"{lang}.traineddata");
      if (!File.Exists(dest))
        using (System.Net.WebClient webclient = new System.Net.WebClient())
        {
          String source = Tesseract.GetLangFileUrl(lang);

          Trace.WriteLine($"Downloading file from '{source}' to '{dest}'");
          webclient.DownloadFile(source, dest);
          Trace.WriteLine("Download completed");
        }
    }

    private bool InitOcr(String path, String lang, OcrEngineMode mode)
    {

      try
      {
        _ocr?.Dispose();
        _ocr = null;

        if (String.IsNullOrEmpty(path))
          path = Tesseract.DefaultTesseractDirectory;

        TesseractDownloadLangFile(path, lang);
        TesseractDownloadLangFile(path, "osd"); //script orientation detection

        _ocr = new Tesseract(path, lang, mode);
        _ocr.SetVariable("tessedit_char_whitelist", "123456789");

        return true;
      }
      catch (Exception e)
      {
        _ocr = null;
        return false;
      }
    }

    public char[,] ReadBoardFromImage(string path)
    {
      _originalImage = CvInvoke.Imread(path, ImreadModes.Color);
      var contour = FindExternalContour(ref _originalImage);
      return Find81Fields(_originalImage, contour);
    }

    public List<Point> FindExternalContour(string file)
    {
      Mat image = CvInvoke.Imread(file, ImreadModes.Color);
      return FindExternalContour(ref image);
    }

    public List<Point> FindExternalContour(ref Mat img)
    {
      _originalImage = img;
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

    public char[,] Find81Fields(Mat originalImage, List<Point> contour)
    {
      var board = new char[9, 9];

      var corners = FindCornerPoints(contour);
      var isScreenShot = IsScreenShot(contour);
      Point upperLeft = corners[0];
      Point upperRight = corners[1];
      Point lowerRight = corners[2];
      Point lowerLeft = corners[3];

      if ((ShowIntermediates & (Int16)ShowIntermediateResults.LabelCorner) != 0)
      {
        CvInvoke.PutText(originalImage, "1", lowerRight, FontFace.HersheyPlain, 8, new MCvScalar(255, 0, 255), 4);
        CvInvoke.PutText(originalImage, "2", lowerLeft, FontFace.HersheyPlain, 8, new MCvScalar(255, 0, 255), 4);
        CvInvoke.PutText(originalImage, "3", upperRight, FontFace.HersheyPlain, 8, new MCvScalar(255, 0, 255), 4);
        CvInvoke.PutText(originalImage, "4", upperLeft, FontFace.HersheyPlain, 8, new MCvScalar(255, 0, 255), 4);
      }

      using (var grayImage = new Mat())
      {
        CvInvoke.CvtColor(originalImage, grayImage, ColorConversion.Bgr2Gray);

        (bool[,] blackValues, int average, int avgBlack, int avgWhite) =
          CalcBlackWhiteMatrix(grayImage, upperLeft, upperRight, lowerLeft, lowerRight);

        var dxr = (upperRight.X - lowerRight.X) / 9;
        var dyr = (lowerRight.Y - upperRight.Y) / 9; // always positiv
        var dxl = (upperLeft.X - lowerLeft.X) / 9;
        var dyl = (lowerLeft.Y - upperLeft.Y) / 9; // always positiv

        for (int r = 0; r < 9; r++)
        {
          var tl = new Point(upperLeft.X - r * dxl, upperLeft.Y + r * dyl);
          var tr = new Point(upperRight.X - r * dxr, upperRight.Y + r * dyr);
          var bl = new Point(upperLeft.X - (r + 1) * dxl, upperLeft.Y + (r + 1) * dyl);
          var br = new Point(upperRight.X - (r + 1) * dxr, upperRight.Y + (r + 1) * dyr);
          var dxt = (tr.X - tl.X) / 9;
          var dyt = (tr.Y - tl.Y) / 9;
          var dxb = (br.X - bl.X) / 9;
          var dyb = (br.Y - bl.Y) / 9;
          for (int c = 0; c < 9; c++)
          {
            // corner points of the ROI
            var topLeft = new Point(tl.X + c * dxt, tl.Y + c * dyt);
            var topRight = new Point(tl.X + (c + 1) * dxt, tl.Y + (c + 1) * dyt);
            var bottomLeft = new Point(bl.X + c * dxb, bl.Y + c * dyb);
            var bottomRight = new Point(bl.X + (c + 1) * dxb, bl.Y + (c + 1) * dyb);

            _ySizeBoard = bottomLeft.Y - topLeft.Y;

            if ((ShowIntermediates & (Int16) ShowIntermediateResults.DrawRois) != 0)
            {
              // draw rectangle
              CvInvoke.Line(originalImage, topLeft, topRight, new MCvScalar(0, 255, 0), 2);
              CvInvoke.Line(originalImage, topLeft, bottomLeft, new MCvScalar(0, 255, 0), 2);
              CvInvoke.Line(originalImage, bottomLeft, bottomRight, new MCvScalar(0, 255, 0), 2);
              CvInvoke.Line(originalImage, topRight, bottomRight, new MCvScalar(0, 255, 0), 2);
            }

            // define roi
            int shrink = Convert.ToInt16(dxt * 0.20); // take 20% off from all sides
            Rectangle rect = new Rectangle(topLeft.X + shrink, topLeft.Y + shrink,
                                        topRight.X - topLeft.X - (2 * shrink), bottomLeft.Y - topLeft.Y - (2 * shrink));

            int digit = -1;
            var isBlack = blackValues[r, c];
            using (Mat roi = new Mat(grayImage, rect))
            {
              SaveRegionToFile(roi, $"{r}{c}.png");

              board[r, c] = isBlack ? '#' : ' ';
              digit = isBlack
                ? GetDigitFromBlackCell(roi, r, c, avgWhite, isScreenShot)
                : GetDigitFromWhiteCell(roi, r, c, avgWhite, isScreenShot);
            }

            if (digit is >= 1 and <= 9)
            {
              if (isBlack)
                board[r, c] = (char) ('A' + digit - 1);
              else
                board[r, c] = (char) ('0' + digit);

              if ((ShowIntermediates & (Int16) ShowIntermediateResults.ShowOcrResults) != 0)
              {
                _points[r, c] = bottomLeft;
                NumberDetected?.Invoke(r, c, (char) ('0' + digit));
              }
            }

            // keep the bottom left coordinates of each of the 9x9 grid cells to write the solution in the OnSolved callback
            _points[r, c] = bottomLeft;
          }
        }
      }

      return board;
    }

    private (bool[,], int, int, int) CalcBlackWhiteMatrix(Mat image, Point upperLeft, Point upperRight, Point lowerLeft, Point lowerRight)
    {
      var dxr = (upperRight.X - lowerRight.X) / 9;
      var dyr = (lowerRight.Y - upperRight.Y) / 9;    // always positiv
      var dxl = (upperLeft.X - lowerLeft.X) / 9;
      var dyl = (lowerLeft.Y - upperLeft.Y) / 9;      // always positiv

      var isBlack = new bool[9, 9];

      void ProcessRectangles (Action<double, int, int> processAverage)
      {
        for (int r = 0; r < 9; r++)
        {
          var tl = new Point(upperLeft.X - r * dxl, upperLeft.Y + r * dyl);
          var tr = new Point(upperRight.X - r * dxr, upperRight.Y + r * dyr);
          var bl = new Point(upperLeft.X - (r + 1) * dxl, upperLeft.Y + (r + 1) * dyl);
          var br = new Point(upperRight.X - (r + 1) * dxr, upperRight.Y + (r + 1) * dyr);
          var dxt = (tr.X - tl.X) / 9;
          var dyt = (tr.Y - tl.Y) / 9;
          var dxb = (br.X - bl.X) / 9;
          var dyb = (br.Y - bl.Y) / 9;
          for (int c = 0; c < 9; c++)
          {
            // Berechne die Eckpunkte des Rechtecks
            var topLeft = new Point(tl.X + c * dxt, tl.Y + c * dyt);
            var topRight = new Point(tl.X + (c + 1) * dxt, tl.Y + (c + 1) * dyt);
            var bottomLeft = new Point(bl.X + c * dxb, bl.Y + c * dyb);
            var bottomRight = new Point(bl.X + (c + 1) * dxb, bl.Y + (c + 1) * dyb);

            _ySizeBoard = bottomLeft.Y - topLeft.Y;

            // Rechteck definieren
            var dMin = Math.Min(dxt, dyt);
            dMin = Math.Min(dMin, dxb);
            dMin = Math.Min(dMin, dyb);
            int shrink = Convert.ToInt16(dMin * 0.20); // take 20% off from all sides
            Rectangle rect = new Rectangle(topLeft.X + shrink, topLeft.Y + shrink, topRight.X - topLeft.X - (2 * shrink), bottomLeft.Y - topLeft.Y - (2 * shrink));

            // Durchschnittlichen Grauwert berechnen
            //var grayImage = new Mat();
            //CvInvoke.CvtColor(originalImage, grayImage, ColorConversion.Bgr2Gray);
            var average = GetAverageGrayValue(image, rect);
            processAverage(average, r, c);
          }
        }
      }

      int[] vals = new int[81];
      ProcessRectangles((average, r, c) => {
        vals[9*r+c] = (int)average;
      });
      var (blackCluster, whiteCluster) = KMeansClustering.ClassifyIntoClusters(vals);
      int avgWhite = whiteCluster.Sum() / whiteCluster.Length;
      int avgBlack = blackCluster.Sum() / blackCluster.Length;

      for (int i = 0; i < 81; i++)
      {
        isBlack[i/9, i%9] = blackCluster.Contains(vals[i]);
      }
      int mean = avgBlack + avgWhite / 2;
      return (isBlack, mean, avgBlack, avgWhite);
    }

    private static double GetAverageGrayValue(Mat grayImage, Rectangle rect)
    {
      // extract ROI
      Mat roi = new Mat(grayImage, rect);

      // calculate the sum of all pixel values
      MCvScalar sumScalar = CvInvoke.Sum(roi);

      // return mean value
      return sumScalar.V0 / (rect.Width * rect.Height);
    }

    private int GetDigitFromWhiteCell(Mat roi, int r, int c, int averageWhiteVal, bool isScreenShot)
    {
      SaveHistogramImage(roi, r, c);

      char digit = ' ';
      if (!isScreenShot)
      {
        var increasedContrast = new Mat();
        CvInvoke.ConvertScaleAbs(roi, increasedContrast, 1.5, 5);
        using var graySmooth = increasedContrast.ToImage<Gray, byte>().SmoothGaussian(45, 45, 2.5, 2.5);
        using Mat imgThresholded = new Mat();
        double binThreshold = averageWhiteVal * 1.2;
        CvInvoke.Threshold(graySmooth.Mat, imgThresholded, binThreshold, 255, ThresholdType.Binary);
        digit = ExtractDigitsFromImage(imgThresholded, r, c, false);
      }
      else
      {
        digit = ExtractDigitsFromImage(roi, r, c, false);
      }

      int val = -1;
      if (digit != ' ')
        val = digit - '0';

      //CvInvoke.PutText(imgThresholded, $"{val}", new Point(3, roi.Height - 3), FontFace.HersheyPlain, 4, new MCvScalar(0, 0, 0), 4);
      //SaveRegionToFile(imgThresholded, $"{r}{c}c.png");
      return val;
    }

    private int GetDigitFromBlackCell (Mat roi, int r, int c, int averageWhiteVal, bool isScreenShot)
    {
      Mat img4Ocr = roi;

      SaveHistogramImage(roi, r, c);

      Image<Gray, byte> graySmooth = new Image<Gray, byte>(roi.Size);
      if (!isScreenShot)
      {
        //var increasedContrast = new Mat();
        //CvInvoke.ConvertScaleAbs(roi, increasedContrast, 1.5, 5);
        //graySmooth = increasedContrast.ToImage<Gray, byte>().SmoothGaussian(45, 45, 2.5, 2.5);
        //img4Ocr = graySmooth.Mat;

        //Mat imgThresholded = new Mat();
        //double binThreshold = isScreenShot ? 150 : 180;

        //CvInvoke.Threshold(roi, img4Ocr, 0, 255, ThresholdType.Binary | ThresholdType.Otsu);
        CvInvoke.Threshold(roi, img4Ocr, averageWhiteVal * 0.70, 255, ThresholdType.Binary);

        //CvInvoke.Threshold(graySmooth, imgThresholded, binThreshold, 255, ThresholdType.Binary);
        //img4Ocr = roi;
        //img4Ocr = graySmooth.Mat;
      }

      var digit = ExtractDigitsFromImage(img4Ocr, r, c, true);
      int val = -1;
      if (digit != ' ')
        val = digit - '0';

      CvInvoke.PutText(img4Ocr, $"{val}", new Point(3, roi.Height - 3), FontFace.HersheyPlain, 4, new MCvScalar(222, 222, 222), 4);
      SaveRegionToFile(img4Ocr, $"{r}{c}c.png");

      return val;
    }

    private void SaveHistogramImage(Mat img, int r, int c)
    {
      DenseHistogram histogram = new DenseHistogram(256, new RangeF(0, 256));
      histogram.Calculate(new Image<Gray, byte>[] { img.ToImage<Gray, byte>() }, false, null);

      CvInvoke.Normalize(histogram, histogram, 0, 255, NormType.MinMax);
      using var histImage = new Mat(256, 256, DepthType.Cv8U, 1);
      histImage.SetTo(new MCvScalar(255));
      for (int i = 0; i < 256; i++)
      {
        int binVal = (int)histogram.GetBinValues()[i];
        CvInvoke.Line(histImage, new Point(i, 256), new Point(i, 256 - binVal), new MCvScalar(0, 255, 0), 2);
      }

      // Store histogram as PNG-File
      SaveRegionToFile(histImage, $"{r}{c}h.png");
    }

    public void SaveRegionToFile(Mat roi, string filename)
    {
#if DEBUG
      CvInvoke.Imwrite(Path.Combine(_dataFolder, filename), roi);
#endif
    }

    private char ExtractDigitsFromImage(Mat img, int r, int c, bool black)
    {
      bool Validate (Tesseract.Word word) 
      {
        return word.Confident > 33 && word.Text.Trim().Length == 1 && word.Region.Height > img.Height * 0.4;
      }

      if (_ocr == null)
        return ' ';

      _ocr.SetImage(img);
      _ocr.Recognize();

      var words = _ocr.GetWords();
      if (words != null)
      {
        var hit = words.FirstOrDefault(Validate);
        if (!hit.Equals(null) && hit.Text != null)
        {
          var color = black ? new MCvScalar(222, 222, 222) : new MCvScalar(0, 0, 0);
          CvInvoke.PutText(img, $"{(int)hit.Confident}", new Point(3, img.Height - 3), FontFace.HersheyPlain, 4, color, 4);
          CvInvoke.Rectangle(img, hit.Region, color, 1);
          SaveRegionToFile(img, $"{r}{c}v.png");

          return hit.Text[0];
        }
      }
      return ' ';
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

    public Mat PositionSolved(int x, int y, char newValue, bool green = false)
    {
      var fontScale = _ySizeBoard > 100 ? 7 : 2;
      var thickness = _ySizeBoard > 100 ? 7 : 3;
      var point = new Point(_points[x,y].X + 10, _points[x,y].Y - 10);
      if (green)
        CvInvoke.PutText(_originalImage, $"{newValue}", point, FontFace.HersheyPlain, fontScale, new MCvScalar(0, 255, 0), thickness);
      else
        CvInvoke.PutText(_originalImage, $"{newValue}", point, FontFace.HersheyPlain, fontScale, _black, thickness);
 
      return _originalImage;
    }

    public Mat OnFinished(bool success)
    {
      if (success)
      {
        if (_contour.Count > 2)
        {
          for (int i = 0; i < _contour.Count; i++)
          {
            Point startPoint = _contour[i];
            Point endPoint = _contour[(i + 1) % _contour.Count]; // connect the last point with the first
            CvInvoke.Line(_originalImage, startPoint, endPoint, new MCvScalar(0, 255, 0), 10);
          }
        }
        else
        {

        }
      }
      else
      {
        //DrawSadSmiley();
      }
      return _originalImage;
    }

    public void DrawSadSmiley()
    {
      var corners = FindCornerPoints(_contour);
    
      Point upperLeft = corners[0];
      Point upperRight = corners[1];
      Point lowerRight = corners[2];
      Point lowerLeft = corners[3];

      int centerX = upperLeft.X + (upperRight.X - upperLeft.X) / 2;
      int centerY = upperLeft.Y + (lowerLeft.Y - upperLeft.Y) / 2;

      // Berechne den Radius des Smileys
      int radius = (upperRight.X - upperLeft.X) / 4;

      // Zeichne das Gesicht
      CvInvoke.Circle(_originalImage, new Point(centerX, centerY), radius, new MCvScalar(0, 0, 255), 15);

      // Zeichne die Augen
      int eyeRadius = radius / 10;
      CvInvoke.Circle(_originalImage, new Point(centerX - radius / 2, centerY - radius / 2), eyeRadius, new MCvScalar(0, 0, 255), -1);
      CvInvoke.Circle(_originalImage, new Point(centerX + radius / 2, centerY - radius / 2), eyeRadius, new MCvScalar(0, 0, 255), -1);

      // Zeichne den traurigen Mund
      int mouthWidth = radius;
      int mouthHeight = radius / 2;
      CvInvoke.Ellipse(_originalImage, new Point(centerX, centerY + mouthHeight / 2), new Size(mouthWidth / 2, mouthHeight / 2), 0, 0, -180, new MCvScalar(0, 0, 255), 15);
    }
  }
}

