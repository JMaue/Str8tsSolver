using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Str8tsSolverLib
{
  public static class Permutations
  {
    public static IEnumerable<char[]> Permute(List<List<char>> input)
    {
      var outerSize = input.Count();
      var innerSizes = new int[outerSize];
      var innerIdx = new int[outerSize];
      var innerDivisors = new int[outerSize];
      int permutations = 1;
      for (int o=0; o<outerSize; o++)
      {
        innerSizes[o] = input[o].Count;
        permutations *= input[o].Count;
        innerIdx[o] = 0;
        if (o > 0)
          innerDivisors[o] = innerDivisors[o - 1] * innerSizes[o-1];
        else
          innerDivisors[o] = 1;
      }
      for (int o=0; o<permutations; o++)
      {
        var rc = new char[outerSize];
        for (int i=0; i<outerSize; i++)
        {
          var inner = (o / innerDivisors[i]) % innerSizes[i];
          rc[i] = input[i][inner];
        }
        yield return rc;
      }
    }

    public static IEnumerable<char[]> Permute(char[] options, int depth, int pos)
    {
      if (depth == pos)
      {
        yield return options.Take(pos).ToArray();
      }
      else
      {
        for (int i = depth; i < options.Length; i++)
        {
          Swap(ref options[depth], ref options[i]);
          foreach (var perm in Permute(options, depth + 1, pos))
          {
            yield return perm;
          }
          Swap(ref options[depth], ref options[i]); // Rückgängig machen des Swaps
        }
      }
    }

    static void Swap(ref char a, ref char b)
    {
      char temp = a;
      a = b;
      b = temp;
    }
  }
}
