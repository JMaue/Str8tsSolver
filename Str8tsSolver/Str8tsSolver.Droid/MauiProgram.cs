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

      builder.Services.AddSingleton<IOcrService>(OcrPlugin.Default);
      return builder.Build();
    }
  }
}
