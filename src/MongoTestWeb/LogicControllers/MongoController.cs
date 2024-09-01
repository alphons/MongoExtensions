using Microsoft.AspNetCore.Mvc;

// nuget MongoDB.Driver
using MongoDB.Driver;
using BsonExtensions;


namespace MongoTestWeb.LogicControllers
{
	public class MongoController(IMongoClient mongo) : ControllerBase
	{

		[HttpGet("~/api/MongoClientSettings")]
		public async Task<IActionResult> MongoClientSettings()
		{
			await Task.Yield();

			var settings = mongo.Settings;

			return Ok(new
			{
				settings.HeartbeatInterval,
				settings.HeartbeatTimeout,
				settings.ConnectTimeout,
				Host = settings.Server.Host +":" + settings.Server.Port
			});
		}

		[HttpGet("~/api/ShowDatabases")]
		public async Task<IActionResult> ShowDatabases()
		{
			var List = await mongo.ListDatabaseNames().ToListAsync();

			return Ok(new
			{
				List
			});
		}

		[HttpPost("~/api/ShowCollections")]
		public async Task<IActionResult> ShowCollections(string DbName)
		{
			var List = await mongo.GetDatabase(DbName).ListCollectionNames().ToListAsync();

			return Ok(new
			{
				List
			});
		}

		[HttpPost("~/api/ShowCollection")]
		public async Task<IActionResult> ShowCollection(string DbName, string ColName)
		{
			var List = await mongo.GetDatabase(DbName).GetCollection(ColName).Find("{}").ToListAsync();

			return Ok(new
			{
				List
			});
		}

		[HttpPost("~/api/Test0")]
		public async Task<IActionResult> Test0()
		{
			var List = await mongo.GetDatabase("acme").GetCollection("Alphons").Find("{}").ToListAsync();

			//var json = MongoDB.Converters.BsonDocumentsConverter.ToJson(List, new System.Text.Json.JsonSerializerOptions() { WriteIndented = false });
			//System.IO.File.WriteAllText(@"d:\temp\json.json", json);

			return Ok(new
			{
				List
			});

		}


		[HttpPost("~/api/Test1")]
		public async Task<IActionResult> Test1()
		{
			var accounts = mongo.GetDatabase("test1").GetCollection("accounts");

			if (accounts == null)
				return NotFound();

			_ = await accounts.InsertOneAsync(@"
{
		'level': 1,
		'acct_id': 'abcAnnet',
		'cc':
		{
			'level': 5,
			'type': 'yy',
			'num': 0,
			'exp_date': '1-11-2021 00:00:00',
			'billing_addr':
			{
				'level': 5,
				'addr1': '123 ABC Street',
				'city': 'Some Dronten'
			},
			'shipping_addr':
			[
				{
					'level': 2,
					'addr1': '987 XYZ Ave',
					'city': 'Pandje Dronten'
				},
				{
					'level': 3,
					'addr1': 'PO Box 0123',
					'city': 'Beiersgulden Dronten'
				}
			]
		},
		'status': 'A'
	}");

			var List = await accounts.Find("{}").ToListAsync();

			//System.IO.File.WriteAllText(@"d:\temp\json.json", List.Pretty());


			//var json = Serializer.ToJson(List, new System.Text.Json.JsonSerializerOptions() { WriteIndented = true });
			//System.IO.File.WriteAllText(@"d:\temp\json.json", json);

			return Ok(new
			{
				List
			});

		}



		[HttpPost("~/api/Test3")]
		public async Task<IActionResult> Test3()
		{
			var armbanden = mongo.GetDatabase("Ibizanet").GetCollection("armbanden2");

			if (armbanden == null)
				return NotFound();


//			var result = await armbanden.InsertOneAsync(@"
//{
//	'Added': new Date(),
//	'Naam' : 'Kinder Armandje',
//    'Prijs' : 45.12,
//	'Status' : 'sale',
//	'Color'	: 'Brown'
//}");

			//			var result2 = await armbanden.InsertOneAsync(@"
			//{
			//	'Naam' : 'Mannen armband',
			//    'Prijs' : 33,
			//	'Status' : 'sale',
			//	'Color' : 'Paars'
			//}");


			var List = await armbanden.Find("{ 'Prijs' : { $gt: 45.1 }, 'Status' : 'sale' }").ToListAsync();

			//System.IO.File.WriteAllText(@"d:\temp\json.json", List.Pretty());

			return Ok(new
			{
				List
			});


		}


		[HttpPost("~/api/Test4")]
		public async Task<IActionResult> Test4()
		{
			var col = mongo.GetDatabase("sample_training").GetCollection("grades");
			var List = await col.Find("{}").ToListAsync();

			//System.IO.File.WriteAllText(@"d:\temp\json.json", List.Pretty());

			return Ok(new
			{
				List
			});
		}



		[HttpPost("~/api/Test5")]
		public async Task<IActionResult> Test5()
		{
			var List = await mongo.GetDatabase("acme").GetCollection("posts").Find("{}").ToListAsync();

			//System.IO.File.WriteAllText(@"d:\temp\json.json", List.Pretty());

			return Ok(new
			{
				List
			});
		}


		[HttpPost("~/api/Test6")]
		public async Task<IActionResult> Test6(int Page, string ColName, int SortDir)
		{
			var PageLength = 10;
			var skip = Page * PageLength;

			var col = mongo.GetDatabase("test").GetCollection("corona");

			if (col == null)
				return NotFound();

			var List = await col.Aggregate(@"
[
  {
   $lookup:
  {
      from: 'gemeenten',
      localField: 'gmcode',
      foreignField: 'gemcode',
      as: 'corona'
    }
  },
  {
	$unwind: '$corona'
  },
  {
    $project:
    {
	_id:0,
	infected_per_100k:1
	infected: 1,
	Name: '$corona.name',
    }
  },
  {
    $sort:
    {
      " + ColName + @": " + SortDir + @"
    }
  },
  {
    $skip: " + skip + @"
  },
  {
    $limit: "+ PageLength + @"
  }
]").ToListAsync();

			return Ok(new
			{
				List
			});
		}








	}
}
