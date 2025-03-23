using Plugin.Maui.OCR;

namespace Str8tsSolver.Droid
{
  public static class MauiProgram
  {
    public static MauiApp CreateMauiApp()
    {
      var builder = MauiApp.CreateBuilder();

      builder
        .UseSharedMauiApp()
        .UseOcr();

      //builder.Services.AddSingleton<IOcrService>(OcrPlugin.Default);
      builder.Services.AddSingleton<IOcrDigitRecognizer>(new OcrDigitRecognizer());
      return builder.Build();
    }
  }
}
