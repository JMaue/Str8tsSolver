using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Primitives;
using Str8tsSolverImageTools;
using Plugin.Maui.OCR;

namespace Str8tsSolver
{
  public partial class MainPage : ContentPage
  {
    private ICameraProvider _cameraProvider;

    private AutoResetEvent _captureEvent = new AutoResetEvent(true);
    private CancellationTokenSource _cancellationTokenSource;
    private Thread _captureThread;
    private int _counter = 0;

    private double _viewWidth;
    private double _viewHeight;

    private int _imgWidth;
    private int _imgHeight;

    private ContourFinder _contourFinder;

    private byte[] _stream;
    private OcrResult _ocrResult;
    private OcrOptions _ocrOptions;
    private List<System.Drawing.Point> _corners;

    public MainPage(ICameraProvider cp)
    {
      InitializeComponent();
      _cameraProvider = cp;
      _contourFinder = new ContourFinder();

      MyCamera.MediaCaptured += MyCamera_MediaCaptured;

      var builder = new OcrOptions.Builder();
      //builder.AddPatternConfig(new OcrPatternConfig("[0..9]"));
      _ocrOptions = builder.Build(); 

      CreateCaptureThread();
    }

    protected async override void OnAppearing()
    {
      base.OnAppearing();
      _viewWidth = myView.Width;
      _viewHeight = myView.Height;
      await OcrPlugin.Default.InitAsync();
    }

    private void CreateCaptureThread()
    {
      _cancellationTokenSource = new CancellationTokenSource();
      _captureThread = new Thread(() => CaptureImagesPeriodically(_cancellationTokenSource.Token))
      {
        Name = "CaptureThread"
      };
    }

    private async Task CaptureImagesPeriodically(CancellationToken token)
    {
      Thread.Sleep(10);
      LogToFile($"Thread: CaptureImagesPeriodically started");
      _viewWidth = myView.Width;
      _viewHeight = myView.Height;
      while (!token.IsCancellationRequested)
      {
        LogToFile($"Thread: CaptureImagesPeriodically ");
        MyCamera.CaptureImage(CancellationToken.None);
        _captureEvent.Reset();
        _captureEvent.WaitOne();
        Thread.Sleep(10);
      }

      LogToFile($"Thread: CaptureImagesPeriodically ended");
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
      LogToFile($"Thread: {currentThreadName} - Bytes: {bytes.Length} Counter {_counter}");
      if (bytes == null || bytes.Length == 0)
        return;

      _counter++;
      try
      {
        var corners = _contourFinder.FindExternalContour(bytes, out _imgWidth, out _imgHeight);
        LogToFile($"Thread: {currentThreadName} - Corners: {corners.Count}");
        if (corners.Count == 0)
        {
          myGraphics.InvalidatePosition(_counter);
        }
        else
        {       
          _cancellationTokenSource.Cancel();
          myView.Invalidate();
          _captureEvent.Set();

          OcrResult ocrResult = null;
          Task.Run(async () =>
          {
            ocrResult = await OcrPlugin.Default.RecognizeTextAsync(bytes, _ocrOptions);
          }).Wait();

          if (ocrResult != null && ocrResult.Success && ocrResult.Elements.Count > 0)
          {
            _ocrResult = ocrResult;
          }
          _stream = bytes;
          _corners = corners;
          myGraphics.UpdatePosition(corners, _viewWidth, _viewHeight, _imgWidth, _imgHeight, _counter, _ocrResult);

          Dispatcher.Dispatch(() => {
            EnableAnalyzeButton(true);
            EnableScanButton(true);
            ShowCapturedImage(bytes);
          });
        }
      }
      catch (Exception ex)
      {
        myGraphics.InvalidatePosition(_counter, ex.Message);
      }
      myView.Invalidate();
      _captureEvent.Set();
    }

    private void ShowCapturedImage(byte[] imageBytes)
    {
      MyCamera.IsVisible = false;
      CapturedImage.IsVisible = true;

      CapturedImage.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
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

    private void OnScanButtonClicked(object sender, EventArgs e)
    {
      EnableScanButton(false);
      EnableAnalyzeButton(false);
      EnableSolveButton(false);

      if (_captureThread.ThreadState == ThreadState.Stopped)
      {
        CreateCaptureThread();
      }
      MyCamera.IsVisible = true;
      CapturedImage.IsVisible = false;

      _captureThread.Start();
    }
    private void OnOpenButtonClicked(object sender, EventArgs e)
    {
      _viewWidth = myView.Width;
      _viewHeight = myView.Height;
      EnableScanButton(false);
      EnableAnalyzeButton(false);
      EnableSolveButton(false);
      byte[] imageAsBytes = null;
      Task.Run(async () =>
      {
        try
        {
          var pickResult = await MediaPicker.Default.PickPhotoAsync();
          if (pickResult != null)
          {
            using var imageAsStream = await pickResult.OpenReadAsync();
            var imageAsBytes = new byte[imageAsStream.Length];
            Task.Run(async () => await imageAsStream.ReadAsync(imageAsBytes)).Wait();
            var corners = _contourFinder.FindExternalContour(imageAsBytes, out int width, out int heigth);
            if (corners.Count > 0)
            {
              var ocrResult = await OcrPlugin.Default.RecognizeTextAsync(imageAsBytes, _ocrOptions);
              if (ocrResult != null && ocrResult.Success && ocrResult.Elements.Count > 0)
              {
                _ocrResult = ocrResult;
              }
              _stream = imageAsBytes;
              _corners = corners;
              Dispatcher.Dispatch(() => { ShowCapturedImage(imageAsBytes); });
              myGraphics.UpdatePosition(corners, _viewWidth, _viewHeight, width, heigth, _counter, _ocrResult);
              myView.Invalidate();
            }
          }
        }
        catch (Exception ex)
        {
          await DisplayAlert("Error", ex.Message, "OK");
        }
      });

      MyCamera.IsVisible = false;
      CapturedImage.IsVisible = true;

      //_captureThread.Start();
    }

    private void OnAnalyzeButtonClicked(object sender, EventArgs args)
    {
      var elements = OcrResultValidation.GetValidElements(_ocrResult, _imgWidth, _imgHeight);
      var board = BoardFinder.FindBoard(_stream, _corners, elements);
    }

    private void OnSolveButtonClicked(object sender, EventArgs e)
    {
      // Implement Solve functionality
    }

    public void EnableScanButton(bool isEnabled) => ScanButton.IsEnabled = isEnabled;
    public void EnableAnalyzeButton(bool isEnabled) => AnalyzeButton.IsEnabled = isEnabled;
    public void EnableSolveButton(bool isEnabled) => SolveButton.IsEnabled = isEnabled;
  
    public void LogToFile(string message)
    {
      return;

      string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
      string filename = Path.Combine(path, "log.txt");
      File.AppendAllText(filename, message + Environment.NewLine);
    }
  }
}
