using Str8tsSolverImageTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Maui.OCR;
using Microsoft.Maui.Platform;

namespace Str8tsSolver
{
  internal static class OcrResultValidation
  {
    public enum ImgSource
    {
      Camera,
      Screenshot,
      Photo
    }

    private static List<string> _valid = ["1", "2", "3", "4", "5", "6", "7", "8", "9"];

    public static List<OcrResult.OcrElement> PickValidElements(OcrResult ocrResult) //, List<System.Drawing.Point> corners)
    {
      var candidates = ocrResult.Elements.Where(e => _valid.Contains(e.Text.Trim())).ToList();
      //candidates.Where(e => corners.All(c => c.X > e.X && c.Y == e.Y));
      return ocrResult.Elements.Where(e => _valid.Contains(e.Text.Trim())).ToList();
    }

    public static List<OcrElement> GetValidElements(OcrResult ocrResult, int imgWidth, int imgHeight, ImgSource imgSource)
    {
      var elements = new List<OcrElement>();
      foreach (var e in ocrResult.Elements.Where(e => _valid.Contains(e.Text.Trim())))
      {
        var ocrElement = imgSource == ImgSource.Camera ? OcrElementFromCamera(e, imgWidth) : OcrElementFromPhotograoh(e);
        elements.Add(ocrElement);
      }

      return elements;
    }
  
    public static OcrElement OcrElementFromCamera(OcrResult.OcrElement e, int imgWidth)
    {
      return new OcrElement
      {
        Confidence = e.Confidence,
        Text = e.Text,
        Height = e.Width,     
        Width = e.Height,
        X = imgWidth - e.Y,
        Y = e.X
      };
    }

    public static OcrElement OcrElementFromPhotograoh(OcrResult.OcrElement e)
    {
      return new OcrElement
      {
        Confidence = e.Confidence,
        Text = e.Text,
        Height = e.Height,
        Width = e.Width,
        X = e.X,
        Y = e.Y
      };
    }

    public static (int, int) OcrToImageFromScreenshot(int x, int y)
    {
      return (x, y);
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
