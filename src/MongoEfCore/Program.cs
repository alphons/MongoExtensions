
using BsonExtensions;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

using MongoDB.Bson;

using MongoEfCore;
using System.Diagnostics;

var connectionString = "mongodb://192.168.28.123:27017";

if (1 == 1)
{
	var sw1 = Stopwatch.StartNew();

	var settings = MongoClientSettings.FromConnectionString($"{connectionString}");

	settings.ServerSelectionTimeout = TimeSpan.FromSeconds(1);

	var dbClient = new MongoClient(settings);

	var movies = dbClient?.GetDatabase("testdb1").GetCollection<Movie>("movies");

	var aaa1 = movies.FirstOrDefault(x => x._id == ObjectId.Parse("66d48adca720951e07550d78"));

	var aaa2 = await movies.Where(x => x._id ==ObjectId.Parse("66d48adca720951e07550d78")).FirstOrDefaultAsync();

	var aaa3 = await movies.FirsOrDefaultAsync(x => x._id == ObjectId.Parse("66d48adca720951e07550d78"));

	for (int i = 0; i < 1000; i++)
	{
		await movies.FirsOrDefaultAsync(x => x._id == ObjectId.Parse("66d48adca720951e07550d78"));
	}


	var movie1 = await movies.FirsOrDefaultAsync(x => x._id == ObjectId.Parse("66d48adca720951e07550d78"));


	Console.WriteLine($"{sw1.ElapsedMilliseconds}mS {movie1.Title}");

	return;
}

//====================================

var sw2 = Stopwatch.StartNew();


var client = new MongoClient(connectionString);

var db = MyDbContext.Create(client.GetDatabase("testdb1"));

//db.Database.EnsureCreated();

for (int i = 0; i < 1000; i++)
{
	await db.Movies.AsNoTracking().FirstOrDefaultAsync(m => m._id == ObjectId.Parse("66d48adca720951e07550d78"));
}
var movie2 = await db.Movies.AsNoTracking().FirstOrDefaultAsync(m => m._id == bjectId.Parse("66d48adca720951e07550d78"));

Console.WriteLine($"{sw2.ElapsedMilliseconds}mS {movie2.Title}");

//if(movie == null)
//{
//	movie = new Movie()
//	{
//		Title = "Back to the Future",
//		Rated = "9",
//		Plot = "people in the past going to the future"
//	};
//	db.Movies.Add(movie);
//	var status = await db.SaveChangesAsync();
//}

//Console.WriteLine(movie.Plot);
