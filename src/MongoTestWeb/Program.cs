
using MongoDB.Driver;
using MongoExtensions;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    ContentRootPath = AppDomain.CurrentDomain.BaseDirectory
});

var services = builder.Services;

var mongoclient = new MongoClient(builder.Configuration.GetSection("Mongo")["ConnectionString2"]);

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
