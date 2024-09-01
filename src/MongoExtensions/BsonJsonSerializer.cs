
using System.Text;
using System.Text.Json;

using System.Globalization;

using MongoDB.Bson;
using MongoExtensions.Converters;

namespace MongoExtensions;

public class BsonJsonSerializer
{
	public enum TypeSerializationEnum
	{
		None,
		Colorize
	}
	private static readonly JsonSerializerOptions DefaultSerializerOptions = new()
	{
		Converters = { new DateTimeBsonConverter(), new BinaryDataBsonConverter(), new GuidBsonConverter() }
	};

	private const string SPACES = "                                                                                ";

	public static string ToJson(List<BsonDocument> bsonDocuments, JsonSerializerOptions? options, TypeSerializationEnum typeSerializationEnum)
	{
		var WriteIndented = options != null && options.WriteIndented;
		var sb = new StringBuilder();
		sb.Append('[');
		if (WriteIndented)
			sb.AppendLine();
		for (int intI = 0; intI < bsonDocuments.Count; intI++)
		{
			sb.Append(BsonJsonSerializer.ToJson(1, bsonDocuments[intI], options, typeSerializationEnum));
			if (intI < (bsonDocuments.Count - 1))
				sb.Append(',');
			if (WriteIndented)
				sb.AppendLine();
		}
		sb.Append(']');
		if (WriteIndented)
			sb.AppendLine();
		return sb.ToString();
	}

	private static string Escape(object? o)
	{
		return $"{o}"
			.Replace("\\", "\\\\")
			.Replace("\"", "\\\"")
			.Replace("\t", "\\t")
			.Replace("\b", "\\b")
			.Replace("\f", "\\f")
			.Replace("\n", "\\n")
			.Replace("\r", "\\r")
			;
	}

	private static string Indent(int Depth)
	{
		return SPACES[0..(2 * Depth)];  // TABS[0..Depth]
	}

	// Names and values MUST be encoded using doublequote chars

	public static string ToJson(int Depth, BsonValue bsonValue, JsonSerializerOptions? options, TypeSerializationEnum typeSerializationEnum)
	{
		var WriteIndented = options != null && options.WriteIndented;
		var I = WriteIndented ? Indent(Depth) : string.Empty;	// Indent
		var II = WriteIndented ? Indent(Depth+1) : string.Empty; // Indent + 1
		var N = WriteIndented ? Environment.NewLine : string.Empty; // NewLine
		var S = WriteIndented ? " " : string.Empty; // Space
		var sb = new StringBuilder();

		if (bsonValue.IsBsonDocument)
		{
			sb.Append($"{I}{{{N}");
			var bsonDocument = bsonValue.AsBsonDocument;
			for (int intI = 0; intI < bsonDocument.ElementCount; intI++)
			{
				BsonElement element = bsonDocument.Elements.ElementAt(intI);

				if(typeSerializationEnum == TypeSerializationEnum.Colorize)
					sb.Append($"{II}\u0084\"{element.Name}\"\u0080:{S}");
				else
					sb.Append($"{II}\"{element.Name}\":{S}");

				if(WriteIndented && (element.Value.IsBsonDocument || element.Value.IsBsonArray))
					sb.AppendLine();

				sb.Append($"{ToJson(Depth + 1, element.Value, options, typeSerializationEnum)}");

				if (intI < (bsonDocument.ElementCount - 1))
					sb.Append(',');
				sb.Append(N);
			}
			sb.Append($"{I}}}");

			return sb.ToString();
		}

		if (bsonValue.IsBsonArray)
		{
			sb.AppendLine($"{I}[");
			for (int intJ = 0; intJ < bsonValue.AsBsonArray.Count; intJ++)
			{
				var val = bsonValue.AsBsonArray[intJ];
				if (WriteIndented && !val.IsBsonDocument && !val.IsBsonArray)
					sb.Append(II);
				sb.Append($"{ToJson(Depth + 1, val, options, typeSerializationEnum)}");
				if (intJ < (bsonValue.AsBsonArray.Count - 1))
					sb.Append(',');
				sb.Append(N);
			}
			sb.Append($"{I}]");
			return sb.ToString();
		}

		var dotNetValue = BsonTypeMapper.MapToDotNetValue(bsonValue);

		if (typeSerializationEnum == TypeSerializationEnum.Colorize)
		{
			if (dotNetValue is BsonRegularExpression)
				dotNetValue = dotNetValue.ToString();

			if (dotNetValue is string)
				dotNetValue = $"\u0081\"{Escape(dotNetValue)}\"\u0080";

			if (dotNetValue is Guid)
				dotNetValue = $"\u008aUUID(\"{ dotNetValue}\")\u0080";

			if (dotNetValue is ObjectId)
				dotNetValue = $"\u008aObjectId(\"{dotNetValue}\")\u0080";

			//if (dotNetValue is byte[] bindata)
			//	dotNetValue = "\u008aBinary(Buffer.from(\"" + BitConverter.ToString(bindata).ToLower().Replace("-", string.Empty) + "\", \"hex\"), 0)\u0080";
			if (dotNetValue is byte[] bindata)
				dotNetValue = "\u008aBinary(\"" + Convert.ToBase64String(bindata) + "\", 0)\u0080";

			if (bsonValue is BsonTimestamp timestamp)
				dotNetValue = $"\u008aTimestamp({{ t: {timestamp.Timestamp}, i: {timestamp.Increment}}})\u0080";

			if (dotNetValue is bool boolValue)
				dotNetValue = $"\u0083{boolValue.ToString().ToLower()}\u0080";

			if (dotNetValue == null || dotNetValue is BsonNull)
				dotNetValue = $"null";

			if (dotNetValue is DateTime dtm)
				dotNetValue = FormattableString.Invariant($"\u008aISODate(\"{dtm:yyyy-MM-ddTHH:mm:ss.fffZ}\")\u0080");// dtm.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");


			if (dotNetValue is int || dotNetValue is decimal || dotNetValue is double || dotNetValue is Decimal128)
				dotNetValue = FormattableString.Invariant($"\u0082{dotNetValue}\u0080");
		}
		else
		{
			if (dotNetValue is ObjectId)
				dotNetValue = dotNetValue.ToString();

			if (dotNetValue is DateTime dtm)
				dotNetValue = dtm.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

			if (dotNetValue is BsonRegularExpression)
				dotNetValue = dotNetValue.ToString();

			if (dotNetValue is byte[] bindata)
				dotNetValue = Convert.ToBase64String(bindata);

			if (dotNetValue is BsonTimestamp)
				dotNetValue = dotNetValue.ToString();

			if (dotNetValue is Guid)
				dotNetValue = dotNetValue.ToString();

			if (dotNetValue is string)
				dotNetValue = $"\"{Escape(dotNetValue)}\"";

			if (dotNetValue is bool boolValue)
				dotNetValue = boolValue.ToString().ToLower();

			if (dotNetValue == null || dotNetValue is BsonNull)
				dotNetValue = $"null";
		}

		sb.Append(CultureInfo.InvariantCulture, $"{dotNetValue}");

		return sb.ToString();
	}


	public static BsonDocument ToBsonDocument(object objA)
	{
		return BsonDocument.Parse(JsonSerializer.Serialize(objA, DefaultSerializerOptions));
	}
}

