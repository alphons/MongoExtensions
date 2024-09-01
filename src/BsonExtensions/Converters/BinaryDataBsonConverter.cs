
using System.Text.Json.Serialization;
using System.Text.Json;

namespace BsonExtensions.Converters;

public class BinaryDataBsonConverter : JsonConverter<byte[]>
{
	public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		throw new NotImplementedException();
	}
	public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options) =>
		writer.WriteRawValue($"Binary(\"{Convert.ToBase64String(value)}\", 0)", true);
}
