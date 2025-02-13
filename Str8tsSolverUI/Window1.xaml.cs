using Emgu.CV;
using Str8tsSolverImageTools;
using System.Windows;
using Str8tsSolverLib;
using System.Threading.Tasks;
using System.Configuration;
using Emgu.CV.CvEnum;

namespace Str8tsSolver.WPF
{
  public partial class MainWindow : Window
  {
    private string _dataFolder;

    public MainWindow()
    {
      InitializeComponent();
      _dataFolder = ConfigurationManager.AppSettings["DataFolder"];
      _boardFinder = new BoardFinder(_dataFolder);
    }

    private readonly BoardFinder _boardFinder;

    private void Window_DragEnter(object sender, DragEventArgs e)
    {
      if (e.Data.GetDataPresent(DataFormats.FileDrop))
      {
        e.Effects = DragDropEffects.Copy;
      }
      else
      {
        e.Effects = DragDropEffects.None;
      }
    }

    private void Window_Drop(object sender, DragEventArgs e)
    {
      if (e.Data.GetDataPresent(DataFormats.FileDrop))
      {
        string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
        if (files.Length > 0)
        {
          _boardFinder.ShowIntermediates = (int)ShowIntermediateResults.CornerCycle;
          _boardFinder.ShowIntermediates |= (int)(ShowIntermediateResults.ShowOcrResults | ShowIntermediateResults.DrawAllContours);
          
          var img = CvInvoke.Imread(files[0], ImreadModes.Color);
          var contour = _boardFinder.FindExternalContour(ref img);
          imageBox.Source = img.ToBitmapSource();
          Task.Run(() =>
          {
            if (contour.Count >= 4)
            {
              _boardFinder.NumberDetected += OnNumberDetected;
              var chars = _boardFinder.Find81Fields(img, contour);
              _boardFinder.NumberDetected -= OnNumberDetected;

              var board = new Board(chars);
              board.ReadBoard();
              board.PositionSolved += OnPositionSolved;

              Task.Run(() =>
              {
                bool isSolved = Str8tsSolverLib.Str8tsSolver.Solve(board, out var iterations);
                board.PositionSolved -= OnPositionSolved;
                Dispatcher.Invoke(() =>
                {
                  var mat = _boardFinder.OnFinished(isSolved);
                  imageBox.Source = mat.ToBitmapSource();
                });
              });
            }
          });
        }
      }
    }

    private void OnPositionSolved(int x, int y, char newValue)
    {
      // use the Dispatcher, to execute the code in the UI-Thread
      Dispatcher.Invoke(() =>
      {
        var mat = _boardFinder.PositionSolved(x, y, newValue);
        imageBox.Source = mat.ToBitmapSource();
      });
      Task.Delay(100).Wait();
    }
    private void OnNumberDetected(int x, int y, char newValue)
    {
      // use the Dispatcher, to execute the code in the UI-Thread
      Dispatcher.Invoke(() =>
      {
        var mat = _boardFinder.PositionSolved(x, y, newValue, true);
        imageBox.Source = mat.ToBitmapSource();
      });
      Task.Delay(100).Wait();
    }
  }
}

