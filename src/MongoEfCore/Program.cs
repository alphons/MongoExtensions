using MongoDB.Driver;
using MongoDB.Bson;
using MyExtensions;

var client = new MongoClient("mongodb://192.168.28.123:27017");

if (client == null)
	return;

var klanten = client.GetDatabase("dbCrm").GetCollection<Klant>("KlantenTable");

var indexKeysDefinition = Builders<Klant>.IndexKeys.Ascending(x => x.Name);
await klanten.Indexes.CreateOneAsync(new CreateIndexModel<Klant>(indexKeysDefinition));


if (klanten == null) 
	return;

//var klant = new Klant()
//{
//	Name = "Jochem",
//	Address = "Huizen",
//	Age = 35
//};

//await klanten.InsertOneAsync(klant);


var wie = klanten.FirstOrDefault(x => x.Name == "Jochem");

if(wie == null) return;

Console.WriteLine($"Het {wie.Address}");



class Klant
{
	public ObjectId Id { get; set; }
	public string? Name { get; set; }

	public string? Address { get; set; }

	public int Age { get; set; }
}

