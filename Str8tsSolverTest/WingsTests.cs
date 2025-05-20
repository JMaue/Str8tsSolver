using Str8tsSolverLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Str8tsSolverTest
{
  internal class WingsTests
  {

    [Test]
    public void FindWing()
    {
      char[,] b = new char[,]
      { // hard
        { ' ', ' ', '2', ' ', '3', ' ', ' ', ' ', ' ' },
        { 'H', '#', ' ', ' ', ' ', ' ', '5', ' ', '6' },
        { ' ', ' ', ' ', '#', ' ', ' ', ' ', ' ', ' ' },
        { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '7' },
        { ' ', '4', '#', ' ', ' ', ' ', '#', ' ', ' ' },
        { '#', 'E', '#', ' ', ' ', '#', ' ', ' ', '4' },
        { '#', ' ', ' ', ' ', ' ', '2', ' ', '5', 'A' },
        { ' ', ' ', ' ', ' ', ' ', ' ', ' ', '9', '3' },
        { ' ', ' ', ' ', ' ', ' ', '3', ' ', ' ', '2' }};
      var board = new Str8tsSolverLib.Board(b);
      board.ReadBoard();
      board.AssignCandidates(0, 0, new List<int> { 1, 4, 5, 6, 7, 9 });
      board.AssignCandidates(0, 1, new List<int> { 1, 6, 9 });
      board.AssignCandidates(0, 2, new List<int>());
      board.AssignCandidates(0, 3, new List<int> { 1, 8 });
      board.AssignCandidates(0, 4, new List<int>());
      board.AssignCandidates(0, 5, new List<int> { 4, 5, 6, 7, 8, 9 });
      board.AssignCandidates(0, 6, new List<int> { 6, 7, 8 });
      board.AssignCandidates(0, 7, new List<int> { 1, 4, 6, 7, 8 });
      board.AssignCandidates(0, 8, new List<int> { 5, 8, 9 });

      board.AssignCandidates(1, 0, new List<int>());
      board.AssignCandidates(1, 1, new List<int>());
      board.AssignCandidates(1, 2, new List<int> { 1, 3, 4 });
      board.AssignCandidates(1, 3, new List<int> { 2, 7 });
      board.AssignCandidates(1, 4, new List<int> { 1, 2, 4, 7 });
      board.AssignCandidates(1, 5, new List<int> { 4, 7 });
      board.AssignCandidates(1, 6, new List<int>());
      board.AssignCandidates(1, 7, new List<int> { 1, 2, 3, 4, 7 });
      board.AssignCandidates(1, 8, new List<int>());

      board.AssignCandidates(2, 0, new List<int> { 1, 2, 3, 4 });
      board.AssignCandidates(2, 1, new List<int> { 2, 3 });
      board.AssignCandidates(2, 2, new List<int> { 1, 3, 4 });
      board.AssignCandidates(2, 3, new List<int>());
      board.AssignCandidates(2, 4, new List<int> { 4, 5, 6, 7, 8 });
      board.AssignCandidates(2, 5, new List<int> { 4, 5, 6, 7, 8 });
      board.AssignCandidates(2, 6, new List<int> { 6, 7, 8 });
      board.AssignCandidates(2, 7, new List<int> { 4, 6, 7, 8 });
      board.AssignCandidates(2, 8, new List<int> { 5, 8, 9 });
      board.AssignCandidates(3, 0, new List<int> { 1, 2, 3, 4, 5, 6 });
      board.AssignCandidates(3, 1, new List<int> { 2, 3 });
      board.AssignCandidates(3, 2, new List<int> { 1, 3, 4, 5 });
      board.AssignCandidates(3, 3, new List<int> { 1, 2, 3, 4, 5, 6, 8, 9 });
      board.AssignCandidates(3, 4, new List<int> { 1, 2, 4, 5, 6, 8, 9 });
      board.AssignCandidates(3, 5, new List<int> { 4, 5, 6, 8, 9 });
      board.AssignCandidates(3, 6, new List<int> { 6, 8 });
      board.AssignCandidates(3, 7, new List<int> { 1, 2, 3, 4, 6, 8 });
      board.AssignCandidates(3, 8, new List<int>());
      board.AssignCandidates(4, 0, new List<int> { 3, 5 });
      board.AssignCandidates(4, 1, new List<int>());
      board.AssignCandidates(4, 2, new List<int>());
      board.AssignCandidates(4, 3, new List<int> { 5, 6, 7, 8, 9 });
      board.AssignCandidates(4, 4, new List<int> { 5, 6, 7, 8, 9 });
      board.AssignCandidates(4, 5, new List<int> { 5, 6, 7, 8, 9 });
      board.AssignCandidates(4, 6, new List<int>());
      board.AssignCandidates(4, 7, new List<int> { 6, 8 });
      board.AssignCandidates(4, 8, new List<int> { 5, 9 });

      board.AssignCandidates(5, 0, new List<int>());
      board.AssignCandidates(5, 1, new List<int>());
      board.AssignCandidates(5, 2, new List<int>());
      board.AssignCandidates(5, 3, new List<int> { 6, 7, 8, 9 });
      board.AssignCandidates(5, 4, new List<int> { 6, 7, 8, 9 });
      board.AssignCandidates(5, 5, new List<int>());
      board.AssignCandidates(5, 6, new List<int> { 2, 3 });
      board.AssignCandidates(5, 7, new List<int> { 2, 3 });
      board.AssignCandidates(5, 8, new List<int>());

      board.AssignCandidates(6, 0, new List<int>());
      board.AssignCandidates(6, 1, new List<int> { 6, 7, 8 });
      board.AssignCandidates(6, 2, new List<int> { 6, 7, 8 });
      board.AssignCandidates(6, 3, new List<int> { 3, 4, 6, 7, 8 });
      board.AssignCandidates(6, 4, new List<int> { 4, 6, 7, 8 });
      board.AssignCandidates(6, 5, new List<int>());
      board.AssignCandidates(6, 6, new List<int> { 3, 4 });
      board.AssignCandidates(6, 7, new List<int>());
      board.AssignCandidates(6, 8, new List<int>());

      board.AssignCandidates(7, 0, new List<int> { 2, 4, 5, 6, 7 });
      board.AssignCandidates(7, 1, new List<int> { 6, 7, 8 });
      board.AssignCandidates(7, 2, new List<int> { 5, 6, 7, 8 });
      board.AssignCandidates(7, 3, new List<int> { 1, 2, 4, 5, 6, 7, 8 });
      board.AssignCandidates(7, 4, new List<int> { 1, 2, 4, 5, 6, 7, 8 });
      board.AssignCandidates(7, 5, new List<int> { 1, 4 });
      board.AssignCandidates(7, 6, new List<int> { 1, 2, 4 });
      board.AssignCandidates(7, 7, new List<int>());
      board.AssignCandidates(7, 8, new List<int>());

      board.AssignCandidates(8, 0, new List<int> { 1, 4, 5, 6, 7 });
      board.AssignCandidates(8, 1, new List<int> { 6, 7, 8, 9 });
      board.AssignCandidates(8, 2, new List<int> { 5, 6, 7, 8, 9 });
      board.AssignCandidates(8, 3, new List<int> { 1, 4, 5, 6, 7, 8, 9 });
      board.AssignCandidates(8, 4, new List<int> { 1, 4, 5, 6, 7, 8, 9 });
      board.AssignCandidates(8, 5, new List<int>());
      board.AssignCandidates(8, 6, new List<int> { 1, 4 });
      board.AssignCandidates(8, 7, new List<int> { 1, 4, 6, 7, 8 });
      board.AssignCandidates(8, 8, new List<int>());

      var ws = new ExcludeWings();
      ws.Solve(board, board.Str8ts[0]);
    }
  }
}
