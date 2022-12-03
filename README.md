# MongoDB.MvcCore
Serialization Extensions to MvcCore for using MongoDB 

![pretty print colored](https://github.com/alphons/[MongoDB.MvcCore/tree]/blob/PrettyPrintColored.png?raw=true)


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
