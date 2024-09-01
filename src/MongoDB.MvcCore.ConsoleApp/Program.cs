
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Reflection;

using MongoDB.Bson;
using MongoDB.Driver;

using InteractiveReadLine.KeyBehaviors;
using InteractiveReadLine;
using InteractiveReadLine.Tokenizing;

namespace MongoDB.MvcCore.ConsoleApp;

class Program
{
	private static readonly string DEFAULTPORT = "27017";
	private static readonly string CONNECTIONADDRESS = "127.0.0.1";

	private static MongoClient client;

	private static string DbName = string.Empty;
	private static string ColName = string.Empty;

	private static List<BsonDocument> result = null;

	private static StringBuilder sb = new();

	private static readonly ConsoleColor restore = Console.ForegroundColor;

	private static string CurrentConnection = string.Empty;

	private static BsonJsonSerializer.TypeSerializationEnum typeSerializationEnum = BsonJsonSerializer.TypeSerializationEnum.Colorize;

	private static readonly string[] autoCompleteWords = {
"ls",
"rename",
"import",
"export",
"show",
"find",
"aggregate",

"insertone",
"insertmany",
"insert",

"use",
"updateone",
"updatemany",
"update",

"deleteone",
"deletemany",
"delete",
"drop",
"dropdatabase",

"cd",
"cls",
"count",
"command",
"connect",
"createindex",
"countdocuments" };


	private static List<string> history = new();

	private static string[] AutoComplete(TokenizedLine line)
	{
		if (string.IsNullOrWhiteSpace(DbName))
		{
			var databases = client
				.ListDatabases()
				.ToList()
				.Select(x => x["name"].ToString())
				.Where(s => s.StartsWith(line.CursorToken.Text))
				.ToArray();
			return databases;
		}
		if (string.IsNullOrWhiteSpace(ColName))
		{
			var collections = client.GetDatabase(DbName)
				.ListCollectionNames()
				.ToList()
				.Where(s => s.StartsWith(line.CursorToken.Text))
				.ToArray();
			return collections;
		}

		return autoCompleteWords
			.Where(s => s.StartsWith(line.CursorToken.Text))
			.ToArray();
	}

	public class Demo
	{
		public DateTime Datum { get; set; }

		public static async Task Main(String[] args)
		{
			//var demo = new Demo() { Datum = DateTime.Now };
			//var a = BsonSerializer.Serialize(demo);
			//var b = BsonDocument.Parse(a);


			var prompt = MakePrompt();

			var version = $"{Assembly.GetExecutingAssembly().GetName().Version}";

			var serVersion = $"{typeof(BsonJsonSerializer).Assembly.GetName().Version}";

			ColorOutput($"\u0084MongoDB CSharp Shell version:\u0080 {version}{Environment.NewLine}");
			ColorOutput($"\u0084MongoDB.MvcCore.BsonJsonSerializer version:\u0080 {serVersion}{Environment.NewLine}");

			try
			{
				if (args.Length > 0)
					await ConnectAsync(args[0]);
				else
					await ConnectAsync(CONNECTIONADDRESS);
			}
			catch (TimeoutException)
			{
				ColorException($"\u0088Timeout\u0080, use connect(ipnr[:portnr]) to change connection");
			}
			catch (Exception eee)
			{
				ColorException(eee.Message);
				return;
			}

			Console.WriteLine();

			var config = ReadLineConfig.Basic
				.AddCtrlNavKeys()
				.AddTabAutoComplete()
				.SetLexer(CommonLexers.SplitOnWhitespace)
				.SetAutoCompletion(AutoComplete);

			config.SetHistorySource(history);

			while (true)
			{
				ColorOutput(prompt);

				var line = ConsoleReadLine.ReadLine(config);
				if (line == "exit" || line == null)
					break;

				if (!string.IsNullOrWhiteSpace(line))
					history.Add(line);

				if (line == "....")
				{
					sb = new();
					continue;
				}

				sb.AppendLine(line);

				var parsed = ParseHelper.ParseFunction(sb.ToString().Trim());

				if (parsed.Complete)
				{
					try
					{
						await ExecParsedAsync(parsed);
					}
					catch (TimeoutException)
					{
						ColorException("TimeOut");
					}
					catch (Exception eee)
					{
						ColorException(eee.Message);
					}
					if (result != null)
						ColorOutput(result.Pretty(typeSerializationEnum));
					result = null;
					sb = new StringBuilder();

					prompt = MakePrompt();
				}
				else
				{
					prompt = "... ";
				}
			}

			Console.WriteLine();
			Console.WriteLine("exiting, bye!");
		}

