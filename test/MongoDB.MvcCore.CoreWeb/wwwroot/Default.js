'use strict';

var DbName;
var ColName;

const DataBases = $id("DataBases");
const Collections = $id("Collections");
const Output = $id("Output");
const TemplateList = $id("TemplateList");
const Template = $id("TemplateObjectProperties");
const TemplateCollumnList = $id("TemplateCollumnList");

(function ()
{
	PageEvents();
	Init();
})();


function Init()
{

}

function PageEvents()
{
	document.addEventListener("click", function (e)
	{
		if (typeof window[e.target.id] === "function")
			window[e.target.id].call(e, e);
	});

	Output.on("click", function (e)
	{
		if (e.target.tagName == "TH")
			SortData(e.target.innerText);
	});

	DataBases.on("click", function (e)
	{
		var tr = e.target.parentNode;
		if (tr.tagName != "TR")
			return;
		DbName = tr.qsall("td")[1].innerText;
		ShowCollections();
	});

	Collections.on("click", function (e)
	{
		var tr = e.target.parentNode;
		if (tr.tagName != "TR")
			return;
		ColName = tr.qsall("td")[1].innerText;
		ShowCollection();
	});

	$id("Vorige").on("click", function (e)
	{
		page--;
		if (page < 0)
			page = 0;
		GetMongoData();
	});

	$id("Volgende").on("click", function (e)
	{
		page++;
		GetMongoData();
	});

}

function MongoClientSettings()
{
	netproxy("./api/MongoClientSettings", {}, function ()
	{
		Output.Template(Template, this.Settings, false);
	});
}

function ShowDatabases()
{
	Collections.innerHTML = "";
	netproxy("./api/ShowDatabases", {}, function ()
	{
		DataBases.Template(TemplateList, { Name: "Databases", List: this.List }, false);
	});
}

function ShowCollections()
{
	netproxy("./api/ShowCollections", { DbName: DbName}, function ()
	{
		Collections.Template(TemplateList, { Name: DbName + " collections", List: this.List }, false);
	});
}

function ShowCollection()
{
	netproxy("./api/ShowCollection", { DbName: DbName, ColName: ColName }, function ()
	{
		//Output.Template(TemplateCollumnList, this.List, false);
		Output.innerHTML = RenderJSON(this);
	});
}

function Doit(name)
{
	netproxy("./api/" + name, {}, function ()
	{
		Output.Template(TemplateCollumnList, this.List, false);
	});
}

function Test0(e)
{
	Doit(e.target.id);
}

function Test1(e)
{
	Doit(e.target.id);
}

function Test3(e)
{
	Doit(e.target.id);
}

function Test4(e)
{
	Doit(e.target.id);
}

function Test5(e)
{
	Doit(e.target.id);
}

var ApiName;
var colName;
var OldColName;
var sortDir = 1;
var page = 0;

function Test6(e)
{
	colName = "Name";
	ApiName = "Test6";
	page = 0;
	GetMongoData();
}

function SortData(name)
{
	colName = name;

	page = 0;

	if (colName != OldColName)
	{
		sortDir = 1;
	}
	else
	{
		if (sortDir == 1)
			sortDir = -1;
		else
			sortDir = 1;
	}

	OldColName = colName;

	GetMongoData();
}

function GetMongoData()
{
	$id("PageNr").innerText = page;

	//if (console)
	//	console.log(page + " " + colName + " " + sortDir);

	netproxy("./api/" + ApiName, { Page: page, ColName: colName, SortDir: sortDir }, function ()
	{
		Output.Template(TemplateCollumnList, this.List, false);
	});
}


function RenderJSON(obj)
{
	var retValue = "";
	for (var key in obj)
	{
		var val = obj[key];
		if (typeof val === 'object')
		{
			retValue += "<div class='tree'>" + key;
			retValue += RenderJSON(val);
			retValue += "</div>";
		} else
		{
			retValue += "<div class='tree'>" + key + " : " + val + "</div>";
		}
	}
	return retValue;
}


