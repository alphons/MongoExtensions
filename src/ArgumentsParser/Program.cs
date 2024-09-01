

using MonoCli;

//Helper.Parse(" f ( { a:'b', c:\"d\", e: [ aa: 'bb' , i : 123 ] }, {  a:'b', c:\"d\", e: [ aa: 'bb' , i : 123 ]} )");

//Helper.Parse(" f (  [ { }, {}, {  }, {}    ]   )");

//var af1 = Helper.ParseFunction(" f ( { }, {}, {  }, {}       )");

//var af2 = Helper.ParseFunction("f(a )");

string s;

s = " ls -al";
//s = " f( a";
//s = " insert { _id: 'abc' }";

var af = ParseHelper.ParseFunction(s);
Console.WriteLine($"[{s}] Complete:{af.Complete} Function:{af.IsFunction} Name:{af.Name}");
foreach (var a in af.Arguments)
	Console.WriteLine($"\t{a}");

var totalArguments = string.Join(' ', af.Arguments);
Console.WriteLine("TotalArguments:" + totalArguments);
Console.WriteLine("----");

//var af4 = Helper.ParseFunction(" cls");