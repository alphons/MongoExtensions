using MongoDB.Bson;
using MongoDB.Driver;


namespace MongoExample;

public class CrmContext() : MongoDbContext("Crm")
{
	public IMongoCollection<Klant> KlantTable => Table<Klant>();
	public IMongoCollection<Hypotheek> HypotheekTable => Table<Hypotheek>();
}

public class Hypotheek
{
	public ObjectId Id { get; set; }
	public string? Name { get; set; }
	public string? Rente { get; set; }
}

public class Klant
{
	public ObjectId Id { get; set; }
	public string? Name { get; set; }
	public string? Address { get; set; }
	public int Age { get; set; }
}

