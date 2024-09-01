using MongoDB.Bson;

using System.Text.Json.Serialization;
using System.Text.Json;

namespace BsonExtensions.Converters;

public class BsonDocumentJsonConverter : JsonConverter<BsonDocument>
{
	public override BsonDocument Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		throw new NotImplementedException();
	}

	public override void Write(Utf8JsonWriter writer, BsonDocument bsonDocument, JsonSerializerOptions options)
	{
		writer.WriteRawValue(BsonJsonSerializer.ToJson(0, bsonDocument, options, BsonJsonSerializer.TypeSerializationEnum.None));
	}
}
