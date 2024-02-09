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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}

app.UseHttpsRedirection();

app.MapGet("/weatherreport", async () =>
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
            double.TryParse(item[3].Replace("*", ""), out var rainfall))
        {
            return new WeatherReport
            {
                City = item[0].ToLower(), 
                MaxTemp = maxTemp,
                MinTemp = minTemp,
                Rainfall = rainfall,
            };
        }
        return null;
    })
    .Where(weatherReport => weatherReport != null)
    .ToList();

})
.WithName("GetWeatherReport")
.WithOpenApi();

app.Run();

record WeatherReport(string City, double MinTemp, double MaxTemp, double Rainfall)
{
    public int MinTempF => 32 + (int)(MinTemp / 0.5556);
    public int MaxTempF => 32 + (int)(MaxTemp / 0.5556);
}
