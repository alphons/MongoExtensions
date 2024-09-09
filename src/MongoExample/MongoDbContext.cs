using MongoDB.Driver;

using System.Configuration;


namespace MongoExample;

public class MongoDbContext(string name)
{
	private readonly IMongoDatabase db = new MongoClient(ConfigurationManager.ConnectionStrings["mongo"].ConnectionString)
	.GetDatabase(name);
	public IMongoCollection<T> Table<T>() => db.GetCollection<T>($"{typeof(T).Name}Table");
}
