using Emgu.CV.CvEnum;
using Emgu.CV;
using Emgu.CV.OCR;
using Str8tsSolverImageTools;
using Str8tsSolver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Str8tsSolver.WinUI
{
  public class OcrDigitRecognizer : IOcrDigitRecognizer
  {
    Tesseract? _ocr = new();

    public async Task Initialize()
    {
      await Task.Run (() => InitOcr(Tesseract.DefaultTesseractDirectory, "eng", OcrEngineMode.TesseractOnly));
    }

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

    public char GetDigit(int row, int column)
    {
      return '?';
    }

    public List<OcrElement> GetElements(int imgWidth, int imgHeight, ImgSource imgSource)
    {
      //if (_ocrResult == null)
        return new List<OcrElement>();

      //return OcrResultValidation.GetValidElements(_ocrResult, imgWidth, imgHeight, imgSource);
    }

    public bool ProcessImage(byte[] rawBytes)
    {
      if (_ocr == null)
        return false;

      Mat img = new Mat();
      CvInvoke.Imdecode(rawBytes, ImreadModes.Color, img);

      _ocr.SetImage(img);
      _ocr.Recognize();
      var words = _ocr.GetWords();
      return words != null;
    }

    public void Reset()
    {
    }
  }
}
