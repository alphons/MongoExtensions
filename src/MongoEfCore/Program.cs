
using MongoDB.Driver;

using MongoDB.Bson;

using System.Diagnostics;

using MongoEfCore;

var connectionString = "mongodb://192.168.28.123:27017";


var sw1 = Stopwatch.StartNew();

//var settings = MongoClientSettings.FromConnectionString($"{connectionString}");

//settings.ServerSelectionTimeout = TimeSpan.FromSeconds(1);

var dbClient = new MongoClient(connectionString);

if (dbClient == null)
	return;

var movies = dbClient.GetDatabase("testdb3").GetCollection<Movie>("cmovies");

if (movies == null)
	return;

//var indexKeysDefinition = Builders<Movie>.IndexKeys.Ascending(x => x.Title);
//await movies.Indexes.CreateOneAsync(new CreateIndexModel<Movie>(indexKeysDefinition));


//for (int i = 0; i < 10000; i++)
//{
//	await movies.InsertOneAsync(new Movie()
//	{
//		Title = $"This is movie {i}",
//		Plot = "This plot is unknown",
//		Rated = "1"
//	});
//}


//var mmm = new Movie()
//{
//	Title = "Unknown",
//	Plot = "This plot is unknown",
//	Rated = "1"
//};

//await movies.InsertOneAsync(mmm);


movies.FirstOrDefault(x => x.Id == ObjectId.Parse("66d5653d643b92e1eb73b087"));

sw1.Restart();

var aaa1 = movies.FirstOrDefault(x => x.Id == ObjectId.Parse("66d5653d643b92e1eb73b087"));

Console.WriteLine($"{sw1.ElapsedMilliseconds}mS");

sw1.Restart();

var aaa2 = await movies.Where(x => x.Id == ObjectId.Parse("66d5653d643b92e1eb73b087")).FirstOrDefaultAsync();

Console.WriteLine($"{sw1.ElapsedMilliseconds}mS");

sw1.Restart();

var aaa3 = movies.Where(x => x.Id == ObjectId.Parse("66d5653d643b92e1eb73b087")).FirstOrDefault();

Console.WriteLine($"Sync {sw1.ElapsedMilliseconds}mS");

var aaa4 = await movies.FirsOrDefaultAsync(x => x.Id == ObjectId.Parse("66d5653d643b92e1eb73b087"));

Console.WriteLine($"{sw1.ElapsedMilliseconds}mS");

sw1.Restart();

var aaa5 = movies.FirstOrDefault(x => x.Title == "This is movie 5432");

Console.WriteLine($"{sw1.ElapsedMilliseconds}mS");

sw1.Restart();

var a = await movies.CountDocumentsAsync(x => x.Title == "Unknown");

Console.WriteLine($"{sw1.ElapsedMilliseconds}mS");


//for (int i = 0; i < 1000; i++)
//{
//	await movies.FirsOrDefaultAsync(x => x.Id == ObjectId.Parse("66d5653d643b92e1eb73b087"));
//}


var movie1 = await movies.FirsOrDefaultAsync(x => x.Id == ObjectId.Parse("66d5653d643b92e1eb73b087"));

if(movie1 != null)
	Console.WriteLine($"{sw1.ElapsedMilliseconds}mS {movie1.Title}");




class Movie
{
	public ObjectId Id { get; set; }

	public string? Title { get; set; }

	public string? Rated { get; set; }

	public string? Plot { get; set; }
}
