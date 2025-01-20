using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using HtmlAgilityPack;
using Str8tsSolverLib;

namespace Str8tsSolverTest
{
  internal class BulkTest
  {

    public char[,] ReadBoard(int day)
    {
      var reader = new HttpReader();
      var html = reader.ReadUntilLoadedAsync2(day).Result;

      // Load the HTML into an HtmlDocument
      var htmlDoc = new HtmlDocument();
      htmlDoc.LoadHtml(html);

      // Extract the <table id="boardtable"> node
      var tableNode = htmlDoc.DocumentNode.SelectSingleNode("//table[@id='boardtable']");

      // Check if the tableNode is not null
      Assert.IsNotNull(tableNode, "The table with id 'boardtable' was not found.");

      // Extract the <tbody> node
      var tbodyNode = tableNode.SelectSingleNode("tbody");
      Assert.IsNotNull(tbodyNode, "The tbody element was not found in the table.");

      var board = new char[9, 9];
      int row = 0;
      // Iterate over all <tr> nodes within the <tbody>
      foreach (var trNode in tbodyNode.SelectNodes("tr"))
      {
        int col = 0;
        // Iterate over all <td> nodes within the <tr>
        foreach (var tdNode in trNode.SelectNodes("td"))
        {
          // Extract the attributes id and class
          var id = tdNode.GetAttributeValue("id", string.Empty);
          var classAttr = tdNode.GetAttributeValue("class", string.Empty);
          var innerHtml = tdNode.InnerHtml;

          // Output the extracted information
          var val = 0;
          if (int.TryParse(innerHtml, out val))
          {
            if (classAttr == "CellGiven" && val != ' ')
              board[row, col] = innerHtml[0];
            else
              board[row, col] = (char)('A' + val - 1);
          }
          else if (classAttr == "CellBlack")
            board[row, col] = '#';
          else 
            board[row, col] = '.';

          col++;
        }
        row++;
      }
      return board;
    }

    [Test]
    public void ReadBoard_30Days()
    {
      var day = DateTime.Now;
      for (int i=0; i<=30; i++)
      {
        var board = ReadBoard(i);
        var d = day.AddDays(-i);
        SaveBoardToFile(board, $"board_{d.ToString("yyyyMMdd")}.txt");
      }
    }

    private void SaveBoardToFile(char[,] board, string filePath)
    {
      using (var writer = new StreamWriter(filePath))
      {
        for (int i = 0; i < board.GetLength(0); i++)
        {
          for (int j = 0; j < board.GetLength(1); j++)
          {
            writer.Write(board[i, j]);
          }
          writer.WriteLine();
        }
      }
    }

  }
}
