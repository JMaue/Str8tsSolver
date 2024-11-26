﻿namespace Str8tsSolver
{
  internal class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("Str8ts Solver");

      char[,] b = new char[,]
      { // devilish, not solved
        { '#', ' ', ' ', 'I', ' ', ' ', '#', ' ', ' ' },
        { '#', ' ', ' ', ' ', '2', ' ', '7', ' ', ' ' },
        { ' ', ' ', ' ', ' ', '#', ' ', ' ', ' ', ' ' },
        { ' ', ' ', ' ', ' ', ' ', ' ', '3', ' ', '#' },
        { 'G', '#', '6', '3', ' ', ' ', ' ', '#', '#' },
        { '#', ' ', '8', ' ', ' ', ' ', ' ', ' ', ' ' },
        { ' ', ' ', ' ', ' ', '#', ' ', ' ', ' ', ' ' },
        { ' ', ' ', '4', ' ', ' ', ' ', ' ', '7', 'A' },
        { ' ', '2', '#', ' ', ' ', '#', ' ', ' ', 'F' },
      };

      // { // devilish
      //  { ' ', ' ', '#', 'I', ' ', ' ', '#', ' ', '3' },
      //  { ' ', ' ', ' ', ' ', ' ', '#', '1', ' ', ' ' },
      //  { '#', ' ', ' ', ' ', '#', '3', ' ', ' ', 'D' },
      //  { '#', '5', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
      //  { ' ', ' ', 'A', '#', ' ', ' ', '#', ' ', ' ' },
      //  { ' ', ' ', ' ', ' ', '5', ' ', ' ', ' ', '#' },
      //  { '#', '#', '3', ' ', '#', ' ', ' ', ' ', 'G' },
      //  { ' ', ' ', '#', ' ', ' ', '#', ' ', ' ', ' ' },
      //  { ' ', ' ', ' ', '1', ' ', 'H', '#', ' ', ' ' },
      //};
      //{ // medium
      //  { 'E', ' ', ' ', ' ', ' ', '#', '#', ' ', '7' },
      //  { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
      //  { ' ', ' ', '#', ' ', ' ', '2', ' ', 'G', '#' },
      //  { 'D', '#', ' ', ' ', 'H', '#', ' ', ' ', ' ' },
      //  { ' ', ' ', ' ', ' ', ' ', ' ', ' ', '4', ' ' },
      //  { ' ', ' ', ' ', '#', '#', ' ', ' ', '#', '#' },
      //  { '#', 'I', ' ', ' ', ' ', ' ', '#', ' ', '4' },
      //  { ' ', ' ', ' ', ' ', ' ', '5', ' ', ' ', ' ' },
      //  { '9', ' ', '#', '#', ' ', ' ', ' ', ' ', 'A' },
      //};
      //{ // medium
      //  { '#', ' ', '4', 'C', ' ', ' ', '#', ' ', ' ' },
      //  { ' ', '3', ' ', '#', '6', ' ', '#', '9', ' ' },
      //  { ' ', ' ', 'F', ' ', ' ', '#', 'G', ' ', '#' },
      //  { 'A', '6', ' ', ' ', ' ', ' ', ' ', '5', ' ' },
      //  { ' ', ' ', ' ', ' ', 'E', ' ', ' ', ' ', ' ' },
      //  { '7', ' ', ' ', ' ', ' ', ' ', ' ', '6', '#' },
      //  { 'I', ' ', '#', '#', '1', ' ', '#', ' ', ' ' },
      //  { ' ', ' ', '#', ' ', ' ', 'H', ' ', ' ', ' ' },
      //  { '5', ' ', '#', ' ', ' ', '#', ' ', ' ', '#' },
      //};
      //{ // devilish 
      //  { '#', '#', '9', ' ', '#', ' ', ' ', ' ', 'B' },
      //  { ' ', ' ', ' ', ' ', '#', ' ', ' ', ' ', '#' },
      //  { ' ', ' ', '#', ' ', ' ', 'C', '#', ' ', ' ' },
      //  { '#', ' ', ' ', '#', ' ', ' ', ' ', '4', ' ' },
      //  { '#', ' ', '7', ' ', ' ', ' ', ' ', ' ', '#' },
      //  { ' ', ' ', ' ', ' ', ' ', '#', ' ', ' ', '#' },
      //  { '4', ' ', '#', '#', ' ', ' ', 'I', ' ', ' ' },
      //  { '#', '2', ' ', '4', 'A', '8', ' ', ' ', ' ' },
      //  { 'F', ' ', ' ', ' ', '#', ' ', ' ', '#', '#' },
      //};
      //{ // medium
      //  { '#', ' ', ' ', '#', ' ', ' ', ' ', ' ', 'G' },
      //  { ' ', ' ', ' ', ' ', '#', ' ', '3', ' ', '#' },
      //  { '9', ' ', '#', ' ', ' ', 'E', '4', '2', ' ' },
      //  { '#', ' ', '7', 'C', ' ', ' ', 'I', ' ', ' ' },
      //  { '1', ' ', ' ', '8', ' ', ' ', ' ', ' ', ' ' },
      //  { ' ', ' ', '#', ' ', ' ', '#', '6', ' ', '#' },
      //  { ' ', '4', ' ', '#', ' ', '8', '#', ' ', ' ' },
      //  { '#', '5', ' ', ' ', 'A', '7', ' ', ' ', ' ' },
      //  { 'F', ' ', ' ', '5', ' ', '#', ' ', ' ', '#' },
      //};

      //{ // devilish
      //  { '1', ' ', '#', '5', ' ', ' ', ' ', '#', '#' },
      //  { ' ', '3', ' ', ' ', ' ', '9', ' ', ' ', '#' },
      //  { '#', ' ', ' ', '#', '7', ' ', ' ', ' ', ' ' },
      //  { '#', '#', ' ', ' ', ' ', 'A', '#', ' ', ' ' },
      //  { ' ', ' ', ' ', ' ', ' ', '2', ' ', ' ', '7' },
      //  { ' ', '6', '#', 'D', ' ', ' ', ' ', 'E', '#' },
      //  { ' ', ' ', '9', ' ', ' ', '#', ' ', ' ', 'C' },
      //  { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
      //  { '#', '#', '8', ' ', ' ', ' ', 'I', ' ', ' ' },
      //};

      //{ // easy
      //  { '#', '#', '6', ' ', '#', ' ', ' ', '#', 'H' },
      //  { ' ', ' ', '5', ' ', '#', '3', ' ', '1', '2' },
      //  { ' ', ' ', '#', '4', '5', '#', '#', ' ', ' ' },
      //  { ' ', '8', 'A', ' ', '4', ' ', ' ', ' ', '#' },
      //  { '#', '#', '9', ' ', '#', ' ', ' ', 'G', '#' },
      //  { '#', ' ', ' ', ' ', ' ', '7', 'B', ' ', '3' },
      //  { '2', ' ', '#', '#', ' ', ' ', '#', ' ', ' ' },
      //  { '3', ' ', '2', ' ', 'I', ' ', ' ', ' ', ' ' },
      //  { 'F', '#', ' ', ' ', '#', ' ', ' ', '#', '#' },
      //};

      //{ // devilish (not solved)
      //  { ' ', ' ', '#', '#', ' ', '1', ' ', '#', 'H' },
      //  { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
      //  { '#', 'F', ' ', ' ', '#', '#', ' ', ' ', ' ' },
      //  { ' ', '#', '#', ' ', '8', ' ', 'B', '#', '5' },
      //  { ' ', ' ', ' ', ' ', ' ', ' ', '#', ' ', ' ' },
      //  { 'A', ' ', ' ', '#', '#', ' ', ' ', ' ', '#' },
      //  { ' ', ' ', '#', ' ', ' ', ' ', ' ', 'I', '#' },
      //  { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
      //  { '#', ' ', ' ', 'C', '#', ' ', ' ', ' ', ' ' },
      //};

      //{ // hard
      //  { 'E', 'I', ' ', ' ', '#', '#', ' ', ' ', ' ' },
      //  { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '6' },
      //  { ' ', ' ', '#', '#', ' ', ' ', ' ', ' ', '#' },
      //  { '#', ' ', ' ', 'D', ' ', ' ', '#', 'H', ' ' },
      //  { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
      //  { ' ', '#', 'F', ' ', ' ', 'B', ' ', ' ', 'C' },
      //  { '#', ' ', ' ', '5', ' ', '#', '#', '1', ' ' },
      //  { ' ', ' ', '7', ' ', ' ', ' ', ' ', ' ', ' ' },
      //  { ' ', ' ', ' ', 'A', '#', '3', '4', '#', '#' },
      //};

      //{ // medium
      //  { '5', '6', '#', '#', ' ', ' ', ' ', '#', '#' },
      //  { '4', ' ', '#', ' ', ' ', ' ', ' ', ' ', '#' },
      //  { 'B', ' ', ' ', ' ', '#', ' ', '7', '9', ' ' },
      //  { '#', ' ', ' ', ' ', '1', ' ', 'F', '#', ' ' },
      //  { ' ', ' ', 'I', ' ', '2', ' ', '#', ' ', ' ' },
      //  { ' ', '#', 'H', '2', ' ', ' ', ' ', ' ', '#' },
      //  { ' ', ' ', ' ', '8', '#', ' ', ' ', ' ', 'D' },
      //  { '#', ' ', ' ', ' ', '5', '4', '#', ' ', '3' },
      //  { '#', '#', ' ', '6', ' ', 'A', '#', ' ', ' ' },
      //};

      //{ // easy
      //  { 'E', '#', '8', ' ', ' ', '#', '#', '3', ' ' },
      //  { ' ', '7', '9', '#', 'B', ' ', ' ', '4', ' ' },
      //  { ' ', ' ', '#', '3', ' ', ' ', '2', ' ', '6' },
      //  { '#', ' ', ' ', ' ', '8', ' ', 'D', ' ', ' ' },
      //  { '#', 'F', '4', ' ', '#', ' ', ' ', 'A', '#' },
      //  { ' ', '1', 'C', '4', ' ', ' ', ' ', ' ', '#' },
      //  { ' ', ' ', ' ', ' ', ' ', ' ', '#', ' ', ' ' },
      //  { '4', ' ', '1', ' ', '#', '#', '6', ' ', ' ' },
      //  { ' ', ' ', '#', '#', ' ', ' ', ' ', 'I', 'G' },
      //};

      //var test = new List<List<char>>();
      //test.Add(new List<char> { 'A', 'B' });
      //test.Add(new List<char> { '1' });
      //test.Add(new List<char> { 'C', 'D', 'E' });
      //test.Add(new List<char> { 'X', 'Y' });

      //foreach (var x in Permutations.Permute(test))
      //  Console.WriteLine(string.Join("", x));
      //Console.ReadLine();

      var board = new Board (b);
      board.ReadStr8ts();
      board.PrintBoard(true);

      //var str = new HStr8t(2, 3, board);
      //str.Append(4);
      //str.Append(5);
      //str.Append(6);
      //str.Members.Add(board._grid[2, 3]);
      //str.Members.Add(board._grid[2, 4]);
      //str.Members.Add(board._grid[2, 5]);
      //str.Members.Add(board._grid[2, 6]);
      //str.Members[1].Candidates = new List<int> { 1, 3, 4, 5, 6 };
      ////str.Members[2].Candidates = new List<int> { 1, 3, 4, 5, 6 };
      //str.Members[3].Candidates = new List<int> { 1, 3, 4, 5, 6 };
      //var certain = str.CertainCells();
      //Console.WriteLine(string.Join("", certain));

      var solved = Str8tsSolver.Solve(board, out int iterations);
      var msg = solved ? "Solved" : "Not solved";
      Console.WriteLine($"{msg} with {iterations} iterations");
      board.PrintBoard();
    }
  }
}
