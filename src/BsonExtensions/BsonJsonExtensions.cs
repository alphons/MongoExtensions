﻿
using System.Text.Json;


using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;

using BsonExtensions.Converters;
using System.Linq.Expressions;


namespace BsonExtensions;
public static class BsonJsonExtensions
{
	/// <summary>
	/// Add BsonDocumentJsonConverter and BsonDocumentsJsonConverter to IMvcCoreBuilder
	/// </summary>
	/// <param name="builder"></param>
	/// <returns></returns>
	public static IMvcCoreBuilder AddBsonJsonConverters(this IMvcCoreBuilder builder)
	{
		return builder.AddJsonOptions(options =>
		{
			options.JsonSerializerOptions.Converters.Add(new BsonDocumentJsonConverter());
			options.JsonSerializerOptions.Converters.Add(new BsonDocumentsJsonConverter());

			options.JsonSerializerOptions.DictionaryKeyPolicy = null;
			options.JsonSerializerOptions.PropertyNamingPolicy = null;
		});
	}

	public async static Task<BsonDocument> RunCommandAsync(this IMongoDatabase db, string JsonDocument, ReadPreference? readPreference = null, CancellationToken cancellationToken = default)
	{
		return await db.RunCommandAsync<BsonDocument>(BsonDocument.Parse(JsonDocument), readPreference, cancellationToken);
	}

	public async static Task<BsonValue> InsertOneAsync(this IMongoCollection<BsonDocument> collection, string Json, InsertOneOptions? options = null, CancellationToken cancellationToken = default)
	{
		var doc = BsonDocument.Parse(Json);
		await collection.InsertOneAsync(doc, options, cancellationToken);
		return doc["_id"];
	}

	public async static Task<BsonValue> InsertOneAsync(this IMongoCollection<BsonDocument> collection, object objA, InsertOneOptions? options = null, CancellationToken cancellationToken = default)
	{
		var doc = BsonJsonSerializer.ToBsonDocument(objA);
		await collection.InsertOneAsync(doc, options, cancellationToken);
		return doc["_id"];
	}

	public async static Task<List<BsonValue>> InsertManyAsync(this IMongoCollection<BsonDocument> collection, string JsonDocuments, InsertManyOptions? options = null, CancellationToken cancellationToken = default)
	{
		var bsonDocuments = BsonSerializer.Deserialize<BsonDocument[]>(JsonDocuments);
		await collection.InsertManyAsync(bsonDocuments, options, cancellationToken);
		return bsonDocuments.Select(x => x["_id"]).ToList();
	}

	public async static Task<ReplaceOneResult> ReplaceOneByIdAsync(this IMongoCollection<BsonDocument> collection, object objA, CancellationToken cancellationToken = default)
	{
		var doc = BsonJsonSerializer.ToBsonDocument(objA);
		var id = doc["_id"];
		string filter;
		if (id.IsGuid)
			filter = $"{{ _id : UUID('{id.AsGuid}') }}";
		else if (id.IsObjectId)
			filter = $"{{ _id : ObjectId('{id.AsObjectId}') }}";
		else if (id.IsString)
			filter = $"{{ _id : '{id.AsString}' }}";
		else
			filter = $"{{ _id : {id.AsInt64} }}"; // some kind of int
		return await collection.ReplaceOneAsync(filter, doc, new ReplaceOptions() { IsUpsert = true }, cancellationToken);
	}

	public async static Task<UpdateResult> UpdateOneAsync<T>(this IMongoCollection<T> collection, string Json, T? objA, UpdateOptions? updateOptions = null, CancellationToken cancellationToken = default)
	{
		if (objA == null)
			throw new Exception("update object is null");
		var doc = BsonJsonSerializer.ToBsonDocument(objA);
		var id = doc["_id"];
		string filter;
		if (id.IsGuid)
			filter = $"{{ _id : UUID('{id.AsGuid}') }}";
		else if (id.IsObjectId)
			filter = $"{{ _id : ObjectId('{id.AsObjectId}') }}";
		else if (id.IsString)
			filter = $"{{ _id : '{id.AsString}' }}";
		else
			filter = $"{{ _id : {id.AsInt64} }}"; // some kind of int

		return await collection.UpdateOneAsync(filter, BsonJsonSerializer.ToBsonDocument(objA), updateOptions, cancellationToken);
	}

	public async static Task<DeleteResult> DeleteOneAsync(this IMongoCollection<BsonDocument> collection, string Json, CancellationToken cancellationToken = default)
	{
		return await collection.DeleteOneAsync(BsonDocument.Parse(Json), cancellationToken);
	}

	public async static Task<DeleteResult> DeleteOneByIdAsync(this IMongoCollection<BsonDocument> collection, object objA, CancellationToken cancellationToken = default)
	{
		var doc = BsonJsonSerializer.ToBsonDocument(objA);
		var id = doc["_id"];
		string filter;
		if (id.IsGuid)
			filter = $"{{ _id : UUID('{id.AsGuid}') }}";
		else if (id.IsObjectId)
			filter = $"{{ _id : ObjectId('{id.AsObjectId}') }}";
		else if (id.IsString)
			filter = $"{{ _id : '{id.AsString}' }}";
		else
			filter = $"{{ _id : {id.AsInt64} }}"; // some kind of int

		return await collection.DeleteOneAsync(filter, cancellationToken);
	}

