using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Primitives;
using Microsoft.Maui.Graphics;
using System.IO;
using System.Threading;
using System.Timers;

namespace Str8tsSolver
{
  public partial class MainPage : ContentPage
  {
    private ICameraProvider _cameraProvider;
    private int _counter;

    private AutoResetEvent _captureEvent = new AutoResetEvent(true);
    private CancellationTokenSource _cancellationTokenSource;
    private Thread _captureThread;

    public MainPage(ICameraProvider cp)
    {
      InitializeComponent();
      _cameraProvider = cp;
      _counter = 0;

      MyCamera.MediaCaptured += MyCamera_MediaCaptured;

      // Worker-Thread starten
      _cancellationTokenSource = new CancellationTokenSource();
      _captureThread = new Thread(() => CaptureImagesPeriodically(_cancellationTokenSource.Token))
      {
        Name = "CaptureThread"
      };
      _captureThread.Start();
    }

    private async Task CaptureImagesPeriodically(CancellationToken token)
    {
      Thread.Sleep(2000);
      while (!token.IsCancellationRequested)
      {
        string currentThreadName = Thread.CurrentThread.Name;
        MyCamera.CaptureImage(CancellationToken.None);
        _captureEvent.Reset();
        _captureEvent.WaitOne();
        Thread.Sleep(10);
      }
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

      // Worker-Thread stoppen
      //_cancellationTokenSource.Cancel();
    }

    private void MyCamera_MediaCaptured(object? sender, CommunityToolkit.Maui.Views.MediaCapturedEventArgs e)
    {
      string currentThreadName = Thread.CurrentThread.Name;
      var bytes = ReadFromStream(e.Media);
      if (bytes == null || bytes.Length == 0)
        return;

      _counter++;
      myGraphics.UpdatePosition(_counter, _counter);
      myView.Invalidate();      
      _captureEvent.Set();

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

    private byte[] ReadFromStream(Stream stream)
    {
      using (var memoryStream = new MemoryStream())
      {
        try
        {
          stream.CopyTo(memoryStream);
          byte[] data = memoryStream.ToArray();
          //Mat mat = new Mat();
          //CvInvoke.Imdecode(data, ImreadModes.Color, mat);
          //CvInvoke.Circle(mat, new Point(100, 100), 50, new MCvScalar(0, 0, 255), 2);
          //SaveRegionToFile(mat, fn);
          return data;
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
        }
      }
      return null;
    }
  }

}
