
using System.Text.Json.Serialization;
using System.Text.Json;

namespace MongoExtensions.Converters;

public class GuidBsonConverter : JsonConverter<Guid>
{
	public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		throw new NotImplementedException();
	}
	public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options) =>
		writer.WriteRawValue($"UUID(\"{value}\")", true);
}