	public async static Task<DeleteResult> DeleteOneAsync(this IMongoCollection<BsonDocument> collection, object objA, CancellationToken cancellationToken = default)
	{
		return await collection.DeleteOneAsync(BsonJsonSerializer.ToBsonDocument(objA), cancellationToken);
	}

	public async static Task<IAsyncCursor<BsonDocument>> AggregateAsync(this IMongoCollection<BsonDocument> collection, string JsonDocuments, AggregateOptions? options = null, CancellationToken cancellationToken = default)
	{
		var bsonDocuments = BsonSerializer.Deserialize<BsonDocument[]>(JsonDocuments);
		return await collection.AggregateAsync<BsonDocument>(bsonDocuments, options, cancellationToken);
	}

	public async static Task<long> CountAsync(this IMongoCollection<BsonDocument> collection, CountOptions? options = null, CancellationToken cancellationToken = default)
	{
		return await collection.CountDocumentsAsync("{}", options, cancellationToken);
	}

	public async static Task<long> CountDocumentsAsync(this IMongoCollection<BsonDocument> collection, CountOptions? options = null, CancellationToken cancellationToken = default)
	{
		return await collection.CountDocumentsAsync("{}", options, cancellationToken);
	}

	public async static Task<BulkWriteResult<BsonDocument>> BulkWriteAsync(this IMongoCollection<BsonDocument> collection, string JsonDocuments, WriteConcern? writeConcern = null, BulkWriteOptions? options = null, CancellationToken cancellationToken = default)
	{
		var bsonDocuments = BsonSerializer.Deserialize<BsonDocument[]>(JsonDocuments);

		var requests = new List<WriteModel<BsonDocument>>();
		foreach (var bsonDocument in bsonDocuments)
		{
			var first = bsonDocument.Elements.FirstOrDefault();
			var doc = first.Value.AsBsonDocument;
			switch (first.Name)
			{
				default:
					break;
				case "insertOne":
					requests.Add(new InsertOneModel<BsonDocument>(doc.GetValue("document").AsBsonDocument));
					break;
				case "updateOne":
					requests.Add(new UpdateOneModel<BsonDocument>(doc.GetValue("filter").AsBsonDocument, doc.GetValue("update").AsBsonDocument));
					break;
				case "deleteOne":
					requests.Add(new DeleteOneModel<BsonDocument>(doc.GetValue("filter").AsBsonDocument));
					break;
				case "replaceOne":
					requests.Add(new ReplaceOneModel<BsonDocument>(doc.GetValue("filter").AsBsonDocument, doc.GetValue("replacement").AsBsonDocument));
					break;
				case "updateMany":
					requests.Add(new UpdateManyModel<BsonDocument>(doc.GetValue("filter").AsBsonDocument, doc.GetValue("update").AsBsonDocument));
					break;
				case "deleteMany":
					requests.Add(new DeleteManyModel<BsonDocument>(doc.GetValue("filter").AsBsonDocument));
					break;
			}
		}

		if (writeConcern != null)
			return await collection.WithWriteConcern(writeConcern).BulkWriteAsync(requests, options, cancellationToken);
		else
			return await collection.BulkWriteAsync(requests, options, cancellationToken);
	}

	public async static Task<IChangeStreamCursor<BsonDocument>> WatchAsync(this IMongoCollection<BsonDocument> collection, string JsonDocuments, ChangeStreamOptions? options = null, CancellationToken cancellationToken = default)
	{
		var bsonDocuments = BsonSerializer.Deserialize<BsonDocument[]>(JsonDocuments);

		return await collection.WatchAsync<BsonDocument>(bsonDocuments, options, cancellationToken);
	}


	// General
	// ==============================================================================================================
	public static IMongoCollection<BsonDocument> GetCollection(this IMongoDatabase db, string CollectionName, MongoCollectionSettings? settings = null)
	{
		return db.GetCollection<BsonDocument>(CollectionName, settings);
	}

	public static string Pretty(this List<BsonDocument> list,BsonJsonSerializer.TypeSerializationEnum typeSerializationEnum)
	{
		return BsonJsonSerializer.ToJson(list, new JsonSerializerOptions() { WriteIndented = true }, typeSerializationEnum);
	}

	public static string Pretty(this List<BsonValue> list, BsonJsonSerializer.TypeSerializationEnum typeSerializationEnum)
	{
		return list.Select(x => x.ToBsonDocument()).ToList().Pretty(typeSerializationEnum);
	}

	public static string Pretty(this IAsyncCursor<BsonDocument> cursor, BsonJsonSerializer.TypeSerializationEnum typeSerializationEnum)
	{
		return cursor.ToList().Pretty(typeSerializationEnum);
	}

