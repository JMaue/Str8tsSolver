namespace Str8tsSolver
{
  using Str8tsSolverImageTools;
  using Str8tsSolverLib;
  using System;

  internal class Program
  { 
    static void Main(string[] args)
    {
      Console.WriteLine("Str8ts Solver");

      char[,] b = new char[,]
      { // hard
        { '#', '#', ' ', ' ', '#', ' ', ' ', 'D', '#' },
        { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '6' },
        { '8', ' ', '#', ' ', ' ', 'A', ' ', ' ', ' ' },
        { '#', '#', ' ', ' ', '3', ' ', 'G', ' ', ' ' },
        { '#', ' ', '3', '#', '#', ' ', ' ', ' ', '#' },
        { ' ', '4', '#', '#', ' ', ' ', '#', 'E', ' ' },
        { ' ', ' ', ' ', ' ', '7', ' ', 'I', ' ', ' ' },
        { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
        { 'F', '#', ' ', ' ', '#', '#', ' ', ' ', '#' },
      };

      string imagePath1 = @"D:\Jens\Repositories\Str8tsSolver\Data\20241129_222948.jpg";
      string imagePath2 = @"D:\Jens\Repositories\Str8tsSolver\Data\20241223_160209.jpg";
      string imagePath3 = @"D:\Jens\Repositories\Str8tsSolver\Data\20241225_18=16.jpg";
      string imagePath4 = @"D:\Jens\Repositories\Str8tsSolver\Data\ex1.png";
      string imagePath5 = @"D:\Jens\Repositories\Str8tsSolver\Data\20241229_101829.jpg";
      string imagePath6 = @"D:\Jens\Repositories\Str8tsSolver\Data\20241229_095738.jpg";
      string imagePath7 = @"D:\Jens\Repositories\Str8tsSolver\Data\ex3.png";
      string imagePath8 = @"D:\Jens\Repositories\Str8tsSolver\Data\ex4.png";
      string imagePath9 = @"D:\Jens\Repositories\Str8tsSolver\Data\ex5.png";
      string imagePath10 = @"D:\Jens\Repositories\Str8tsSolver\Data\ex6.jpg";
      string imagePath11 = @"D:\Jens\Repositories\Str8tsSolver\Data\20250107_071128.jpg";
      string imagePath12 = @"D:\Jens\Repositories\Str8tsSolver\Data\20250106_1916";
      string imagePath13 = @"D:\Jens\Repositories\Str8tsSolver\Data\2024-01-12.png";
      string imagePath14 = @"D:\Jens\Repositories\Str8tsSolver\Data\20250213_225501.jpg";

      //var bf = new BoardFinder(@"..\data");
      //var cf = new ContourFinder();
      //var contour = cf.FindExternalContour(imagePath14);
      //var grid = bf.(imagePath14);
      //bf.Dispose();

      var board = new Board (b);
      board.ReadBoard();
      board.PrintBoard(true);
      board.PositionSolved += (x, y, newValue) => Console.WriteLine($"Position {x},{y} solved with {newValue}");

      var solved = Str8tsSolver.Solve(board, out int iterations);
      var msg = solved ? "Solved" : "Not solved";
      Console.WriteLine($"{msg} with {iterations} iterations");
      board.PrintBoard();
    }
  }
}