		private static string MakePrompt()
		{
			var prompt = $"\u0085{DbName}\u0080.\u0086{ColName}\u0080>";
			if (string.IsNullOrWhiteSpace(ColName))
				prompt = $"\u0085{DbName}\u0080>";
			return prompt;
		}

		private static void ColorOutput(string input, bool Restore = true)
		{
			var pattern = "([\u0080\u0081\u0082\u0083\u0084\u0085\u0086\u0087\u0088\u0089\u008a])";

			var substrings = Regex.Split(input, pattern);
			foreach (var match in substrings)
			{
				switch (match)
				{
					default:
						Console.Write(match);
						break;
					case "\u0080":
						Console.ForegroundColor = restore;
						break;
					case "\u0081":
						Console.ForegroundColor = ConsoleColor.Green;
						break;
					case "\u0082":
						Console.ForegroundColor = ConsoleColor.DarkYellow;
						break;
					case "\u0083":
						Console.ForegroundColor = ConsoleColor.DarkRed;
						break;
					case "\u0084":
						Console.ForegroundColor = ConsoleColor.DarkGray;
						break;
					case "\u0085":
						Console.ForegroundColor = ConsoleColor.Cyan; // Dbs
						break;
					case "\u0086":
						Console.ForegroundColor = ConsoleColor.DarkCyan; // Collections
						break;
					case "\u0087":
						Console.ForegroundColor = ConsoleColor.Yellow;
						break;
					case "\u0088":
						Console.ForegroundColor = ConsoleColor.Red;
						break;
					case "\u0089":
						Console.ForegroundColor = ConsoleColor.Magenta;
						break;
					case "\u008a":
						Console.ForegroundColor = ConsoleColor.DarkMagenta;
						break;
				}
			}
			if (Restore)
				Console.ForegroundColor = restore;
		}

		private static void ColorUsage(string s)
		{
			ColorOutput($"\u0084usage:\u0080 {s}{Environment.NewLine}");
		}

		private static void ColorException(string s)
		{
			ColorOutput($"\u0088Exception:\u0080 {s}{Environment.NewLine}");
		}

		private async static Task ConnectAsync(string connectionstring)
		{
			if (!connectionstring.Contains(':'))
				connectionstring += $":{DEFAULTPORT}";
			CurrentConnection = connectionstring;
			var settings = MongoClientSettings.FromConnectionString($"mongodb://{CurrentConnection}");

			settings.ServerSelectionTimeout = TimeSpan.FromSeconds(1);

			ColorOutput($"\u0084connecting to:\u0080 {CurrentConnection}{Environment.NewLine}");

			client = new(settings);

			var buildInfo = await client.GetDatabase("local").RunCommandAsync("{ buildInfo : 1 }");
			ColorOutput($"\u0084MongoDB server version:\u0080 {buildInfo["version"]} \u0084(git {buildInfo["gitVersion"]}\u0080){Environment.NewLine}");
		}

		private static void Color(BsonValue val)
		{
			ColorOutput(BsonJsonSerializer.ToJson(0, val, new JsonSerializerOptions() { WriteIndented = true }, typeSerializationEnum) + Environment.NewLine);
		}

