using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddResponseCompression(options =>
{
    options.Providers.Add<GzipCompressionProvider>();
});

var app = builder.Build();

app.Use(async (a,q) =>
{
    await q.Invoke();
    var select = a.Response.Headers
                .Select(a => "Server:"+a.Key + ":" + a.Value);
    Console.WriteLine(string.Join("\n\r",select));
});
app.UseResponseCompression();

app.MapGet("/check", () =>
{
    return "1234512345dddd";
});


Task.Run(async () =>
{
    await Task.Delay(4000);
    var handler = new HttpClientHandler
    {
       // AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
    };
    var client = new HttpClient(handler);
    client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
    var response = await client.GetStringAsync("http://localhost:5258/check");
    Console.WriteLine(response);
});

app.Run();
