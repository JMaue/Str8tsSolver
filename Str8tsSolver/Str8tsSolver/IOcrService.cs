using Str8tsSolverImageTools;

namespace Str8tsSolver
{
  public enum ImgSource
  {
    Camera,
    Screenshot,
    Photo
  }

  public interface IOcrDigitRecognizer
  {
    Task Initialize();

    void Reset();

    bool ProcessImage(byte[] image);

    char GetDigit(int row, int column);

    List<OcrElement> GetElements(int imgWidth, int imgHeight, ImgSource imgSource);
  }
}
