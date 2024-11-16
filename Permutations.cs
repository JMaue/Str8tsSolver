using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Str8tsSolver
{
  public static class Permutations
  {
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
