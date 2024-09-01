
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

using BsonExtensions;

using MongoEfCore;
using System.Diagnostics;

var connectionString = "mongodb://192.168.28.123:27017";

var settings = MongoClientSettings.FromConnectionString($"{connectionString}");

settings.ServerSelectionTimeout = TimeSpan.FromSeconds(1);

var dbClient = new MongoClient(settings);

var movies = dbClient?.GetDatabase("testdb1").GetCollection("movies");

var iiii = await movies.FindAsync("{ _id : '66d48adca720951e07550d78' }");

var j = iiii.FirstOrDefault();

//====================================




var client = new MongoClient(connectionString);

var db = MyDbContext.Create(client.GetDatabase("testdb1"));

db.Database.EnsureCreated();

//for (int i = 0; i < 10000; i++)
//{
//	var movie1 = new Movie()
//	{
//		Title = "Testbeeld " + i,
//		Rated = i.ToString(),
//		Plot = "this is movie " + i
//	};
//	db.Movies.Add(movie1);
//	var status1 = await db.SaveChangesAsync();
//}

//return;

await db.Movies.FirstOrDefaultAsync();

//await db.Movies.CountAsync();

var sw = Stopwatch.StartNew();

//var movie = await db.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.Title == "Back to the Future");

var movie = await db.Movies.FirstOrDefaultAsync(m => m._id == MongoDB.Bson.ObjectId.Parse("66d48adca720951e07550d78"));

Console.WriteLine($"{sw.ElapsedMilliseconds}mS");

if(movie == null)
{
	movie = new Movie()
	{
		Title = "Back to the Future",
		Rated = "9",
		Plot = "people in the past going to the future"
	};
	db.Movies.Add(movie);
	var status = await db.SaveChangesAsync();
}

Console.WriteLine(movie.Plot);
