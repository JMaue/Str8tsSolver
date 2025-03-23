
namespace Str8tsSolver.WinUI
{
  public static class MauiProgram
  {
    public static MauiApp CreateMauiApp()
    {
      var builder = MauiApp.CreateBuilder();

      builder
        .UseSharedMauiApp();

      builder.Services.AddSingleton<IOcrDigitRecognizer>(new OcrDigitRecognizer());
      return builder.Build();
    }
  }
}
