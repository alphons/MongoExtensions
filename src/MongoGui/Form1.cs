

using MongoDB.Bson;
using MongoDB.Driver;
using BsonExtensions;

using System.Text;
using System.Text.RegularExpressions;

namespace MongoGui;

public partial class Form1 : Form
{
	private MongoClient? client;

	public Form1()
	{
		InitializeComponent();

		// pixels
		this.txtInput.SelectionTabs = Enumerable.Range(1, 30).Select(x => x * 15).ToArray();
		this.txtOutput.SelectionTabs = Enumerable.Range(1, 30).Select(x => x * 15).ToArray();

		this.Text += $" (MongoDB.MvcCore.BsonJsonSerializer: {typeof(BsonJsonSerializer).Assembly.GetName().Version})";
	}

	private void TextBox1_KeyDown(object sender, KeyEventArgs e)
	{
		//if(e.KeyData == Keys.Enter)
		//{
		//	e.Handled = true;
		//	MessageBox.Show(this.txtInput.Text);
		//}
	}


	private IMongoCollection<BsonDocument>? GetCollection()
	{
		if (this.client == null)
			return default;

		var db = this.client.GetDatabase("" + this.cmbDBName.SelectedItem);

		if (db == null)
			return default;


		return db.GetCollection("" + this.cmbCollectionName.SelectedItem);
	}

	private void ShowOutput()
	{
		this.txtOutput.Text = GetCollection()?.Pretty(BsonJsonSerializer.TypeSerializationEnum.None);
	}

	private void ShowOutput(List<BsonDocument> list)
	{
		this.txtOutput.Text = list.Pretty(BsonJsonSerializer.TypeSerializationEnum.None);
	}

	private string GetInput()
	{
		var sb = new StringBuilder();
		var json = this.txtInput.Text.Trim();
		var sr = new StringReader(json);
		while(true)
		{
			var line = sr.ReadLine();
			if (line == null)
				break;
			var i = line.IndexOf("//"); //strip comment
			if (i >= 0)
				line = line[..i];
			sb.AppendLine(line);
		}


		return sb.ToString();
	}


	private void Button1_Click(object sender, EventArgs e)
	{
		ShowOutput();
	}

	private async void Button2_Click(object sender, EventArgs e)
	{
		try
		{
			var json = GetInput();

			if (!string.IsNullOrWhiteSpace(json))
			{
				var collection = GetCollection();

				if (collection != null)
				{
					if (json.StartsWith('['))
						await collection.InsertManyAsync(json);
					else
						await collection.InsertOneAsync(json);
				}
			}

			ShowOutput();
		}
		catch (Exception eee)
		{
			this.txtOutput.Text = eee.Message;
		}
	}

	string Find = "{}";

	private async void Button3_Click(object sender, EventArgs e)
	{
		try
		{
			Find = GetInput();

			if (string.IsNullOrWhiteSpace(Find))
				Find = "{}";

			var collection = GetCollection();
			if (collection != null)
			{
				var list = await collection.Find(Find).ToListAsync();

				ShowOutput(list);
			}
		}
		catch (Exception eee)
		{
			this.txtOutput.Text = eee.Message;
		}
	}

	// aggregate
	private async void Button4_Click(object sender, EventArgs e)
	{
		await AggregateAsync();
	}

	private async Task AggregateAsync()
	{
		try
		{
			var json = GetInput();

			var collection = GetCollection();

			if (collection != null)
			{
				var list = await collection.AggregateAsync(json);

				ShowOutput(await list.ToListAsync());
			}
		}
		catch(Exception eee)
		{
			this.txtOutput.Text = eee.Message;
		}
	}

	private async void Button5_Click(object sender, EventArgs e)
	{
		try
		{
			var filter = GetInput();

			if (string.IsNullOrWhiteSpace(filter))
				filter = "{}";

			var collection = GetCollection();

			if (collection != null)
			{
				var deleteResult = await collection.DeleteOneAsync(filter);

				ShowOutput();
			}
		}
		catch (Exception eee)
		{
			this.txtOutput.Text = eee.Message;
		}
	}

	string Sort = "{}";

	string Project = "{}";

	private async void Button7_Click(object sender, EventArgs e)
	{
		try
		{
			Project = GetInput();

			var collection = GetCollection();

			if (collection != null)
			{
				var list = await collection.Find(Find).Project(Project).ToListAsync();

				ShowOutput(list);
			}
		}
		catch (Exception eee)
		{
			this.txtOutput.Text = eee.Message;
		}
	}

