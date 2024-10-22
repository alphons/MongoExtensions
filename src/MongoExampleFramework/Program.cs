using MongoDb.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoExampleFramework
{
	internal class Program
	{
		static void Main(string[] args)
		{
			var db = new CrmContext();

			var klant = db.KlantTable.FirstOrDefault(x => x.Name == "Peter");

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
