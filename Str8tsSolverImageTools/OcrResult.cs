using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Str8tsSolverImageTools
{
  /// <summary>
  /// The words of the OCR result.
  /// </summary>
  public class OcrElement
  {
    /// <summary>
    /// The confidence of the OCR result.
    /// </summary>
    public float Confidence { get; set; }

    /// <summary>
    /// The height of the element.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// The text of the element.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// The width of the element.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// The X coordinates of the element.
    /// </summary>
    public int X { get; set; }

    /// <summary>
    /// The Y coordinates of the element.
    /// </summary>
    public int Y { get; set; }
  }
}
