using MongoDB.Bson;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace BsonExtensions.Converters;

public class BsonDocumentsJsonConverter : JsonConverter<List<BsonDocument>>
{
	public override List<BsonDocument> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		throw new NotImplementedException();
	}

	public override void Write(Utf8JsonWriter writer, List<BsonDocument> bsonDocuments, JsonSerializerOptions? options)
	{
		writer.WriteRawValue(BsonJsonSerializer.ToJson(bsonDocuments, options, BsonJsonSerializer.TypeSerializationEnum.None));
	}
}
