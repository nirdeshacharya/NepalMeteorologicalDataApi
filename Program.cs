var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");
    });
}

app.UseHttpsRedirection();

app.MapGet("/GetWeatherReport", async (string city) =>
{
    var webClient = new HttpClient();
    string page = await webClient.GetStringAsync("http://www.mfd.gov.np/weather/");

    var doc = new HtmlAgilityPack.HtmlDocument();
    doc.LoadHtml(page);

    var table = doc.DocumentNode.SelectSingleNode("//table[@class='table']")
        .Descendants("tr")
        .Skip(1)
        .Where(tr => tr.Elements("td").Count() > 1)
        .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
        .ToList();

    
   var result = Enumerable.Range(0, table.Count)
    .Select(index =>
    {
        var item = table[index];

        if (double.TryParse(item[1], out var maxTemp) &&
            double.TryParse(item[2], out var minTemp) &&
            double.TryParse(item[3].Replace("*", ""), out var rainfall) &&
            item[0].ToLower() == city.ToLower())
        {
            return new WeatherReport
            {
                City = item[0], 
                MaxTemp = maxTemp,
                MinTemp = minTemp,
                Rainfall = rainfall,
            };
        }
        return null;
    })
    .Where(weatherReport => weatherReport != null)
    .ToList();
   return result;
})
.WithDescription("Only works with major cities: Dipayal, Dadeldhura, Dhangadi, Birendranagar, Nepalgunj, Jumla, Ghorahi, Pokhara, Bhairahawa, Simara, Kathmandu, Okhaldhunga, Taplejung, Dhankuta, Biratnagar, Jomsom, Dharan, Lumle, Janakpur, Jiri")
.WithOpenApi();

app.Run();

record WeatherReport(string City, double MinTemp, double MaxTemp, double Rainfall)
{
    public WeatherReport() : this("", 0.0, 0.0, 0.0)
    {
    }
    public int MinTempF => (int)(MinTemp * 9 / 5) + 32;
    public int MaxTempF => (int)(MaxTemp * 9 / 5) + 32;
}
