using Str8tsSolverLib;

namespace Str8tsSolverTest
{
  public class Str8tsSolverTest
  {
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
      char[,] b = new char[,]
      { // devilish, 
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
      var board = new Board(b);
      board.ReadBoard();
      board.PrintBoard(true);
      var solved = Str8tsSolver.Solve(board, out int iterations);
      Assert.IsTrue(solved);
    }

    [Test]
    public void Test2()
    {
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
      var board = new Board(b);
      board.ReadBoard();
      board.PrintBoard(true);
      var solved = Str8tsSolver.Solve(board, out int iterations);
      Assert.IsTrue(solved);
    }


    [Test]
    public void Test3()
    {
      char[,] b = new char[,]
      { // devilish
        { 'F', ' ', ' ', '#', '2', ' ', ' ', '#', '#' },
        { ' ', ' ', '7', ' ', '#', '#', ' ', ' ', '#' },
        { ' ', ' ', '#', ' ', ' ', '#', '#', ' ', ' ' },
        { '9', ' ', '#', ' ', ' ', ' ', ' ', '6', ' ' },
        { '#', ' ', ' ', 'H', ' ', ' ', ' ', ' ', '#' },
        { ' ', ' ', ' ', ' ', 'I', '#', ' ', ' ', ' ' },
        { ' ', ' ', '#', ' ', ' ', ' ', 'A', ' ', ' ' },
        { '#', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '#' },
        { '#', ' ', ' ', '#', ' ', ' ', ' ', ' ', 'C' },
      };
      var board = new Board(b);
      board.ReadBoard();
      board.PrintBoard(true);
      var solved = Str8tsSolver.Solve(board, out int iterations);
      Assert.IsTrue(solved);
    }
  }
}