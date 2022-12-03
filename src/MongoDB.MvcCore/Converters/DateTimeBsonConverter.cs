
using System.Text.Json.Serialization;
using System.Text.Json;

namespace MongoDB.MvcCore.Converters
{
	public class DateTimeBsonConverter : JsonConverter<DateTime>
	{
		public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			throw new NotImplementedException();
		}
		public override void Write(Utf8JsonWriter writer,DateTime dateTimeValue, JsonSerializerOptions options) =>
			writer.WriteRawValue(FormattableString.Invariant($"ISODate(\"{dateTimeValue:yyyy-MM-ddTHH:mm:ss.fffZ}\")"), true);
	}
}