		private static async Task ExecParsedAsync(FunctionArguments parsed)
		{
			var args = parsed.Arguments.Count == 0 ? string.Empty : parsed.Arguments[0];

			switch (parsed.Name.ToLower())
			{
				case "":
					break;
				case "color":
					if (string.IsNullOrWhiteSpace(args))
						ColorUsage($"{parsed.Name} [on|off]");
					if (args == "on" || args == "1" || args == "true" || args == "yes")
						typeSerializationEnum = BsonJsonSerializer.TypeSerializationEnum.Colorize;
					else
						typeSerializationEnum = BsonJsonSerializer.TypeSerializationEnum.None;
					break;
				case "cls":
					Console.Clear();
					break;
				case "rename":
					if (string.IsNullOrWhiteSpace(args) || parsed.Arguments.Count != 2)
					{
						ColorUsage($"{parsed.Name} <oldcollectionname> <newcollectionname>");
						break;
					}
					await client.GetDatabase(DbName).RenameCollectionAsync(parsed.Arguments[0], parsed.Arguments[1]);
					ColorOutput($"renamed collection \u0086{parsed.Arguments[0]}\u0080 to new name \u0086{parsed.Arguments[1]}\u0080{Environment.NewLine}");
					break;
				case "q":
					if (string.IsNullOrEmpty(args))
					{
						result = await client.GetDatabase(DbName).GetCollection("q")
							.Aggregate("[{'$unwind':'$name'},{'$group':{'_id':null,queries:{$push:'$name'}}},{'$project':{queries:true,_id:false}}]")
							.ToListAsync();
						break;
					}
					var qDoc = await client.GetDatabase(DbName).GetCollection("q").Find($"{{ name: '{args}'}}").FirstOrDefaultAsync();
					if (qDoc == null)
					{
						Console.WriteLine($"Query {args} not found");
					}
					else
					{
						var dbName = qDoc.Contains("DbName") ? qDoc["DbName"].ToString() : DbName;
						var colName = qDoc.Contains("ColName") ? qDoc["ColName"].ToString() : ColName;
						var query = qDoc.Contains("query") ? qDoc["query"].ToString() : "{}";
						result = await client.GetDatabase(dbName).GetCollection(colName).Aggregate(query).ToListAsync();
					}
					break;
				case "import":
					await ImportJson(args);
					break;
				case "export":
					var list = await client.GetDatabase(DbName).GetCollection(ColName).Find("{}").ToListAsync();
					await File.WriteAllTextAsync(args, list.ToJson(new MongoDB.Bson.IO.JsonWriterSettings() { Indent = true }));
					Console.WriteLine($"Exported {list.Count} documents to {args}");
					break;
				case "use":
					if (string.IsNullOrWhiteSpace(args))
						ColorUsage("use <databasename>[.<collectionname>]");
					else
					{
						var point = args.IndexOf('.');
						if (point < 0)
						{
							DbName = args;
							ColName = string.Empty;
							Console.WriteLine($"switched to db {DbName}");
						}
						else
						{
							DbName = args[0..point].Trim();
							ColName = args[(point + 1)..].Trim();
							Console.WriteLine($"switched to db {DbName}.{ColName}");
						}
					}
					break;
				case "show":
					if (string.IsNullOrWhiteSpace(args))
						ColorUsage("show [dbs|databases|collections]");
					else
						await CommandShowAsync(args, string.Empty);
					break;
				case "ls":
					if (string.IsNullOrWhiteSpace(DbName))
						await CommandShowAsync("databases", args);
					else if (string.IsNullOrWhiteSpace(ColName))
						await CommandShowAsync("collections", args);
					else
					{
						if (parsed.IsFunction == false)
							args = string.Join(' ', parsed.Arguments).Trim();
						if (string.IsNullOrWhiteSpace(args) || args == "-al" || args == "-l")
							args = "{}";
						else if (args == "1")
						{
							args = "[ { $limit: 1 } ]";
							result = await client.GetDatabase(DbName).GetCollection(ColName).Aggregate(args).ToListAsync();
							break;
						}
						result = await client.GetDatabase(DbName).GetCollection(ColName).Find(args).ToListAsync();
					}
					break;
				case "cd":
					if (!string.IsNullOrWhiteSpace(args))
					{
						if (args == ".")
							break;
						if (args == "..")
						{
							if (!string.IsNullOrWhiteSpace(ColName))
								ColName = string.Empty;
							else
							{
								if (!string.IsNullOrWhiteSpace(DbName))
								{
									ColName = string.Empty;
									DbName = string.Empty;
								}
							}
						}
						else
						{
							if (args.StartsWith('/'))
							{
								var slash = args.IndexOf('/', 1);
								var dot = args.IndexOf('.', 1);
								if (slash < 0 && dot < 0)
								{
									DbName = args[1..].Trim();
									ColName = String.Empty;
								}
								else
								{
									if (slash < 0 && dot >= 0)
										slash = dot;
									DbName = args[1..slash].Trim();
									ColName = args[(slash + 1)..].Trim();
								}
							}
							else
							{
								if (string.IsNullOrWhiteSpace(DbName))
								{
									var slash = args.IndexOf('/');
									var dot = args.IndexOf('.');
									if (slash < 0 && dot < 0)
									{
										DbName = args.Trim();
										ColName = String.Empty;
									}
									else
									{
										if (slash < 0 && dot >= 0)
											slash = dot;
										DbName = args[0..slash].Trim();
										ColName = args[(slash + 1)..].Trim();
									}
								}
								else
								{
									ColName = args;
								}
							}
						}
					}
					break;
				// -----------------------------------------
				default:
					Console.WriteLine($"Unknown keyword: {parsed.Name}");
					break;
				case "command":
					if (parsed.IsFunction == false)
						args = string.Join(' ', parsed.Arguments).Trim();
					if (string.IsNullOrWhiteSpace(args))
						args = "{}";
					BsonDocument valCommand = await client.GetDatabase(DbName).RunCommandAsync(args);
					Color(valCommand);
					break;
				case "connect":
					if (parsed.IsFunction == false)
						args = string.Join(' ', parsed.Arguments).Trim();
					if (string.IsNullOrWhiteSpace(args))
					{
						ColorOutput($"\u0084Current connection string:\u0080 {CurrentConnection}{Environment.NewLine}");
						ColorOutput($"\u0084usage:\u0080 {parsed.Name}(ipnumber[:portnumber])\u0084 to change connection\u0080{Environment.NewLine}");
					}
					else
					{
						await ConnectAsync($"{args}");
						DbName = string.Empty;
						ColName = string.Empty;
					}
					break;
				case "find":
					if (parsed.IsFunction == false)
						args = string.Join(' ', parsed.Arguments).Trim();
					if (string.IsNullOrWhiteSpace(args))
						args = "{}";
					if (parsed.Arguments.Count == 2)
						result = await client.GetDatabase(DbName).GetCollection(ColName).Find(parsed.Arguments[0]).Project(parsed.Arguments[1]).ToListAsync();
					else
						result = await client.GetDatabase(DbName).GetCollection(ColName).Find(args).ToListAsync();
					break;
				case "aggregate":
					if (parsed.IsFunction == false)
						args = string.Join(' ', parsed.Arguments).Trim();
					if (string.IsNullOrWhiteSpace(args) || args[0] != '[')
						ColorUsage($"{parsed.Name}( [, , ])");
					else
						result = await client.GetDatabase(DbName).GetCollection(ColName).Aggregate(args).ToListAsync();
					break;
				case "insertone":
					if (parsed.IsFunction == false)
						args = string.Join(' ', parsed.Arguments).Trim();
					if (string.IsNullOrWhiteSpace(args))
						args = "{}";
					BsonValue valInsertOne = await client.GetDatabase(DbName).GetCollection(ColName).InsertOneAsync(args);
					Color(valInsertOne);
					break;
				case "insertmany":
					if (parsed.IsFunction == false)
						args = string.Join(' ', parsed.Arguments).Trim();
					if (string.IsNullOrWhiteSpace(args) || args[0] != '[')
						ColorUsage($"{parsed.Name}( [, , ])");
					else
					{
						List<BsonValue> valInsertMany = await client.GetDatabase(DbName).GetCollection(ColName).InsertManyAsync(args);
						valInsertMany.ForEach(x => Color(x));
					}
					break;
				case "insert":
					if (parsed.IsFunction == false)
						args = string.Join(' ', parsed.Arguments).Trim();
					if (string.IsNullOrWhiteSpace(args))
						args = "{}";
					if (args.Trim().StartsWith('{'))
					{
						BsonValue valInsertOne1 = await client.GetDatabase(DbName).GetCollection(ColName).InsertOneAsync(args);
						Color(valInsertOne1);
					}
					else
					{
						List<BsonValue> valInsertMany = await client.GetDatabase(DbName).GetCollection(ColName).InsertManyAsync(args);
						valInsertMany.ForEach(x => Color(x));
					}
					break;
				case "updateone":
					UpdateResult updateOneResult = await client.GetDatabase(DbName).GetCollection(ColName).UpdateOneAsync(parsed.Arguments[0], parsed.Arguments[1]);
					ColorOutput($"\u0084updated documents:\u0080 {updateOneResult.ModifiedCount} \u0084(IsAcknowledged:{updateOneResult.IsAcknowledged}):\u0080{Environment.NewLine}");
					break;
				case "updatemany":
				case "update":
					UpdateResult updateManyResult = await client.GetDatabase(DbName).GetCollection(ColName).UpdateManyAsync(parsed.Arguments[0], parsed.Arguments[1]);
					ColorOutput($"\u0084updated documents:\u0080 {updateManyResult.ModifiedCount} \u0084(IsAcknowledged:{updateManyResult.IsAcknowledged}):\u0080{Environment.NewLine}");
					break;
				case "deleteone":
					if (parsed.IsFunction == false)
						args = string.Join(' ', parsed.Arguments).Trim();
					if (string.IsNullOrWhiteSpace(args))
						ColorUsage($"{parsed.Name}(<filter>)");
					else
					{
						DeleteResult valDeleteOne = await client.GetDatabase(DbName).GetCollection(ColName).DeleteOneAsync(args);
						ColorOutput($"\u0084deleted documents:\u0080 {valDeleteOne.DeletedCount} \u0084(IsAcknowledged:{valDeleteOne.IsAcknowledged}):\u0080{Environment.NewLine}");
					}
					break;
				case "deletemany":
				case "delete":
					if (parsed.IsFunction == false)
						args = string.Join(' ', parsed.Arguments).Trim();
					if (string.IsNullOrWhiteSpace(args))
						ColorUsage($"{parsed.Name}(<filter>)");
					else
					{
						DeleteResult valDeleteMany = await client.GetDatabase(DbName).GetCollection(ColName).DeleteManyAsync(args);

						ColorOutput($"\u0084deleted documents:\u0080 {valDeleteMany.DeletedCount} \u0084(IsAcknowledged:{valDeleteMany.IsAcknowledged}):\u0080{Environment.NewLine}");
					}
					break;
				case "drop":
					if (parsed.IsFunction == false)
						args = string.Join(' ', parsed.Arguments).Trim();
					if (string.IsNullOrWhiteSpace(args))
						ColorUsage($"{parsed.Name}(<collection>)");
					else
					{
						await client.GetDatabase(DbName).DropCollectionAsync(args);
						Console.WriteLine($"Collection {parsed.Arguments[0]} dropped");
					}
					break;
				case "dropdatabase":
					if (parsed.IsFunction == false)
						args = string.Join(' ', parsed.Arguments).Trim();
					if (string.IsNullOrWhiteSpace(args))
						ColorUsage($"{parsed.Name}(<dbname>)");
					else
					{
						await client.DropDatabaseAsync(args);
						Console.WriteLine($"Database {args} dropped");
						if (DbName == args)
						{
							DbName = string.Empty;
							ColName = string.Empty;
						}
					}
					break;
				case "createindex":
					if (parsed.IsFunction == false)
						args = string.Join(' ', parsed.Arguments).Trim();
					var indexResult = await client.GetDatabase(DbName).GetCollection(ColName).Indexes.CreateOneAsync(new CreateIndexModel<BsonDocument>(args));
					Console.WriteLine($"Index {indexResult} created");
					break;
				case "count":
				case "countdocuments":
					if (parsed.IsFunction == false)
						args = string.Join(' ', parsed.Arguments).Trim();
					if (string.IsNullOrWhiteSpace(args))
					{
						var lngCount = await client.GetDatabase(DbName).GetCollection(ColName).CountDocumentsAsync();
						Console.WriteLine(lngCount);
					}
					else
					{
						var lngCount = await client.GetDatabase(DbName).GetCollection(ColName).CountDocumentsAsync(args);
						Console.WriteLine(lngCount);
					}
					break;


			}
		}


