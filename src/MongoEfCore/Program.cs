using MongoDB.Driver;
using MongoDB.Bson;
using MyExtensions;

//var client = new MongoClient("mongodb://192.168.28.123");
var client = new MongoClient("mongodb://127.0.0.1");

if (client == null) return;

var klanten = client.GetDatabase("dbCrm").GetCollection<Klant>("KlantenTable");

if (klanten == null) return;

var indexKeysDefinition = Builders<Klant>.IndexKeys.Ascending(x => x.Name);

var indexName = await klanten.Indexes.CreateOneAsync(new CreateIndexModel<Klant>(indexKeysDefinition));

var klant = klanten.FirstOrDefault(x => x.Name == "Jochem");

if(klant == null)
{
	klant = new Klant()
	{
		Name = "Jochem",
		Address = "Huizen",
		Age = 35
	};

	await klanten.InsertOneAsync(klant);
}

Console.WriteLine($"Het {klant.Address}");



class Klant
{
	public ObjectId Id { get; set; }
	public string? Name { get; set; }
	public string? Address { get; set; }
	public int Age { get; set; }
}

