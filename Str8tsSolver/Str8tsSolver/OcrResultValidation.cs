using Str8tsSolverImageTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Maui.OCR;

namespace Str8tsSolver
{
  internal static class OcrResultValidation
  {
    private static List<string> _valid = ["1", "2", "3", "4", "5", "6", "7", "8", "9"];

    public static List<OcrResult.OcrElement> PickValidElements(OcrResult ocrResult, List<System.Drawing.Point> corners)
    {
      var candidates = ocrResult.Elements.Where(e => _valid.Contains(e.Text.Trim())).ToList();
      candidates.Where(e => corners.All(c => c.X > e.X && c.Y == e.Y));
      return ocrResult.Elements.Where(e => _valid.Contains(e.Text.Trim())).ToList();
    }

    public static List<OcrElement> GetValidElements(OcrResult ocrResult, int imgWidth, int imgHeight)
    {
      var elements = new List<OcrElement>();
      foreach (var e in ocrResult.Elements.Where(e => _valid.Contains(e.Text.Trim())))
      {
        elements.Add(new OcrElement
        {
          Confidence = e.Confidence,
          Height = e.Height,
          Text = e.Text,
          Width = e.Width,
          X = imgWidth - e.Y,
          Y = e.X
        });
      }

      return elements;
    }
  }

  public class GridValue
  {
    public char Value { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
  }

  internal static class ViewHelper
  {
    public static (int, int) GridPos2ViewCoo(int x, int y, Point? ul, Point? ur, Point? ll, Point? lr)
    {
      var dx = (ur?.X - ul?.X) / 9;
      var dy = (ll?.Y - ul?.Y) / 9;
      var x1 = ul?.X + y * dx;
      var y1 = 30+ (ul?.Y + x * dy);
      return ((int)x1, (int)y1);
    }
  }
}