		private static async Task ImportJson(string FileName)
		{
			using var session = await client.StartSessionAsync();

			try
			{
				session.StartTransaction();
			}
			catch
			{
				await client.GetDatabase(DbName).DropCollectionAsync(ColName);

				var jsonDocuments = await File.ReadAllTextAsync(FileName);

				var insertedIds = await client.GetDatabase(DbName).GetCollection(ColName).InsertManyAsync(jsonDocuments);

				Console.WriteLine($"Imported {insertedIds.Count} documents from {FileName} (non-transaction)");

				return;
			}

			try
			{
				await client.GetDatabase(DbName).DropCollectionAsync(ColName);

				var jsonDocuments = await File.ReadAllTextAsync(FileName);

				var insertedIds = await client.GetDatabase(DbName).GetCollection(ColName).InsertManyAsync(jsonDocuments);

				await session.CommitTransactionAsync();

				Console.WriteLine($"Imported {insertedIds.Count} documents from {FileName} (transaction)");
			}
			catch (Exception eee)
			{
				await session.AbortTransactionAsync();
				ColorException($"{eee.Message}");
			}
		}

		private static async Task CommandShowAsync(string line, string args)
		{
			switch (line)
			{
				default:
					Console.WriteLine($"show what?:{line}");
					break;
				case "dbs":
				case "databases":
					var list = client.ListDatabases().ToList();
					if (string.IsNullOrWhiteSpace(args) || !args.Contains('-'))
					{
						foreach (BsonDocument doc in list)
							ColorOutput($"\u0085{doc["name"]}{Environment.NewLine}");
					}
					else
					{
						foreach (BsonDocument doc in list)
							ColorOutput($"\u0085{doc["name"],-20}\u0080 {doc["sizeOnDisk"],15}{Environment.NewLine}");
					}
					break;
				case "collections":
					var names = await client.GetDatabase(DbName).ListCollectionNamesAsync();
					if (string.IsNullOrWhiteSpace(args) || !args.Contains('-'))
					{
						ColorOutput("\u0086", false);
						await names.ForEachAsync(x => Console.WriteLine(x));
						ColorOutput("\u0080");
					}
					else
					{
						var db = client.GetDatabase(DbName);
						await names.ForEachAsync(async x => ColorOutput($"\u0086{x,-30}\u0080 {await db.GetCollection(x).EstimatedDocumentCountAsync(),15}{Environment.NewLine}"));
					}
					break;
			}
		}



	}
}


