using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Str8tsSolverLib
{
  public class Cell
  {
    public static char[] ValidCells = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
    public static char[] BlackCells = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I' };
    public int Col { get; set; }
    public int Row { get; set; }

    private Func<int, int, char> _getChar;

    public char[,] Presentation
    {
      get
      {
        var rc = new char[3, 3]
        {
          { ' ', ' ', ' ' },
          { ' ', ' ', ' ' },
          { ' ', ' ', ' ' }
        };
        var c = Value;
        if (ValidCells.Contains(c))
          rc[1, 1] = c;
        else if (c == '#')
          rc[1, 1] = 'X';
        else if (BlackCells.Contains(c))
          rc[1, 1] = (char)(c - 'A' + '1');
        else
        {
          if (Candidates.Any())
          {
            for (int i = 1; i <= 9; i++)
            {
              if (Candidates.Contains((char)(i + '0')))
                rc[(i - 1) / 3, (i - 1) % 3] = '*';
            }
          }
        }
        return rc;
      }
    }

    public bool IsBlack => Value == '#' || BlackCells.Contains(Value);

    public Cell(int r, int c, Func<int, int, char> ch)
    {
      Row = r;
      Col = c;
      _getChar = ch;
    }

    public char Value => _getChar(Row, Col);

    public Str8t? Horizontal { get; set; }
    public Str8t? Vertical { get; set; }

    public List<int> Candidates = new List<int>();

    internal bool UpdateCandidates(List<int> list)
    {
      var noOfCandidates = Candidates.Count;
      if (Candidates.Count == 0 && Value == ' ')
      {
        Candidates.AddRange(list.Distinct());
        return false;
      }

      Candidates.RemoveAll(c => !list.Contains(c));
      return noOfCandidates > Candidates.Count; // less candidates means progress!
    }

    internal bool WouldEraseCandidates(char[] values)
    {
      if (Value != ' ')
        return false;

      if (Candidates.Count == 0)
        return false;

      return Candidates.All(c => values.Contains((char)c));
    }
  }
}
