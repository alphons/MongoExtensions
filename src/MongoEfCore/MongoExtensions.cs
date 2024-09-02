using MongoDB.Driver;

using System.Linq.Expressions;


namespace MongoEfCore
{
	public static class MongoExtensions
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

		public async static Task<T?> FirsOrDefaultAsync<T>(this IMongoCollection<T> collection, Expression<Func<T, bool>> filter)
		{
			var results = await collection.FindAsync(filter);

			if (results == null)
				return default;
			else
				return await results.FirstOrDefaultAsync();
		}
	}
}
