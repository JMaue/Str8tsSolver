using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV;
using System.Drawing;
using Emgu.CV.OCR;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

namespace Str8tsSolverImageTools
{
  public enum ShowIntermediateResults : Int16
  {
    LabelCorner = 1,
    CornerCycle = 2,
    DrawAllContours = 4,
    DrawRois = 8,
    ShowGivenCells = 16,
    ShowOcrResults = 32,
  }

  public class BoardFinder
  {
    private readonly string _dataFolder;

    Tesseract _ocr = new();

    // bottom left coordinates of each of the 9x9 grid cells to write the solution in the OnSolved callback
    Point[,] _points = new Point[9, 9];
    int ySize = 0;
    MCvScalar black = new MCvScalar(0, 0, 0);

    Mat _originalImage;
    List<Point> _contour;

    public BoardFinder(string dataFolder)
    {
      _dataFolder = dataFolder;
      InitOcr(Tesseract.DefaultTesseractDirectory, "eng", OcrEngineMode.TesseractOnly);
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
        if (_ocr != null)
        {
          _ocr.Dispose();
          _ocr = null;
        }

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

    public (Mat, char[,] board) FindBoard (string file)
    {
      _originalImage = CvInvoke.Imread(file, ImreadModes.Color);

      var contour = FindExternalContour(ref _originalImage);
      if (contour.Any())
      {
        _contour = new List<Point>(contour);
        var isScreenShot = IsScreenShot(contour);
        var board = Find81Fields(_originalImage, contour, isScreenShot);

        return (_originalImage, board);
      }
      return (_originalImage, new char[0,0]);
    }

    public List<Point> FindExternalContour(string file)
    {
      Mat image = CvInvoke.Imread(file, ImreadModes.Color);
      return FindExternalContour(ref image);
    }

    public List<Point> FindExternalContour(ref Mat img)
    {
      var rc = new List<Point>();
      int count = 0;
      var imgX = img.Rows;
      var imgY = img.Cols;
      var minSquareSize = Math.Min(imgX, imgY) * 0.8;

      // Bild in Graustufen konvertieren
      using (Mat gray = new Mat())
      {
        CvInvoke.CvtColor(img, gray, ColorConversion.Bgr2Gray);
        var graySmooth = gray.ToImage<Gray, byte>().SmoothGaussian(5, 5, 1.5, 1.5);
        //img = graySmooth.Mat;
        // Kanten mit Canny-Algorithmus erkenney(230)n
        using (Mat cannyEdges = new Mat())
        {
          CvInvoke.Canny(graySmooth, cannyEdges, 50, 150);
          using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
          {
            // Konturen finden
            CvInvoke.FindContours(cannyEdges, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);
            for (int i = 0; i < contours.Size; i++)
            {
              using (VectorOfPoint contour = contours[i])
              using (VectorOfPoint approxContour = new VectorOfPoint())
              {
                // Kontur approximieren
                CvInvoke.ApproxPolyDP(contour, approxContour, CvInvoke.ArcLength(contour, true) * 0.10, true);

                // Prüfen, ob die approximierte Kontur mindestens 80% der Bildbreite einnimmt.
                var maxX = 0;
                var maxY = 0;
                var minX = imgX;
                var minY = imgY;
                //CvInvoke.DrawContours(img, contours, i, new MCvScalar(0, 0, 255), 2);
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
                    var d = imgY > 1000 ? 15 : 5;
                    if ((ShowIntermediates & (Int16)ShowIntermediateResults.CornerCycle) != 0) 
                      CvInvoke.Circle(img, point, d, new MCvScalar(0, 255, 0), 5);
                    rc.Add(point);
                  }
                  count++;
                  if (count == 1)
                    return rc;
                }
              }
            }
          }
        }
      }

      return rc;
    }

    public Point[] FindCornerPoints(List<Point> contour)
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

