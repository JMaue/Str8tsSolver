using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Primitives;
using Str8tsSolverImageTools;
using Str8tsSolverLib;

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
    private ImgSource _imgSource;
    private ContourFinder _contourFinder;

    private byte[] _stream;
    private IOcrDigitRecognizer _ocrEngine;
    //private OcrResult _ocrResult;
    //private OcrOptions _ocrOptions;
    private List<System.Drawing.Point> _corners;

    private char[,] _grid;

    public MainPage(ICameraProvider cp, IOcrDigitRecognizer ocrEngine)
    {
      InitializeComponent();
      _cameraProvider = cp;
      _contourFinder = new ContourFinder();
      _ocrEngine = ocrEngine;

      MyCamera.MediaCaptured += MyCamera_ImageCaptured;

      //var builder = new OcrOptions.Builder();
      ////builder.AddPatternConfig(new OcrPatternConfig("[0..9]"));
      //_ocrOptions = builder.Build();
      _ocrEngine.Initialize();

      CreateCaptureThread();
    }

    #region Maui Infrastructur
    // Implemented as a follow up video https://youtu.be/JUdfA7nFdWw
    protected async override void OnAppearing()
    {
      base.OnAppearing();
      _viewWidth = myView.Width;
      _viewHeight = myView.Height;
      _ocrEngine.Initialize();  //OcrPlugin.Default.InitAsync();
    }

    protected async override void OnNavigatedTo(NavigatedToEventArgs args)
    {
      base.OnNavigatedTo(args);

      await _cameraProvider.RefreshAvailableCameras(CancellationToken.None);
      MyCamera.SelectedCamera = _cameraProvider.AvailableCameras
          .Where(c => c.Position == CameraPosition.Rear).FirstOrDefault();
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
      base.OnNavigatedFrom(args);

      MyCamera.MediaCaptured -= MyCamera_ImageCaptured;
      MyCamera.Handler?.DisconnectHandler();

      // Worker-Thread stoppen
      _cancellationTokenSource.Cancel();
      _captureEvent.Set();
    }
    #endregion

    private void CreateCaptureThread()
    {
      _cancellationTokenSource = new CancellationTokenSource();
      _captureThread = new Thread(() => CaptureImagesPeriodically(_cancellationTokenSource.Token))
      {
        Name = "CaptureThread"
      };
    }

    private void ResetVars()
    {
      _ocrEngine.Reset();
      _corners?.Clear();
      _grid = null;
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

        // do not continue here until the image is captured
        _captureEvent.Reset();
        _captureEvent.WaitOne();
        Thread.Sleep(10);
      }

      LogToFile($"Thread: CaptureImagesPeriodically ended");
    }

    private void MyCamera_ImageCaptured(object? sender, CommunityToolkit.Maui.Views.MediaCapturedEventArgs e)
    {
      string currentThreadName = Thread.CurrentThread.Name;
      var bytes = ReadFromStream(e.Media);
      LogToFile($"Thread: {currentThreadName} - Bytes: {bytes.Length} Counter {_counter}");
      if (bytes == null || bytes.Length == 0)
        return;

      _counter++;
      try
      {
        // find the outer contour of the board
        var corners = _contourFinder.FindExternalContour(bytes, out _imgWidth, out _imgHeight);
        LogToFile($"Thread: {currentThreadName} - Corners: {corners.Count}");
        if (corners.Count == 0)
        {
          // nothing found
          myGraphics.InvalidatePosition(_counter);
        }
        else
        {       
          // stop the capture thread
          _cancellationTokenSource.Cancel();
          myView.Invalidate();
          _captureEvent.Set();

          _ocrEngine.ProcessImage(bytes);
          //OcrResult? ocrResult = null;
          //Task.Run(async () =>
          //{
          //  ocrResult = await OcrPlugin.Default.RecognizeTextAsync(bytes, _ocrOptions);
          //}).Wait();

          //if (ocrResult != null && ocrResult.Success && ocrResult.Elements.Count > 0)
          //{
          //  // capture the OCR results
          //  _ocrResult = ocrResult;
          //}

          // keep the image byte stream for image analysis, i.e. for finding black and white cells
          _stream = bytes;

          // keep the corners for drawing the board contour
          _corners = corners;
          myGraphics.SetBoardContour(corners, _viewWidth, _viewHeight, _imgWidth, _imgHeight);

          // enable buttons on the UI
          Dispatcher.Dispatch(() => {
            EnableAnalyzeButton(true);
            EnableScanButton(true);

            // show the captured image on the UI and turn of the camera live view
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
      MyCamera.IsVisible = false;  // turn off live view

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
          return memoryStream.ToArray();
        }
        catch (Exception ex)
        {
          Console.WriteLine(ex.Message);
        }
      }
      return Array.Empty<byte>();
    }

    private void OnScanButtonClicked(object sender, EventArgs e)
    {
      // restart the image capture thread
      EnableScanButton(false);
      EnableAnalyzeButton(false);
      EnableSolveButton(false);

      if (_captureThread.ThreadState == ThreadState.Stopped)
      {
        CreateCaptureThread();
      }
      MyCamera.IsVisible = true;
      CapturedImage.IsVisible = false;
      _imgSource = ImgSource.Camera;

      ResetVars();
      myView.Invalidate();

      _captureThread.Start();
    }

    private void OnOpenButtonClicked(object sender, EventArgs e)
    {
      // read image from file
      _viewWidth = myView.Width;
      _viewHeight = myView.Height;

      ResetVars();
      myView.Invalidate();

      EnableScanButton(true);
      EnableAnalyzeButton(true);
      EnableSolveButton(false);
      Task.Run(async () =>
      {
        try
        {
          var pickResult = await MediaPicker.Default.PickPhotoAsync();
          if (pickResult != null)
          {
            _imgSource = ImgSource.Photo;

            using var imageAsStream = await pickResult.OpenReadAsync();
            var imageAsBytes = new byte[imageAsStream.Length];
            Task.Run(async () => await imageAsStream.ReadAsync(imageAsBytes)).Wait();
            var corners = _contourFinder.FindExternalContour(imageAsBytes, out int width, out int heigth);
            if (corners.Count > 0)
            {
              _ocrEngine.ProcessImage(imageAsBytes);
              //var ocrResult = await OcrPlugin.Default.RecognizeTextAsync(imageAsBytes, _ocrOptions);
              //if (ocrResult != null && ocrResult.Success && ocrResult.Elements.Count > 0)
              //{
              //  _ocrResult = ocrResult;
              //}
              _stream = imageAsBytes;
              _corners = corners;
              Dispatcher.Dispatch(() => { ShowCapturedImage(imageAsBytes); });
              myGraphics.SetBoardContour(corners, _viewWidth, _viewHeight, width, heigth);
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
    }

    private void OnAnalyzeButtonClicked(object sender, EventArgs args)
    {
      //if (_imgSource == ImgSource.Screenshot && !_contourFinder.IsScreenShot (_corners))
      //{
      //  _imgSource = ImgSource.Photo;
      //}
      //var elements = OcrResultValidation.GetValidElements(_ocrResult, _imgWidth, _imgHeight, _imgSource);
      var elements = _ocrEngine.GetElements(_imgWidth, _imgHeight, _imgSource);
      _grid = BoardFinder.FindBoard(_stream, _corners, elements);
      myGraphics.SetBoard(_grid);
      myView.Invalidate();

      EnableSolveButton(true);
    }

    private void OnSolveButtonClicked(object sender, EventArgs e)
    {
      // Implement Solve functionality
      if (_grid == null)
        return;

      var board = new Board (_grid);
      board.ReadBoard();
      board.PositionSolved += Board_PositionSolved;
      board.PuzzleSolved += Board_PuzzleSolved;
      board.SolvingProgress += Board_SolvingProgress;

      Task.Run(() => Str8tsSolverLib.Str8tsSolver.Solve(board));
    }

    #region callbacks from Solver Library
    private void Board_SolvingProgress(string currStr8t)
    {
      myGraphics.SolvingProgress(currStr8t);
      myView.Invalidate();
    }

    private void Board_PuzzleSolved(bool success)
    {
      myGraphics.PuzzleSolved(success); 
      myView.Invalidate();
    }

    private void Board_PositionSolved(int x, int y, char newValue)
    {
      myGraphics.PositionSolved(x, y, newValue);
      myView.Invalidate();
      Thread.Sleep(10);
    }
    #endregion region

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
