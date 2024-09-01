
using System.Text;

namespace MongoExtensions.ConsoleApp;

public class FunctionArguments
{
	public string Name { get; set; }
	public List<string> Arguments { get; set; }

	public bool IsFunction { get; set; }

	public bool Complete { get; set; }

	public FunctionArguments()
	{
		this.Name = string.Empty;
		this.Arguments = new();
	}
}
public static class ParseHelper
{
	private enum PartEnum
	{
		Unknown,
		FunctionName, // alphonsnumeric and _
		Arguments,
		Parenthesis, // (
		SingleQuote, // '
		DoubleQuote, // "
		SquareBracket, // [
		CurlyBracket // {
	}


	public static FunctionArguments ParseFunction(string s)
	{
		Stack<PartEnum> nivo = new();

		nivo.Push(PartEnum.Unknown);

		FunctionArguments fa = new();

		StringBuilder sb = new();

		for (int intI = 0; intI < s.Length; intI++)
		{
			var c = s[intI];

			switch (nivo.Peek())
			{
				default:
					break;
				case PartEnum.Unknown:
					if (char.IsWhiteSpace(c))
						continue;
					if (c == '(')
					{
						fa.IsFunction = true;
						nivo.Pop();
						nivo.Push(PartEnum.Parenthesis);
						continue; // function, so next char is exit
					}
					if (fa.Name.Length > 0) // already a name
					{
						nivo.Push(PartEnum.Arguments);
						sb.Append(c);
					}
					else
					{
						nivo.Push(PartEnum.FunctionName);
						fa.Name = $"{c}";
					}
					continue;
				case PartEnum.FunctionName:
					if (char.IsLetterOrDigit(c) || c == '_')
						fa.Name += c;
					else
					{
						nivo.Pop();
						if (c == '(')
						{
							fa.IsFunction = true;
							nivo.Push(PartEnum.Parenthesis);
						}
					}
					continue;
				case PartEnum.Arguments:
					if (char.IsWhiteSpace(c))
					{
						var aa = sb.ToString().Trim();
						if (aa.Length > 0)
						{
							fa.Arguments.Add(sb.ToString().Trim());
							sb = new();
						}
					}
					else
					{
						sb.Append(c);
					}
					continue;
			}

			switch (c)
			{
				default:
					break;
				case '\\':
					sb.Append(c);
					intI++;
					c = s[intI]; // todo check boundaries
					break;
				case '(':
					nivo.Push(PartEnum.Parenthesis);
					break; ;
				case ',':
					if (nivo.Peek() == PartEnum.Parenthesis)
						continue;
					break;
				case ')':
					if (nivo.Peek() == PartEnum.Parenthesis)
						nivo.Pop();
					if (nivo.Count <= 2)
						continue;
					break;
				case '{':
					nivo.Push(PartEnum.CurlyBracket);
					break;
				case '[':
					nivo.Push(PartEnum.SquareBracket);
					break;
				case '\'':
					if (nivo.Peek() == PartEnum.DoubleQuote)
						break;
					if (nivo.Peek() == PartEnum.SingleQuote)
						nivo.Pop();
					else
						nivo.Push(PartEnum.SingleQuote);
					break;
				case '\"':
					if (nivo.Peek() == PartEnum.SingleQuote)
						break;
					if (nivo.Peek() == PartEnum.DoubleQuote)
						nivo.Pop();
					else
						nivo.Push(PartEnum.DoubleQuote);
					break;
				case '}':
					if (nivo.Peek() == PartEnum.CurlyBracket)
						nivo.Pop();
					if (nivo.Peek() == PartEnum.Parenthesis)
					{
						sb.Append(c);
						fa.Arguments.Add(sb.ToString().Trim());
						sb = new();
						continue;
					}
					break;
				case ']':
					if (nivo.Peek() == PartEnum.SquareBracket)
						nivo.Pop();
					if (nivo.Peek() == PartEnum.Parenthesis)
					{
						sb.Append(c);
						fa.Arguments.Add(sb.ToString().Trim());
						sb = new();
						continue;
					}
					break;
			}
			sb.Append(c);
		}
		var rest = sb.ToString().Trim();
		if (!string.IsNullOrWhiteSpace(rest))
			fa.Arguments.Add(rest);

		fa.Complete = (nivo.Count < 2) || (nivo.Peek() == PartEnum.FunctionName) || (nivo.Peek() == PartEnum.Arguments);

		return fa;
	}
}


