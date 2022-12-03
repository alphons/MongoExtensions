using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Extensions;


namespace MongoTesting.ConsoleApp
{
	internal class OldStuff
	{
		private static readonly MongoClient dbClient = new("mongodb://192.168.28.210:27017");

		private static async Task TransactionTest()
		{
			if (dbClient != null)
			{
				using var iSession = await dbClient.StartSessionAsync();
				iSession.StartTransaction();
				try
				{
					// Deletes Updates Inserts
					await iSession.CommitTransactionAsync();
				}
				catch
				{
					await iSession.AbortTransactionAsync();
				}
			}
		}


		private static async Task Test0()
		{
			Console.WriteLine("The list of databases on this server is: ");
			Console.WriteLine(dbClient?.ListDatabases().ToList().Pretty());

			List<BsonDocument> result;

			var db = dbClient?.GetDatabase("acme");

			//await db.CreateCollectionAsync("Alphons");

			var alphons = db?.GetCollection("Alphons");

			//var objectid = await alphons.InsertOneAsync(new { Name = "Alphonsje 2", Age = 55 });

			Console.WriteLine(alphons?.Find("{}").ToList().Pretty());


			Console.WriteLine("The list of collections on database acme: ");
			Console.WriteLine(db?.ListCollections().Pretty());

			var collection = db?.GetCollection("posts");

			var col = dbClient?.GetDatabase("sample_training").GetCollection("grades");

			var goodscores = await col.Find(@"
				{
					scores: 
					{
						$elemMatch:
						{
							type: 'exam',
							score : { '$gte': 89 }
						}
					}
				}")
				.ToListAsync();

			Console.WriteLine(goodscores.Pretty());


			//var delResult11 = await col.DeleteOneAsync("{ 'student_id': 10001 }");

			var document = @"
			{
				'student_id': 10006,
				'scores': 
					[
						{ 'type': 'exam' , 'score': 89.12334193287023 } ,
						{ 'type': 'quiz' , 'score': 74.92381029342834 } ,
						{ 'type': 'homework' , 'score': 89.97929384290324 } ,
						{ 'type': 'homework' , 'score': 82.12931030513218 } 
					],
				'class_id': 480
			}";

			if (col != null)
			{
				var id = await col.InsertOneAsync(document);
				Console.WriteLine("Id = " + id);
			}

			var documents = @"[
			{
				'student_id': 10007,
				'scores': 
					[
						{ 'type': 'exam' , 'score': 88.12334193287023 } ,
						{ 'type': 'quiz' , 'score': 74.92381029342834 } ,
						{ 'type': 'homework' , 'score': 89.97929384290324 } ,
						{ 'type': 'homework' , 'score': 82.12931030513218 } 
					],
				'class_id': 480
			},
			{
				'student_id': 10008,
				'scores': 
					[
						{ 'type': 'exam' , 'score': 88.12334193287023 } ,
						{ 'type': 'quiz' , 'score': 74.92381029342834 } ,
						{ 'type': 'homework' , 'score': 89.97929384290324 } ,
						{ 'type': 'homework' , 'score': 82.12931030513218 } 
					],
				'class_id': 480
			}]";

			if (col != null)
			{
				var ids = await col.InsertManyAsync(documents);

				Console.WriteLine(string.Join(',', ids));
			}

			Console.Write(col.Find("{}").ToList().Pretty());


			var filter = Builders<BsonDocument>.Filter.Eq("student_id", 10000);
			var update = Builders<BsonDocument>.Update.Set("class_id", 483);

			var updResult = col?.UpdateOne(filter, update);

			var x = 486;
			if (col != null)
			{
				var updResult2 = await col.UpdateOneAsync("{ student_id : 10000 }", $"{{ $set: {{ class_id : {x} }} }}");
			}

			if (collection != null)
			{
				var count = await collection.CountDocumentsAsync("{}");
			}

			var aaaa = new { views = new { _gt = 2 } };

			var cc = await collection
				.Find("{ views: { $gt: 2 } }").CountDocumentsAsync();

			result = await collection
				.Find("{ views: { $gt: 2 } }")
				.Project("{ _id:0, title: 1, date: 1, views: 1 }")
				.Sort("{ _id: -1 }")
				.ToListAsync();

			Console.WriteLine(result.Pretty());

			result = await collection
				.Find(@"
				{
					comments: 
					{
						$elemMatch:
						{
							user: 'Mary Williams'
						}
					}
				}")
				.ToListAsync();

			Console.WriteLine(result.Pretty());

			if (collection != null)
			{
				var result3 = await collection.UpdateOneAsync("{ title: 'Post Two' }", @"
{
	$set: 
	{
		body: 'Body for post 2222',
		category: 'Technology'
	}
}");
				var mac = result3.MatchedCount;
				var moc = result3.ModifiedCount;
			}



			//var highExamScoreFilter = Builders<BsonDocument>.Filter.ElemMatch<BsonValue>(
			//		"scores", new BsonDocument { 
			//			{ "type", "exam" },
			//			{ "score", new BsonDocument { { "$gte", 88 } } } });

			//var highExamScores = collection.Find(highExamScoreFilter).ToList();
			//var cursor = collection.Find(highExamScoreFilter).ToCursor();
			//foreach (var document in cursor.ToEnumerable())
			//{
			//	Console.WriteLine(document);
			//}

			//await collection.Find(highExamScoreFilter).ForEachAsync(document => Console.WriteLine(document));

			//var sort = Builders<BsonDocument>.Sort.Descending("student_id");

			//var highestScores = collection.Find(highExamScoreFilter).Sort(sort);

			//var highestScore = collection.Find(highExamScoreFilter).Sort(sort).First();

			//Console.WriteLine(highestScore);


			//var filter = Builders<BsonDocument>.Filter.Eq("student_id", 10000);
			//var update = Builders<BsonDocument>.Update.Set("class_id", 483);

			//collection.UpdateOne(filter, update);

			//var arrayFilter = Builders<BsonDocument>.Filter.Eq("student_id", 10000)
			//				& Builders<BsonDocument>.Filter.Eq("scores.type", "quiz");

			//var arrayUpdate = Builders<BsonDocument>.Update.Set("scores.$.score", 84.92381029342834);

			//collection.UpdateOne(arrayFilter, arrayUpdate);

			//var deleteFilter = Builders<BsonDocument>.Filter.Eq("student_id", 10000);

			//collection.DeleteOne(deleteFilter);

			//var deleteLowExamFilter = Builders<BsonDocument>.Filter.ElemMatch<BsonValue>("scores",
			//		new BsonDocument { { "type", "exam" }, {"score", new BsonDocument { { "$lt", 60 }}}});

			//collection.DeleteMany(deleteLowExamFilter);

			//var document = new BsonDocument 
			//{ 
			//	{ 
			//		"student_id", 10000 
			//	}, 
			//	{
			//		"scores", new BsonArray
			//		{
			//			new BsonDocument { { "type", "exam" }, { "score", 88.12334193287023 } },
			//			new BsonDocument { { "type", "quiz" }, { "score", 74.92381029342834 } },
			//			new BsonDocument { { "type", "homework" }, { "score", 89.97929384290324 } },
			//			new BsonDocument { { "type", "homework" }, { "score", 82.12931030513218 } }
			//		}
			//	}, 
			//	{ 
			//		"class_id", 480 
			//	}
			//};
			//await collection.InsertOneAsync(document);

		}

		private static async Task Test1()
		{
			var forecast = @"
{
  _id: 2,
  title: '123 Department Report',
  tags: [ 'G', 'STLW' ],
  year: 2014,
  subsections: [
    {
      subtitle: 'Section 1: Overview',
      tags: [ 'SI', 'G' ],
      content:  'Section 1: This is the content of section 1.'
    },
    {
      subtitle: 'Section 2: Analysis',
      tags: [ 'STLW' ],
      content: 'Section 2: This is the content of section 2.'
    },
    {
      subtitle: 'Section 3: Budgeting',
      tags: [ 'TK' ],
      content: {
        text: 'Section 3: This is the content of section3.',
        tags: [ 'HCS' ]
      }
    }
  ]
}
";
			var forecasts = dbClient?.GetDatabase("test1").GetCollection("forecasts");

			if (forecasts == null)
				return;

			if (forecasts.CountDocuments() == 0)
			{
				var id = await forecasts.InsertOneAsync(forecast);
			}

			var userAccess = "[ 'STLW', 'G' ]";

			var result = await forecasts.AggregateAsync(@"
	[
     { $match: { year: 2014 } },
     { $redact: {
        $cond: {
           if: { $gt: [ { $size: { $setIntersection: [ '$tags', " + userAccess + @" ] } }, 0 ] },
           then: '$$DESCEND',
           else: '$$PRUNE'
         }
       }
     }
	]");

			Console.WriteLine(result.Pretty());
		}

		private static async Task Test2()
		{
			var accounts = dbClient?.GetDatabase("test1").GetCollection("accounts");

			if (accounts == null)
				return;

			//Console.WriteLine(accounts.Pretty());

			var account = @"
{
  _id: 3,
  level: 1,
  acct_id: 'xyz123',
  cc: {
    level: 5,
    type: 'yy',
    num: 0,
    exp_date: ISODate('2015-11-01T00:00:00.000Z'),
    billing_addr: {
      level: 5,
      addr1: '123 ABC Street',
      city: 'Some City'
    },
    shipping_addr: [
      {
        level: 6,
        addr1: '987 XYZ Ave',
        city: 'Some City'
      },
      {
        level: 5,
        addr1: 'PO Box 0123',
        city: 'Some City'
      }
    ]
  },
  status: 'A'
}";
			if (accounts.CountDocuments() == 2)
			{
				var id = await accounts.InsertOneAsync(account);
			}

			var result = await accounts.AggregateAsync(@"
[
    { $match: { status: 'A' } },
    {
      $redact: {
        $cond: {
          if: { $eq: [ '$level', 5 ] },
          then: '$$PRUNE',
          else: '$$DESCEND'
        }
      }
    }
  ]");
			Console.WriteLine(result.Pretty());

		}

		private static async Task Test3()
		{
			var characters = dbClient?.GetDatabase("test1").GetCollection("characters");

			if (characters == null)
				return;

			//var watchCursor = await characters.WatchAsync("[]", new ChangeStreamOptions() { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup });

			//Console.WriteLine(watchCursor.ToList().Pretty());

			var deletedDocs = await characters.DeleteManyAsync("{}");

			var rr = await characters.InsertManyAsync(@"[
				{ 'char' : 'Londen', 'class' : 'monk', 'lvl' : 4 },
				{ '_id' : 1, 'char' : 'Brisbane', 'class' : 'monk', 'lvl' : 4 },
				{ '_id' : 2, 'char' : 'Eldon', 'class' : 'alchemist', 'lvl' : 3 },
				{ '_id' : 3, 'char' : 'Meldane', 'class' : 'ranger', 'lvl' : 3 }]");

			var result = await characters.BulkWriteAsync(@"
[	
	{
        insertOne: {
            'document': {
                '_id': 4,
                'char': 'Dithras',
                'class': 'barbarian',
                'lvl': 4
            }
        }
    }, {
        insertOne: {
            'document': {
                '_id': 5,
                'char': 'Taeln',
                'class': 'fighter',
                'lvl': 3
            }
        }
    }, {
        updateOne: {
            'filter': {
                'char': 'Eldon'
            },
            'update': {
                $set: {
                    'status': 'Critical Injury'
                }
            }
        }
    }, {
        deleteOne: {
            'filter': {
                'char': 'Brisbane'
            }
        }
    }, {
        replaceOne: {
            'filter': {
                'char': 'Meldane'
            },
            'replacement': {
                'char': 'Tanys',
                'class': 'oracle',
                'lvl': 4
            }
        }
    }
]");



		}

		private static async Task Test4()
		{

			var orders = dbClient?.GetDatabase("test1").GetCollection("orders");

			if (orders == null)
				return;

			//await orders.DeleteManyAsync("{}");

			//			var rr = await orders.InsertManyAsync(@"[
			//   { _id: 0, name: 'Pepperoni', size: 'small', price: 19,
			//     quantity: 10, date: ISODate( '2021-03-13T08:14:30Z' ) },
			//   { _id: 1, name: 'Pepperoni', size: 'medium', price: 20,
			//     quantity: 20, date : ISODate( '2021-03-13T09:13:24Z' ) },
			//   { _id: 2, name: 'Pepperoni', size: 'large', price: 21,
			//     quantity: 30, date : ISODate( '2021-03-17T09:22:12Z' ) },
			//   { _id: 3, name: 'Cheese', size: 'small', price: 12,
			//     quantity: 15, date : ISODate( '2021-03-13T11:21:39.736Z' ) },
			//   { _id: 4, name: 'Cheese', size: 'medium', price: 13,
			//     quantity:50, date : ISODate( '2022-01-12T21:23:13.331Z' ) },
			//   { _id: 5, name: 'Cheese', size: 'large', price: 14,
			//     quantity: 10, date : ISODate( '2022-01-12T05:08:13Z' ) },
			//   { _id: 6, name: 'Vegan', size: 'small', price: 17,
			//     quantity: 10, date : ISODate( '2021-01-13T05:08:13Z' ) },
			//   { _id: 7, name: 'Vegan', size: 'medium', price: 18,
			//     quantity: 10, date : ISODate( '2021-01-13T05:10:13Z' ) }
			//]");



			var list = await orders.AggregateAsync(@"[
   {
      $match: { size: 'medium' }
   },
   {
      $group: { _id: '$name', totalQuantity: { $sum: '$quantity' } }
   }
]");
			Console.WriteLine(list.Pretty());


			var list2 = await orders.AggregateAsync(@"[
   {
      $match:
      {
         'date': { $gte: new ISODate( '2020-01-30' ), $lt: new ISODate( '2022-01-30' ) }
      }
   },
   {
      $group:
      {
         _id: { $dateToString: { format: '%Y-%m-%d', date: '$date' } },
         totalOrderValue: { $sum: { $multiply: [ '$price', '$quantity' ] } },
         averageOrderQuantity: { $avg: '$quantity' }
      }
   },
   {
      $sort: { totalOrderValue: -1 }
   }
 ]");

			Console.WriteLine(list2.Pretty());

		}

	}
}