      return new Point[] { upperLeft, upperRight, lowerLeft, lowerRight };
    }

    public char[,] Find81Fields(Mat originalImage, List<Point> contour, bool isScreenShot = false)
    {
      var corners = FindCornerPoints(contour);
      Point upperLeft = corners[0];
      Point upperRight = corners[1];
      Point lowerLeft = corners[2];
      Point lowerRight = corners[3];

      if ((ShowIntermediates & (Int16)ShowIntermediateResults.LabelCorner) != 0)
      {
        CvInvoke.PutText(originalImage, "1", lowerRight, FontFace.HersheyPlain, 8, new MCvScalar(255, 0, 255), 4);
        CvInvoke.PutText(originalImage, "2", lowerLeft, FontFace.HersheyPlain, 8, new MCvScalar(255, 0, 255), 4);
        CvInvoke.PutText(originalImage, "3", upperRight, FontFace.HersheyPlain, 8, new MCvScalar(255, 0, 255), 4);
        CvInvoke.PutText(originalImage, "4", upperLeft, FontFace.HersheyPlain, 8, new MCvScalar(255, 0, 255), 4);
      }

      var grayImage = new Mat();
      CvInvoke.CvtColor(originalImage, grayImage, ColorConversion.Bgr2Gray);

      (bool[,] blackValues, int average, int avgBlack, int avgWhite) = CalcBlackWhiteMatrix (grayImage, upperLeft, upperRight, lowerLeft, lowerRight);

      var dxr = (upperRight.X - lowerRight.X) / 9;
      var dyr = (lowerRight.Y - upperRight.Y) / 9;    // always positiv
      var dxl = (upperLeft.X - lowerLeft.X) / 9;
      var dyl = (lowerLeft.Y - upperLeft.Y) / 9;      // always positiv

      var board = new char[9, 9];
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

          ySize = bottomLeft.Y - topLeft.Y;

          if ((ShowIntermediates & (Int16)ShowIntermediateResults.DrawRois) != 0)
          {
            // draw rectangle
            CvInvoke.Line(originalImage, topLeft, topRight, new MCvScalar(0, 255, 0), 2);
            CvInvoke.Line(originalImage, topLeft, bottomLeft, new MCvScalar(0, 255, 0), 2);
            CvInvoke.Line(originalImage, bottomLeft, bottomRight, new MCvScalar(0, 255, 0), 2);
            CvInvoke.Line(originalImage, topRight, bottomRight, new MCvScalar(0, 255, 0), 2);
          }

          // define roi
          int shrink = Convert.ToInt16(dxt * 0.20); // take 15% off from all sides
          Rectangle rect = new Rectangle(topLeft.X + shrink, topLeft.Y + shrink, topRight.X - topLeft.X - (2 * shrink), bottomLeft.Y - topLeft.Y - (2 * shrink));
          Mat roi = new Mat(grayImage, rect);
          SaveRegionToFile(roi, Path.Combine(_dataFolder, $"{r}{c}.png"));
          
          var isBlack = blackValues[r, c];
          board[r, c] = isBlack ? '#' : ' ';
          var digit = isBlack 
            ? GetDigitFromBlackCell (roi, r, c, isScreenShot) 
            : GetDigitFromWhiteCell (roi, r, c, avgWhite, isScreenShot);

          if (digit >= 1 && digit <= 9)
          {
            if (isBlack)
              board[r, c] = (char)('A' + digit - 1);
            else
              board[r, c] = (char)('0' + digit);

            if ((ShowIntermediates & (Int16)ShowIntermediateResults.ShowGivenCells) != 0)
            {
              var fontScale = ySize > 100 ? 10 : 3;
              var thickness = ySize > 100 ? 10 : 4;
              CvInvoke.PutText(originalImage, $"{digit}", bottomLeft, FontFace.HersheyPlain, fontScale, new MCvScalar(0, 255, 0), thickness);
            }
          }

          // keep the bottom left coordinates of each of the 9x9 grid cells to write the solution in the OnSolved callback
          _points[r, c] = bottomLeft;
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

      var board = new bool[9, 9];
      double graySum = 0;
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

            ySize = bottomLeft.Y - topLeft.Y;

            // Rechteck definieren
            int shrink = Convert.ToInt16(dxt * 0.20); // take 15% off from all sides
            Rectangle rect = new Rectangle(topLeft.X + shrink, topLeft.Y + shrink, topRight.X - topLeft.X - (2 * shrink), bottomLeft.Y - topLeft.Y - (2 * shrink));

            // Durchschnittlichen Grauwert berechnen
            //var grayImage = new Mat();
            //CvInvoke.CvtColor(originalImage, grayImage, ColorConversion.Bgr2Gray);
            var average = GetAverageGrayValue(image, rect);
            processAverage(average, r, c);
          }
        }
      }
      int[] vals = new int[256];
      ProcessRectangles((average, r, c) => {
        graySum += average;
        vals[Convert.ToInt32(average)]++;
      });
      int mean = Convert.ToInt32(graySum / 81);
      int avgWhite = 0;
      int noWhite = 0;
      int avgBlack = 0;
      int noBlack = 0;
      for (int i=0; i<256; i++)
      {
        if (vals[i] == 0)
          continue; 
        if (i > mean)
        {
          avgWhite += i*vals[i];
          noWhite += vals[i];
        }
        else
        {
          avgBlack += i * vals[i];
          noBlack += vals[i];
        }
      }
      avgBlack /= noBlack;
      avgWhite /= noWhite;

      ProcessRectangles((average, r, c) => board[r, c] = average < mean);

      return (board, mean, avgBlack, avgWhite);
    }

    public static double GetAverageGrayValue(Mat grayImage, Rectangle rect)
    {
      // Bereich des Bildes, der vom Rechteck umschlossen ist, extrahieren
      Mat roi = new Mat(grayImage, rect);

      // Summe der Pixelwerte berechnen
      MCvScalar sumScalar = CvInvoke.Sum(roi);

      // Durchschnitt berechnen
      double averageGrayValue = sumScalar.V0 / (rect.Width * rect.Height);

      return averageGrayValue;
    }

    public int GetDigitFromWhiteCell(Mat roi, int r, int c, int averageWhiteVal, bool isScreenShot)
    {
      Mat img4Ocr = roi;
      SaveHistogramImage(roi, r, c);

      Image<Gray, byte> graySmooth = new Image<Gray, byte>(roi.Size);
      if (!isScreenShot)
      {
        var increasedContrast = new Mat();
        CvInvoke.ConvertScaleAbs(roi, increasedContrast, 1.5, 5);
        graySmooth = increasedContrast.ToImage<Gray, byte>().SmoothGaussian(45, 45, 2.5, 2.5);
        img4Ocr = graySmooth.Mat;

        Mat imgThresholded = new Mat();
        double binThreshold = isScreenShot ? 150 : 180;

        CvInvoke.Threshold(img4Ocr, imgThresholded, averageWhiteVal*1.2, 255, ThresholdType.Binary);
        
        //CvInvoke.Threshold(graySmooth, imgThresholded, binThreshold, 255, ThresholdType.Binary);
        img4Ocr = imgThresholded;
        //img4Ocr = graySmooth.Mat;
      }

      var digit = ExtractDigitsFromImage(img4Ocr, r, c, false);
      int val = -1;
      if (digit != ' ')
        val = digit - '0';

      CvInvoke.PutText(img4Ocr, $"{val}", new Point(3, roi.Height - 3), FontFace.HersheyPlain, 4, new MCvScalar(0, 0, 0), 4);
      SaveRegionToFile(img4Ocr, Path.Combine(_dataFolder, $"{r}{c}c.png")); 
      return val;
    }

    public int GetDigitFromBlackCell (Mat roi, int r, int c, bool isScreenShot)
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

        CvInvoke.Threshold(roi, img4Ocr, 0, 255, ThresholdType.Binary | ThresholdType.Otsu);

        //CvInvoke.Threshold(graySmooth, imgThresholded, binThreshold, 255, ThresholdType.Binary);
        //img4Ocr = roi;
        //img4Ocr = graySmooth.Mat;
      }

      var digit = ExtractDigitsFromImage(img4Ocr, r, c, true);
      int val = -1;
      if (digit != ' ')
        val = digit - '0';

      CvInvoke.PutText(img4Ocr, $"{val}", new Point(3, roi.Height - 3), FontFace.HersheyPlain, 4, new MCvScalar(222, 222, 222), 4);
      SaveRegionToFile(img4Ocr, Path.Combine(_dataFolder, $"{r}{c}c.png"));

      return val;
    }

    public void SaveHistogramImage(Mat img, int r, int c)
    {
      DenseHistogram histogram = new DenseHistogram(256, new RangeF(0, 256));
      histogram.Calculate(new Image<Gray, byte>[] { img.ToImage<Gray, byte>() }, false, null);

      // Optional: Um das Histogramm anzuzeigen
      Mat histImage = new Mat();
      CvInvoke.Normalize(histogram, histogram, 0, 255, NormType.MinMax);
      histImage = new Mat(256, 256, DepthType.Cv8U, 1);
      histImage.SetTo(new MCvScalar(255));
      for (int i = 0; i < 256; i++)
      {
        int binVal = (int)histogram.GetBinValues()[i];
        CvInvoke.Line(histImage, new Point(i, 256), new Point(i, 256 - binVal), new MCvScalar(0, 255, 0), 2);
      }

      // Store Histogramm as PNG-File
      CvInvoke.Imwrite(Path.Combine(_dataFolder, $"{r}{c}h.png"), histImage);
    }

    public static void SaveRegionToFile(Mat roi, string filePath)
    {
      // Bild in Datei speichern
      CvInvoke.Imwrite(filePath, roi);
    }

    private char ExtractDigitsFromImage(Mat img, int r, int c, bool black)
    {
      bool Validate (Tesseract.Word word) 
      {
        return word.Confident > 75 && word.Text.Trim().Length == 1 && word.Region.Height > img.Height * 0.4;
      }

      _ocr.SetImage(img);
      _ocr.Recognize();

      var words = _ocr.GetWords();
      if (words != null)
      {
        var hit = words.FirstOrDefault(w => Validate (w));
        if (!hit.Equals(null) && hit.Text != null)
        {
          var color = black ? new MCvScalar(222, 222, 222) : new MCvScalar(0, 0, 0);
          CvInvoke.PutText(img, $"{(int)hit.Confident}", new Point(3, img.Height - 3), FontFace.HersheyPlain, 4, color, 4);
          CvInvoke.Rectangle(img, hit.Region, color, 1);
          SaveRegionToFile(img, Path.Combine(_dataFolder, $"{r}{c}v.png"));

          if ((ShowIntermediates & (int)ShowIntermediateResults.ShowOcrResults) != 0)
          {
            color = new MCvScalar(222, 0, 0);
            CvInvoke.PutText(_originalImage, $"{(int)hit.Confident}", new Point(3, img.Height - 3), FontFace.HersheyPlain, 4, color, 4);
            CvInvoke.Rectangle(_originalImage, hit.Region, color, 1);
          }
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
      Point lowerLeft = corners[2];
      Point lowerRight = corners[3];

      return upperLeft.Y == upperRight.Y && lowerLeft.Y == lowerRight.Y && upperLeft.X == lowerLeft.X && upperRight.X == lowerRight.X;
    }

    public Mat PositionSolved(int x, int y, char newValue)
    {
      var fontScale = ySize > 100 ? 10 : 3;
      var thickness = ySize > 100 ? 10 : 4;
      CvInvoke.PutText(_originalImage, $"{newValue}", _points[x,y], FontFace.HersheyPlain, fontScale, black, thickness);
 
      return _originalImage;
    }

    public Mat MarkSolved()
    {
      if (_contour.Count > 2)
      {

        for (int i = 0; i < _contour.Count; i++)
        {
          Point startPoint = _contour[i];
          Point endPoint = _contour[(i + 1) % _contour.Count]; // Verbindet den letzten Punkt mit dem ersten Punkt
          CvInvoke.Line(_originalImage, startPoint, endPoint, new MCvScalar(0, 255, 0), 10);
        }
      }
      return _originalImage;
    }
  }
}

