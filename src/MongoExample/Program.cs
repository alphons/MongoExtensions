﻿
using MongoDB.Driver;
using MongoDb.Extensions;

using MongoExample;


var db = new CrmContext();


//var indexKeysDefinition = Builders<Klant>.IndexKeys.Ascending(x => x.Name);

//var indexName = await db.KlantTable.Indexes.CreateOneAsync(new CreateIndexModel<Klant>(indexKeysDefinition));

var klant = db.KlantTable.FirstOrDefault(x => x.Name == "Peter");

if(klant == null)
{
	klant = new Klant()
	{
		Name = "Peter",
		Address = "Street 11",
		Age = 35
	};

	await db.KlantTable.InsertOneAsync(klant);
}

Console.WriteLine($"Het {klant.Address}");


