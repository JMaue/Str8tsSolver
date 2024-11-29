namespace Str8tsSolver
{
  using Str8tsSolverLib;
  internal class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("Str8ts Solver");

      char[,] b = new char[,]
      { // easy
        { 'E', '#', '8', ' ', ' ', '#', '#', '3', ' ' },
        { ' ', '7', '9', '#', 'B', ' ', ' ', '4', ' ' },
        { ' ', ' ', '#', '3', ' ', ' ', '2', ' ', '6' },
        { '#', ' ', ' ', ' ', '8', ' ', 'D', ' ', ' ' },
        { '#', 'F', '4', ' ', '#', ' ', ' ', 'A', '#' },
        { ' ', '1', 'C', '4', ' ', ' ', ' ', ' ', '#' },
        { ' ', ' ', ' ', ' ', ' ', ' ', '#', ' ', ' ' },
        { '4', ' ', '1', ' ', '#', '#', '6', ' ', ' ' },
        { ' ', ' ', '#', '#', ' ', ' ', ' ', 'I', 'G' },
      };

      //var test = new List<List<char>>();
      //test.Add(new List<char> { 'A', 'B' });
      //test.Add(new List<char> { '1' });
      //test.Add(new List<char> { 'C', 'D', 'E' });
      //test.Add(new List<char> { 'X', 'Y' });

      //foreach (var x in Permutations.Permute(test))
      //  Console.WriteLine(string.Join("", x));
      //Console.ReadLine();

      var board = new Board (b);
      board.ReadBoard();
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