	private async void Button6_Click(object sender, EventArgs e)
	{
		try
		{
			Sort = GetInput();


			var collection = GetCollection();

			if (collection != null)
			{
				var list = await collection.Find(Find).Project(Project).Sort(Sort).ToListAsync();

				ShowOutput(list);
			}
		}
		catch (Exception eee)
		{
			this.txtOutput.Text = eee.Message;
		}
	}

	private async void Button8_Click(object sender, EventArgs e)
	{
		try
		{
			var filter = GetInput();

			if (string.IsNullOrWhiteSpace(filter))
				filter = "{}";

			var collection = GetCollection();

			if (collection != null)
			{
				var count = await collection.CountDocumentsAsync(filter);

				this.txtOutput.Text = $"total:{count}";
			}
		}
		catch (Exception eee)
		{
			this.txtOutput.Text = eee.Message;
		}
	}

	private async void Button9_Click(object sender, EventArgs e)
	{
		try
		{
			var filter = GetInput();

			if (string.IsNullOrWhiteSpace(filter))
				filter = "{}";

			var collection = GetCollection();

			if (collection != null)
			{
				await collection.DeleteManyAsync(filter);
			}

			ShowOutput();
		}
		catch (Exception eee)
		{
			this.txtOutput.Text = eee.Message;
		}
	}

	async private void Button10_Click(object sender, EventArgs e)
	{
		this.client = new (this.txtConnectionString.Text);

		var names = await this.client.ListDatabaseNames().ToListAsync();

		this.cmbDBName.Items.Clear();
		this.cmbDBName.Items.AddRange([.. names]);

		this.groupBox1.Enabled = true;
		this.groupBox2.Enabled = true;
	}

	async private void CmbDBName_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (this.client == null)
			return;

		var db  = this.client.GetDatabase("" + this.cmbDBName.SelectedItem);
		if (db == null)
			return;

		var names = await db.ListCollectionNames().ToListAsync();
		names.Sort();

		this.cmbCollectionName.Items.Clear();

		this.cmbCollectionName.Items.AddRange([.. names]);

	}

	private async Task PageAsync(long Skip, long Take)
	{
		var s = this.txtInput.Text;

		s = MyRegex().Replace(s, $"skip: {Skip} ");
		s = MyRegex1().Replace(s, $"limit: {Take} ");

		this.txtInput.Text = s;

		await AggregateAsync();
	}

	private long MaxRecords;

	async private void CmbCollectionName_SelectedIndexChanged(object sender, EventArgs e)
	{
		this.txtInput.Text = @"[
{ $match: {} },
{ $skip: 0 },
{ $limit: 1 }
]";
		this.txtPage.Text = "0";
		this.txtPageLength.Text = "1";
		await PageAsync(0, 1);

		var collection = GetCollection();

		if (collection != null)
		{
			var count = await collection.EstimatedDocumentCountAsync();
			MaxRecords = count;
			this.lblTotal.Text = $"total: {count}";
		}
	}

	private async void BtnPrev_Click(object sender, EventArgs e)
	{
		_ = long.TryParse(this.txtPage.Text, out long lngPageIndex);
		_ = long.TryParse(this.txtPageLength.Text, out long lngPageLength);
		lngPageIndex -= lngPageLength;
		if (lngPageIndex < 0)
			lngPageIndex = 0;

		this.txtPage.Text = lngPageIndex.ToString();

		await PageAsync(lngPageIndex, lngPageLength);
	}

	private async void BtnNxt_Click(object sender, EventArgs e)
	{
		_ = long.TryParse(this.txtPage.Text, out long lngPageIndex);
		_ = long.TryParse(this.txtPageLength.Text, out long lngPageLength);
		lngPageIndex += lngPageLength;
		if (lngPageIndex >= MaxRecords)
			lngPageIndex = MaxRecords-1;

		this.txtPage.Text = lngPageIndex.ToString();

		await PageAsync(lngPageIndex, lngPageLength);
	}

	private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
	{
		this.Close();
	}

	[GeneratedRegex("skip:[^}]*")]
	private static partial Regex MyRegex();
	[GeneratedRegex("limit:[^}]*")]
	private static partial Regex MyRegex1();
}