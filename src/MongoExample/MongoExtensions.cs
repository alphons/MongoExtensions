using MongoDB.Driver;
using System.Configuration;
using System.Linq.Expressions;
using System.Xml.Linq;


namespace MongoDb.Extensions;

public static class LinqExtensions
{
	public static IFindFluent<T, T>? Where<T>(this IMongoCollection<T> collection, Expression<Func<T, bool>> filter)
	{
		return collection.Find(filter);
	}

	public static T? FirstOrDefault<T>(this IMongoCollection<T> collection, Expression<Func<T, bool>> filter)
	{
		var results = collection.Find(filter);

		return results.FirstOrDefault();
	}

	public async static Task<T?> FirstOrDefaultAsync<T>(this IMongoCollection<T> collection, Expression<Func<T, bool>> filter)
	{
		var results = await collection.FindAsync(filter);

		if (results == null)
			return default;
		else
			return await results.FirstOrDefaultAsync();
	}

	public async static Task<List<T>> ToListAsync<T>(this IMongoCollection<T> collection, Expression<Func<T, bool>> filter)
	{
		var results = await collection.FindAsync(filter);

		if (results == null)
			return [];
		else
			return await results.ToListAsync();
	}
}

public class MongoDbContext
{
	public readonly MongoClient Client;

	private readonly IMongoDatabase db;

	public MongoDbContext(string name)
	{
		Client = new(ConfigurationManager.ConnectionStrings[name].ConnectionString);

		db = Client.GetDatabase(name);
	}

	public IMongoCollection<T> Table<T>() => db.GetCollection<T>($"{typeof(T).Name}Table");
}

