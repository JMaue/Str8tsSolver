using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Str8tsSolver
{
  interface IAlgorithm
  {
    bool Solve(Board board, Str8t str8t);
  }

  public static class Str8tsSolver
  {
    public static bool Solve(Board board, out int iterations)
    {
      iterations = 0;
      var algorithms = new List<IAlgorithm>
      {
        //new SingleGapInStr8t(),
        new PermuteOptions()
      };
      bool progress;
      do
      {
        progress = false;
        foreach (var str8t in board.Str8ts)
        {
          if (!str8t.IsSolved())
          {
            foreach (var alg in algorithms)
            {
              progress |= alg.Solve(board, str8t);
            }
          }
        }
        board.PrintBoard();
       
        if (progress)
          iterations++;

      } while (progress);

      return board.IsSolved;
    }
  }
}
