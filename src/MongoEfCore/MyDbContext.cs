using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;


namespace MongoEfCore;

public class MyDbContext : DbContext
{
	public DbSet<Movie> Movies { get; init; }
	public static MyDbContext Create(IMongoDatabase database) =>
		new(new DbContextOptionsBuilder<MyDbContext>()
			.UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
			.Options);
	public MyDbContext(DbContextOptions options) : base(options)
	{
	}
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
		modelBuilder.Entity<Movie>().ToCollection("movies");
	}
}
public class Movie
{
	[BsonId]
	public ObjectId _id { get; set; }
	[BsonElement("title")]
	public string Title { get; set; }
	[BsonElement("rated")]
	public string Rated { get; set; }
	[BsonElement("plot")]
	public string Plot { get; set; }
}
