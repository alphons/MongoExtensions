
using MongoDB.Driver;
using MongoDB.MvcCore;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    ContentRootPath = AppDomain.CurrentDomain.BaseDirectory
});

var services = builder.Services;

var mongoclient = new MongoClient(builder.Configuration.GetSection("Mongo")["ConnectionString1"]);

services.AddSingleton<IMongoClient>(mongoclient);

services
    .AddMvcCore()
    .WithMultiParameterModelBinding()
    .AddBsonJsonConverters(); // Serialization in API controller



var app = builder.Build();

app.MapControllers();

app.UseDefaultFiles();

app.UseStaticFiles();

app.Run();
