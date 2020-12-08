using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Whether.Common;

namespace Whether.Connectors
{
    public class MeteoByConnector : IConnector
    {
        IWhetherLogger _logger;
        public MeteoByConnector(IWhetherLogger logger)
        {
            _logger = logger;
        }
        public async Task<List<WhetherDay>> GetWhetherOnDay(DateTime day, string city)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var result = new List<WhetherDay>();
            var doc = new HtmlDocument();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"https://meteo.by/{city}/retro/{day.ToString("yyyy-M-d")}/");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            //request.Timeout = 1000;
            try
            {
                var response = (HttpWebResponse)await request.GetResponseAsync();
                using (Stream stream = response.GetResponseStream())
                {
                    doc.Load(stream, Encoding.GetEncoding("windows-1251"));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }


            if (doc.DocumentNode.Descendants("body").Any())
            {
                foreach (HtmlNode table in doc.DocumentNode.SelectNodes("//table").Where(x => x.HasClass("t-weather")))
                {
                    var dayPart = 1;
                    foreach (HtmlNode row in table.SelectNodes("tr").Where(x => x.HasClass("time")))
                    {
                        var whetherDay = new WhetherDay();
                        var cellTemp = row.SelectNodes("td").Where(x => x.HasClass("temp")).FirstOrDefault();
                        whetherDay.Temperature = cellTemp.SelectNodes("span").FirstOrDefault().InnerText.Trim().Replace(" ", "").Replace("\n", " ");
                        var cellCloud = row.SelectNodes("td").Where(x => x.HasClass("icon")).FirstOrDefault();
                        whetherDay.Clouds = TransforClouds(cellCloud.InnerHtml.Trim().Replace("  ", "").Replace("\n", ""));

                        var cellsData = row.SelectNodes("td").Where(x => x.HasClass("data")).ToList();
                        whetherDay.Preasure = cellsData[0].InnerText;
                        whetherDay.Humidity = cellsData[1].InnerText;
                        whetherDay.Wind = cellsData[2].InnerText;
                        whetherDay.WindDirection = row.SelectNodes("td").Where(x => x.HasClass("dir")).FirstOrDefault().InnerHtml.Trim().Replace("  ", "");
                        whetherDay.DayPart = (DayPartEnum)dayPart;
                        whetherDay.Date = DateTime.SpecifyKind(day.Date, DateTimeKind.Utc);
                        whetherDay.City = city;
                        result.Add(whetherDay);
                        dayPart++;
                    }
                }
            }
            return result;
        }

        private static string TransforClouds(string cloud)
        {
            if (!string.IsNullOrEmpty(cloud))
            {
                cloud = cloud.Replace("<p>", "").Replace("</p>", "").Replace("<p class=\"night\">", "");
                var endOfImage = cloud.IndexOf(">");

                cloud = cloud.Substring(0, endOfImage) + $"title='{cloud.Substring(endOfImage + 1, cloud.Length - endOfImage - 1)}'>";
            }
            return cloud;
        }
    }
}
