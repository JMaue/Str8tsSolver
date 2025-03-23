namespace Str8tsSolver
{
  using Str8tsSolverImageTools;
  using Str8tsSolverLib;
  using System;
  using System.Drawing;

  public class ConsoleTxtOut : ITxtOut
  {
    void ITxtOut.SetColors(Color foreground, Color background)
    {
      Console.ForegroundColor = foreground == Color.White ? ConsoleColor.White : ConsoleColor.Black;
      Console.BackgroundColor = background == Color.White ? ConsoleColor.White : ConsoleColor.Black;
    }

    void ITxtOut.Write(string text) => Console.Write(text);

    void ITxtOut.Write(char c) => Console.Write(c);


    void ITxtOut.WriteLine() => Console.WriteLine();

    void ITxtOut.WriteLine(string text) => Console.WriteLine(text);
  }

  internal class Program
  {
    private static char[,] LoadBoardFromFile(string filePath)
    {
      var board = new char[9, 9];
      var lines = File.ReadAllLines(filePath);

      for (int i = 0; i < lines.Length; i++)
      {
        for (int j = 0; j < lines[i].Length; j++)
        {
          var v = lines[i][j];
          board[i, j] = v == '.' ? ' ' : v;
        }
      }

      return board;
    }

    static void Main(string[] args)
    {
      Console.WriteLine("Str8ts Solver");

      char[,] b = new char[,]
      { // hard
        { ' ', '3', '1', 'D', '#', ' ', ' ', ' ', ' ' },
        { ' ', ' ', ' ', ' ', ' ', ' ', '7', ' ', ' ' },
        { 'E', '#', 'C', ' ', ' ', ' ', ' ', '#', '#' },
        { '#', ' ', ' ', '7', ' ', '#', ' ', ' ', '2' },
        { ' ', ' ', 'I', ' ', ' ', ' ', 'A', ' ', ' ' },
        { '8', ' ', ' ', '#', '5', ' ', ' ', ' ', '#' },
        { 'F', '#', ' ', '1', ' ', ' ', '#', '#', 'G' },
        { ' ', ' ', ' ', ' ', ' ', ' ', '8', ' ', ' ' },
        { ' ', ' ', ' ', ' ', '#', '#', ' ', ' ', ' ' },
      };

      //string imagePath1 = @"D:\Jens\Repositories\Str8tsSolver\Data\20241129_222948.jpg";
      //string imagePath2 = @"D:\Jens\Repositories\Str8tsSolver\Data\20241223_160209.jpg";
      //string imagePath3 = @"D:\Jens\Repositories\Str8tsSolver\Data\20241225_18=16.jpg";
      //string imagePath4 = @"D:\Jens\Repositories\Str8tsSolver\Data\ex1.png";
      //string imagePath5 = @"D:\Jens\Repositories\Str8tsSolver\Data\20241229_101829.jpg";
      //string imagePath6 = @"D:\Jens\Repositories\Str8tsSolver\Data\20241229_095738.jpg";
      //string imagePath7 = @"D:\Jens\Repositories\Str8tsSolver\Data\ex3.png";
      //string imagePath8 = @"D:\Jens\Repositories\Str8tsSolver\Data\ex4.png";
      //string imagePath9 = @"D:\Jens\Repositories\Str8tsSolver\Data\ex5.png";
      //string imagePath10 = @"D:\Jens\Repositories\Str8tsSolver\Data\ex6.jpg";
      //string imagePath11 = @"D:\Jens\Repositories\Str8tsSolver\Data\20250107_071128.jpg";
      //string imagePath12 = @"D:\Jens\Repositories\Str8tsSolver\Data\20250106_1916";
      //string imagePath13 = @"D:\Jens\Repositories\Str8tsSolver\Data\2024-01-12.png";
      //string imagePath14 = @"D:\Jens\Repositories\Str8tsSolver\Data\20250213_225501.jpg";

      //var bf = new BoardFinder(@"..\data");
      //var cf = new ContourFinder();
      //var contour = cf.FindExternalContour(imagePath14);
      //var grid = bf.(imagePath14);
      //bf.Dispose();

      var txtOut = new ConsoleTxtOut();
      b = LoadBoardFromFile(@"D:\\Jens\\Repositories\\Str8tsSolver\\Str8tsSolverTest\\Samples_derwesten\board_20250302.txt");
      var board = new Board (b, txtOut);
      board.ReadBoard();
      board.PrintBoard(true);
      board.PositionSolved += (x, y, newValue) => Console.WriteLine($"Position {x},{y} solved with {newValue}");

      var (solved, iterations) = Str8tsSolver.Solve(board, txtOut);
      var msg = solved ? "Solved" : "Not solved";
      Console.WriteLine($"{msg} with {iterations} iterations");
      board.PrintBoard(true);
    }
  }
}
