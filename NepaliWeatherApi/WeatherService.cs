using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NepaliWeatherApi
{

    public class WetherModel
    {
        public string City { get; set; }
        public decimal MinTemp { get; set; }
        public decimal MaxTemp { get; set; }
        public decimal Rainfall { get; set; }
    }
    public class WeatherService
    {
      
        public List<WetherModel> GetWeatherData(string city)
        {
            return city!=null?ParseWeather().Where(x=>x.City==city).ToList():ParseWeather().ToList();
        }
        public List<WetherModel> ParseWeather()
        {
            WebClient webClient = new WebClient();
            string page = webClient.DownloadString("http://www.mfd.gov.np/weather/");

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(page);

            var table = doc.DocumentNode.SelectSingleNode("//table[@class='table']")
                        .Descendants("tr")
                        .Skip(1)
                        .Where(tr => tr.Elements("td").Count() > 1)
                        .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
                        .ToList();
            var result = new List<WetherModel>();
           foreach(var item in table)
            {
               
                result.Add(new WetherModel
                {
                    City = item[0].ToLower(),
                    MaxTemp = Convert.ToDecimal(item[1]),
                    MinTemp = Convert.ToDecimal(item[2]),
                    Rainfall = Convert.ToDecimal(item[3].Replace("*","")),
                });
            }
            return result;
            
        }
    }
        
}
