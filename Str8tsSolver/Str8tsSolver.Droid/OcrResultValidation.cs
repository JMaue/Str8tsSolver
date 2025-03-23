using Str8tsSolverImageTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Platform;
using Plugin.Maui.OCR;

namespace Str8tsSolver.Droid
{
  public static class OcrResultValidation
  {
    private static List<string> _valid = ["1", "2", "3", "4", "5", "6", "7", "8", "9"];

    public static List<OcrResult.OcrElement> PickValidElements(OcrResult ocrResult)
    {
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
