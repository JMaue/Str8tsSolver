using Str8tsSolverLib;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;

namespace Str8tsSolverTest
{
  public class ConsoleTxtOut : ITxtOut
  {
    void ITxtOut.SetColors(Color foreground, Color background)
    {
      Console.ForegroundColor = foreground == Color.White ? ConsoleColor.White : ConsoleColor.Black;
      Console.BackgroundColor = background == Color.White ? ConsoleColor.White : ConsoleColor.Black;
    }

    void ITxtOut.Write(string text) => Console.Write(text);

    void ITxtOut.Write(char c) => Console.Write(c);


    void ITxtOut.WriteLine() => Console.WriteLine();

    void ITxtOut.WriteLine(string text) => Console.WriteLine(text);
  }
}
