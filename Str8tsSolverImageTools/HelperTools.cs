using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Str8tsSolverImageTools
{
  public class KMeansClustering
  {
    public static (int[], int[]) ClassifyIntoClusters(int[] vals)
    {
      // Initialisieren Sie die Clusterzentren
      int blackClusterCenter = vals.Min();
      int whiteClusterCenter = vals.Max();

      int[] blackCluster = new int[vals.Length];
      int[] whiteCluster = new int[vals.Length];
      int blackCount = 0, whiteCount = 0;

      bool changed;
      do
      {
        changed = false;
        blackCount = 0;
        whiteCount = 0;

        // Zuweisen der Werte zu den nächsten Clustern
        foreach (var val in vals)
        {
          if (Math.Abs(val - blackClusterCenter) < Math.Abs(val - whiteClusterCenter))
          {
            blackCluster[blackCount++] = val;
          }
          else
          {
            whiteCluster[whiteCount++] = val;
          }
        }

        // Berechnen Sie die neuen Clusterzentren
        int newBlackClusterCenter = blackCluster.Take(blackCount).Sum() / blackCount;
        int newWhiteClusterCenter = whiteCluster.Take(whiteCount).Sum() / whiteCount;

        if (newBlackClusterCenter != blackClusterCenter || newWhiteClusterCenter != whiteClusterCenter)
        {
          blackClusterCenter = newBlackClusterCenter;
          whiteClusterCenter = newWhiteClusterCenter;
          changed = true;
        }
      } while (changed);

      return (blackCluster.Take(blackCount).ToArray(), whiteCluster.Take(whiteCount).ToArray());
    }
  }
}
