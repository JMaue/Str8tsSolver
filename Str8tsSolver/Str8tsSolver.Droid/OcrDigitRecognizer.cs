using Plugin.Maui.OCR;
using Str8tsSolverImageTools;

namespace Str8tsSolver.Droid
{
  public class OcrDigitRecognizer : IOcrDigitRecognizer
  {
    private OcrOptions? _ocrOptions = null;
    private OcrResult? _ocrResult = null;

    public OcrDigitRecognizer()
    {

    }

    public async Task Initialize()
    {
      var builder = new OcrOptions.Builder();
      //builder.AddPatternConfig(new OcrPatternConfig("[0..9]"));
      _ocrOptions = builder.Build();

      await OcrPlugin.Default.InitAsync();
    }

    public void Reset()
    {
      _ocrResult = null;
    }

    public char GetDigit(int row, int column)
    {
      return '?';
    }

    public bool ProcessImage(byte[] image)
    {
      if (_ocrOptions  == null) 
        return false;

      OcrResult? ocrResult = null;
      Task.Run(async () =>
      {
        ocrResult = await OcrPlugin.Default.RecognizeTextAsync(image, _ocrOptions);
      }).Wait();

      if (ocrResult != null && ocrResult.Success && ocrResult.Elements.Count > 0)
      {
        // capture the OCR results
        _ocrResult = ocrResult;

        return true;
      }

      return false;
    }

    public List<OcrElement> GetElements(int imgWidth, int imgHeight, ImgSource imgSource)
    {
      if (_ocrResult == null)
        return new List<OcrElement>();

      return OcrResultValidation.GetValidElements(_ocrResult, imgWidth, imgHeight, imgSource);
    }
  }
}
