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
        { '#', ' ', ' ', ' ', '#', '#', ' ', ' ', 'D' },
        { '#', ' ', ' ', '7', ' ', '#', ' ', ' ', ' ' },
        { ' ', ' ', '#', '5', ' ', ' ', '#', ' ', ' ' },
        { '1', ' ', ' ', '#', 'F', ' ', ' ', ' ', '#' },
        { ' ', ' ', ' ', ' ', ' ', ' ', ' ', '9', ' ' },
        { '#', ' ', ' ', ' ', '#', 'E', ' ', ' ', '7' },
        { ' ', ' ', 'A', ' ', ' ', '4', '#', ' ', ' ' },
        { ' ', ' ', ' ', 'I', ' ', ' ', ' ', ' ', '#' },
        { 'G', ' ', ' ', '#', '#', ' ', '1', ' ', '#' },
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

      var bf = new BoardFinder();
      var grid = bf.ReadBoardFromImage(imagePath9);
      grid[0, 5] = 'H';
      var board = new Board (grid);
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
