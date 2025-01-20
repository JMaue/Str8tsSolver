//----------------------------------------------------------------------------
//  Copyright (C) 2004-2024 by EMGU Corporation. All rights reserved.       
//----------------------------------------------------------------------------
using Emgu.CV;
using Str8tsSolverImageTools;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Emgu.CV;
using Str8tsSolverLib;
using System.Threading.Tasks;
using Emgu.CV.Reg;

namespace DragDropExample
{
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    private BoardFinder _boardFinder = new BoardFinder();

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
          _boardFinder.ShowIntermediates = 2;
          (var image, var chars) = _boardFinder.FindBoard(files[0]);
          imageBox.Source = image.ToBitmapSource();

          var board = new Board(chars);
          board.ReadBoard();
          board.PositionSolved += OnPositionSolved;

          Task.Run(() =>
          {
            bool isSolved = Str8tsSolver.Solve(board, out var iterations);
            Dispatcher.Invoke(() =>
            {
              if (isSolved)
              {
                var mat = _boardFinder.MarkSolved();
                imageBox.Source = mat.ToBitmapSource();
              }
              else
                MessageBox.Show("Not solved");
            });
          });
        }
      }
    }

    private void OnPositionSolved(int x, int y, char newValue)
    {
      // Verwende den Dispatcher, um den Code im UI-Thread auszuführen
      Dispatcher.Invoke(() =>
      {
        //var img = (BitmapSource)imageBox.Source;
        //var mat = img.ToMat();
        var mat = _boardFinder.PositionSolved(x, y, newValue);
        imageBox.Source = mat.ToBitmapSource();
      });
      Task.Delay(100).Wait();
    }
  }
}

