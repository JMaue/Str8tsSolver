using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using Microsoft.Extensions.Logging;

namespace Str8tsSolver
{
  public static class MauiProgramExtensions
  {
    public static MauiAppBuilder UseSharedMauiApp(this MauiAppBuilder builder)
    {
      builder.UseMauiApp<App>()
        // Initialize the .NET MAUI Community Toolkit CameraView by adding the below line of code
        .UseMauiCommunityToolkit()
        .UseMauiCommunityToolkitCore()
        .UseMauiCommunityToolkitCamera()
        .ConfigureFonts(fonts =>
        {
          fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
          fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        });

#if DEBUG
  		builder.Logging.AddDebug();
#endif
    //  builder.Services.AddSingleton<ICameraProvider, CommunityToolkit.Maui.Core.CameraProvider>();
      return builder;
    }
  }
}
