using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Primitives;

namespace Str8tsSolver
{
  public partial class MainPage : ContentPage
  {
    private ICameraProvider _cameraProvider;

    public MainPage(ICameraProvider cp)
    {
      InitializeComponent();
      _cameraProvider = cp;

      MyCamera.MediaCaptured += MyCamera_MediaCaptured;
    }

    protected async override void OnNavigatedTo(NavigatedToEventArgs args)
    {
      base.OnNavigatedTo(args);

      await _cameraProvider.RefreshAvailableCameras(CancellationToken.None);
      MyCamera.SelectedCamera = _cameraProvider.AvailableCameras
          .Where(c => c.Position == CameraPosition.Rear).FirstOrDefault();
    }

    // Implemented as a follow up video https://youtu.be/JUdfA7nFdWw
    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
      base.OnNavigatedFrom(args);

      MyCamera.MediaCaptured -= MyCamera_MediaCaptured;
      MyCamera.Handler?.DisconnectHandler();
    }

    private void MyCamera_MediaCaptured(object? sender, CommunityToolkit.Maui.Views.MediaCapturedEventArgs e)
    {
      //_currentFrame = ReadStreamToMat(e.Media);
      //if (_currentFrame != null)
      //{
      //  // Bildverarbeitung durchführen und Rechteckposition berechnen
      //  var (x, y, width, height) = PerformImageProcessing(_currentFrame);

      //  // Rechteckposition aktualisieren
      //  RectangleDrawable.UpdateRectangle(x, y, width, height);

      //  // GraphicsView aktualisieren
      //  //OverlayGraphicsView.Invalidate();
      //  Dispatcher.Dispatch(() => OverlayGraphicsView.Invalidate());
      //}
    }
  }

}
