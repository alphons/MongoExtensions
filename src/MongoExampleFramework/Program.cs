using System;
using System.Threading.Tasks;

using MongoDb.Extensions;

namespace MongoExampleFramework
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			var db = new CrmContext();

			var klant = await db.KlantTable.FirstOrDefaultAsync(x => x.Name == "Peter");

			if (klant == null)
			{
				klant = new Klant()
				{
					Name = "Peter",
					Address = "Street 11",
					Age = 35
				};

				db.KlantTable.InsertOne(klant);
			}

			Console.WriteLine($"Het {klant.Address}");
		}
	}
}
