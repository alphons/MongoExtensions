
using MongoDB.Driver;

using MyExtensions;
using MongoEfCore;


var db = new DbContext("Crm");


//var indexKeysDefinition = Builders<Klant>.IndexKeys.Ascending(x => x.Name);

//var indexName = await db.KlantTable.Indexes.CreateOneAsync(new CreateIndexModel<Klant>(indexKeysDefinition));

var klant = db.KlantTable.FirstOrDefault(x => x.Name == "Jochem");

if(klant == null)
{
	klant = new Klant()
	{
		Name = "Jochem",
		Address = "Huizen",
		Age = 35
	};

	await db.KlantTable.InsertOneAsync(klant);
}

Console.WriteLine($"Het {klant.Address}");


