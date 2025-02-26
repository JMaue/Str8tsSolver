using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace Str8tsSolverTest
{
  internal class HttpReader
  {
    private HttpClient _httpClient;
    private PuppeteerSharp.IBrowser _browser = null;
    public HttpReader()
    {
      var uri = "https://www.str8ts.com/feed/derwesten/";
      // var uri = "https://www.str8ts.de/";
      _httpClient = new HttpClient { BaseAddress = new Uri(uri) };
    }

    public async Task Init()
    {
      //await new BrowserFetcher().DownloadAsync();
      _browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
    }

    public async Task<string> ReadAsync(int day)
    {
      //var response = await _httpClient.GetAsync($"ASStr8tsv2.asp?d={day}");
      var response = await _httpClient.GetAsync($"");
      return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> ReadUntilLoadedAsync(int id)
    {
      //using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
      using var page = await _browser.NewPageAsync();
      await page.GoToAsync(_httpClient.BaseAddress.ToString());

      for (int i = 0; i < 10; i++)
      {
        var content = await page.GetContentAsync();
        if (!content.Contains("geladen..."))
        {
          return content;
        }
        await Task.Delay(500);
      }

      return string.Empty;
    }

    public async Task<string> ReadUntilLoadedAsync2(int day)
    {
      await new BrowserFetcher().DownloadAsync();
      using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
      using var page = await browser.NewPageAsync();
      await page.GoToAsync(@$"https://www.str8ts.com/feed/derwesten/ASStr8tsv2.asp?d={day}");

      for (int i = 0; i < 10; i++)
      {
        var content = await page.GetContentAsync();
        if (content.Contains("CellNormal"))
        {
          return content;
        }
        await Task.Delay(500);
      }

      return string.Empty;
    }

    public void Dispose() {
      _browser.Dispose();
      _httpClient.Dispose();
    }
  }
}
