using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Str8tsSolverLib
{
  interface IAlgorithm
  {
    bool Solve(Board board, Str8t str8t);
  }

  public interface ITxtOut
  {
    void Write(string text);
    void Write(char c);
    void WriteLine();
    void WriteLine(string text);
    void SetColors (Color foreground, Color background);
  }

  public static class Str8tsSolver
  {

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

    public static (bool, int) Solve(Board board, ITxtOut? txtOut = null)
    {
      int iterations = 0;
      board.TxtOut = txtOut;

      var algorithms = new List<IAlgorithm>
      {
        //new SingleGapInStr8t(),
        new PermuteOptions(),
        new PermuteCandidates(),
        new ExcludeWings()
      };
      bool progress;
      do
      {
        progress = false;
        var alg = iterations == 0 ? algorithms[0] :  iterations % 2 != 0 ? algorithms[1] : algorithms[1];
        //foreach (var alg in algorithms)
        {
          var sortedStr8ts = board.Str8ts.OrderBy(s => s.Len).ToList();
          foreach (var str8t in sortedStr8ts)
          {
            if (!str8t.IsSolved())
            {
              board.ReportProgress($"{str8t}");
              progress |= alg.Solve(board, str8t);
              txtOut?.WriteLine($"Algorithm {alg.GetType().Name} finished. Str8t:{str8t}");
              //board.PrintBoard(true);
            }
          }
          txtOut?.WriteLine($"Algorithm {alg.GetType().Name} finished. Iterations:{iterations}, Progress:{progress}");
          //if (iterations >= 1)
            board.PrintBoard(true);
        }

        iterations++;

        if (!progress)
        {
          alg = algorithms[2];
          iterations++;
          progress = alg.Solve(board, board.Str8ts[0]);
          txtOut?.WriteLine($"Algorithm {alg.GetType().Name} finished. Iterations:{iterations}, Progress:{progress}");
        }

      } while (progress || iterations == 1);
      
      return (board.Finish(), iterations);
    }
  }
}
