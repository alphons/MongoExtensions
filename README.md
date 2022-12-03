# MongoDB.MvcCore
Serialization Extensions to MvcCore for using MongoDB

The MongoDB.MvcCore contains extensions to the MongDB framework where Json strings can be used as input and output can be deserialized to Json.

The deserializer can also be used as a pretty-print formatter. On console apps the pretty-print feature can ben colorized.

![pretty print colored](https://github.com/alphons/MongoDB.MvcCore/blob/main/blob/PrettyPrintColored.png?raw=true)

```
using MongoDB.Driver;
using MongoDB.MvcCore;

var services = builder.Services;

var mongoclient = new MongoClient("mongodb://127.0.0.1:27017");

services.AddSingleton<IMongoClient>(mongoclient);

services
    .AddMvcCore()
    .AddBsonJsonConverters(); // Serialization in API controller
```

```
public class MongoController : ControllerBase
{
	private readonly IMongoClient mongo;
	public MongoController(IMongoClient mongoClient)
	{
		this.mongo = mongoClient;
	}

	[HttpGet]
	[Route("~/api/ShowCollection")]
	public async Task<IActionResult> ShowCollection()
	{
		var List = await mongo
			.GetDatabase("demo")
			.GetCollection("collection1")
			.Find("{}")
			.ToListAsync();

		return Ok(new
		{
			List
		});
	}
}
```