	public static string Pretty(this IMongoCollection<BsonDocument> collection, BsonJsonSerializer.TypeSerializationEnum typeSerializationEnum)
	{
		return collection.Find("{}").ToList().Pretty(typeSerializationEnum);
	}


	// Extensions synchronous
	// ==============================================================================================================
	public static IFindFluent<T, T>? Where<T>(this IMongoCollection<T> collection, Expression<Func<T, bool>> filter)
	{
		return collection.Find(filter);
	}

	public static T? FirstOrDefault<T>(this IMongoCollection<T> collection, Expression<Func<T, bool>> filter)
	{
		var results = collection.Find(filter);

		return results.FirstOrDefault();
	}

	public static BsonDocument RunCommand(this IMongoDatabase db, string JsonDocument, ReadPreference? readPreference = null)
	{
		return db.RunCommand<BsonDocument>(BsonDocument.Parse(JsonDocument), readPreference);
	}

	public static BsonValue InsertOne(this IMongoCollection<BsonDocument> collection, object objA, InsertOneOptions? options = null, CancellationToken cancellationToken = default)
	{
		var doc = BsonJsonSerializer.ToBsonDocument(objA);
		collection.InsertOne(doc, options, cancellationToken);
		return doc["_id"];
	}

	public static List<BsonValue> InsertMany(this IMongoCollection<BsonDocument> collection, string JsonDocuments, InsertManyOptions? options = null)
	{
		var bsonDocuments = BsonSerializer.Deserialize<BsonDocument[]>(JsonDocuments);
		collection.InsertMany(bsonDocuments, options);
		return bsonDocuments.Select(x => x["_id"]).ToList();
	}

	public static IAsyncCursor<BsonDocument> Aggregate(this IMongoCollection<BsonDocument> collection, string JsonDocuments, AggregateOptions? options = null)
	{
		var bsonDocuments = BsonSerializer.Deserialize<BsonDocument[]>(JsonDocuments);
		return collection.Aggregate<BsonDocument>(bsonDocuments, options);
	}

	[Obsolete("Use CountDocuments or EstimatedDocumentCount instead.")]
	public static long Count(this IMongoCollection<BsonDocument> collection, CountOptions? options = null)
	{
		return collection.Count("{}", options);
	}

	public static long CountDocuments(this IMongoCollection<BsonDocument> collection, CountOptions? options = null)
	{
		return collection.CountDocuments("{}", options);
	}

	public static BulkWriteResult<BsonDocument> BulkWrite(this IMongoCollection<BsonDocument> collection, string JsonDocuments, WriteConcern? writeConcern = null, BulkWriteOptions? options = null)
	{
		var bsonDocuments = BsonSerializer.Deserialize<BsonDocument[]>(JsonDocuments);

		var requests = new List<WriteModel<BsonDocument>>();
		foreach (var bsonDocument in bsonDocuments)
		{
			var first = bsonDocument.Elements.FirstOrDefault();
			var doc = first.Value.AsBsonDocument;
			switch (first.Name)
			{
				default:
					break;
				case "insertOne":
					requests.Add(new InsertOneModel<BsonDocument>(doc.GetValue("document").AsBsonDocument));
					break;
				case "updateOne":
					requests.Add(new UpdateOneModel<BsonDocument>(doc.GetValue("filter").AsBsonDocument, doc.GetValue("update").AsBsonDocument));
					break;
				case "deleteOne":
					requests.Add(new DeleteOneModel<BsonDocument>(doc.GetValue("filter").AsBsonDocument));
					break;
				case "replaceOne":
					requests.Add(new ReplaceOneModel<BsonDocument>(doc.GetValue("filter").AsBsonDocument, doc.GetValue("replacement").AsBsonDocument));
					break;
				case "updateMany":
					requests.Add(new UpdateManyModel<BsonDocument>(doc.GetValue("filter").AsBsonDocument, doc.GetValue("update").AsBsonDocument));
					break;
				case "deleteMany":
					requests.Add(new DeleteManyModel<BsonDocument>(doc.GetValue("filter").AsBsonDocument));
					break;
			}
		}

		if (writeConcern != null)
			return collection.WithWriteConcern(writeConcern).BulkWrite(requests, options);
		else
			return collection.BulkWrite(requests, options);
	}

	public static IChangeStreamCursor<BsonDocument> Watch(this IMongoCollection<BsonDocument> collection, string JsonDocuments, ChangeStreamOptions? options = null)
	{
		var bsonDocuments = BsonSerializer.Deserialize<BsonDocument[]>(JsonDocuments);

		return collection.Watch<BsonDocument>(bsonDocuments, options);
	}

	// ------------------------------------

	public async static Task<T?> FirsOrDefaultAsync<T>(this IMongoCollection<T> collection, Expression<Func<T, bool>> filter)
	{
		var results = await collection.FindAsync(filter);

		if (results == null)
			return default;
		else
			return await results.FirstOrDefaultAsync();
	}

}