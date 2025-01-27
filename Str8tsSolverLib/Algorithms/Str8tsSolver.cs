using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Str8tsSolverLib
{
  interface IAlgorithm
  {
    bool Solve(Board board, Str8t str8t);
  }

  public static class Str8tsSolver
  {
    public static string ReplaceFirst(string text, char search, char replace)
    {
      int pos = text.IndexOf(search);
      if (pos < 0)
      {
        return text;
      }
      return text.Substring(0, pos) + replace + text.Substring(pos + 1);
    }

    public static List<char> FindIntersection(List<char[]> charArrays)
    {
      if (charArrays == null || charArrays.Count == 0)
        return new List<char>();

      var intersection = new HashSet<char>(charArrays[0]);
      foreach (var array in charArrays.Skip(1))
      {
        intersection.IntersectWith(array);
      }

      return intersection.ToList();
    }

    public static bool Solve(Board board, out int iterations)
    {
      iterations = 0;
      var algorithms = new List<IAlgorithm>
      {
        //new SingleGapInStr8t(),
        new PermuteOptions(),
        new PermuteCandidates()
        //new ExcludeNakedPairs()
      };
      bool progress;
      do
      {
        progress = false;
        var alg = iterations == 0 ? algorithms[0] : algorithms[1];
        //foreach (var alg in algorithms)
        {
          var sortedStr8ts = board.Str8ts.OrderBy(s => s.Len).ToList();
          foreach (var str8t in sortedStr8ts)
          {
            if (!str8t.IsSolved())
            {
              progress |= alg.Solve(board, str8t);
              Console.WriteLine($"Algorithm {alg.GetType().Name} finished. Str8t:{str8t}");
              //board.PrintBoard(true);
              //if (iterations > 1)
               // Console.ReadLine();
            }
          }
          Console.WriteLine($"Algorithm {alg.GetType().Name} finished. Iterations:{iterations}, Progress:{progress}");
          //if (iterations >= 1)
            board.PrintBoard(true);
        }

        iterations++;

      } while (progress || iterations == 1);

      return board.IsSolved && board.IsValid();
    }
  }
}
