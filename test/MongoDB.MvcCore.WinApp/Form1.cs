

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.MvcCore;

using System.Text;
using System.Text.RegularExpressions;

namespace MongoTesting.WinApp
{
	public partial class Form1 : Form
	{
		private MongoClient? client;

		public Form1()
		{
			InitializeComponent();

			// pixels
			this.txtInput.SelectionTabs = Enumerable.Range(1, 10).Select(x => x * 15).ToArray();
			this.txtOutput.SelectionTabs = Enumerable.Range(1, 10).Select(x => x * 15).ToArray();


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
			this.txtOutput.Text = GetCollection().Pretty();
		}

		private void ShowOutput(List<BsonDocument> list)
		{
			this.txtOutput.Text = list.Pretty();
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

					if (json.StartsWith('['))
						await GetCollection().InsertManyAsync(json);
					else
						await GetCollection().InsertOneAsync(json);
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

				var list = await GetCollection().Find(Find).ToListAsync();

				ShowOutput(list);
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

				var list = await GetCollection().AggregateAsync(json);

				ShowOutput(await list.ToListAsync());
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

				await GetCollection().DeleteOneAsync(filter);

				ShowOutput();
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

				var list = await GetCollection().Find(Find).Project(Project).ToListAsync();

				ShowOutput(list);
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

				var list = await GetCollection().Find(Find).Project(Project).Sort(Sort).ToListAsync();

				ShowOutput(list);
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

				var count = await GetCollection().CountDocumentsAsync(filter);

				this.txtOutput.Text = $"total:{count}";
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

				await GetCollection().DeleteManyAsync(filter);

				ShowOutput();
			}
			catch (Exception eee)
			{
				this.txtOutput.Text = eee.Message;
			}
		}

		async private void button10_Click(object sender, EventArgs e)
		{
			this.client = new (this.txtConnectionString.Text);

			var names = await this.client.ListDatabaseNames().ToListAsync();

			this.cmbDBName.Items.Clear();
			this.cmbDBName.Items.AddRange(names.ToArray());

			this.groupBox1.Enabled = true;
			this.groupBox2.Enabled = true;
		}

		async private void cmbDBName_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.client == null)
				return;

			var db  = this.client.GetDatabase("" + this.cmbDBName.SelectedItem);
			if (db == null)
				return;

			var names = await db.ListCollectionNames().ToListAsync();
			names.Sort();

			this.cmbCollectionName.Items.Clear();

			this.cmbCollectionName.Items.AddRange(names.ToArray());

		}

		private async Task PageAsync(long Skip, long Take)
		{
			var s = this.txtInput.Text;

			s = new Regex("skip:[^}]*").Replace(s, $"skip: {Skip} ");
			s = new Regex("limit:[^}]*").Replace(s, $"limit: {Take} ");

			this.txtInput.Text = s;

			await AggregateAsync();
		}

		private long MaxRecords;

		async private void cmbCollectionName_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.txtInput.Text = @"[
{ $match: {} },
{ $skip: 0 },
{ $limit: 1 }
]";
			this.txtPage.Text = "0";
			this.txtPageLength.Text = "1";
			await PageAsync(0, 1);

			var count = await GetCollection().EstimatedDocumentCountAsync();
			MaxRecords = count;
			this.lblTotal.Text = $"total: {count}";

		}

		private async void btnPrev_Click(object sender, EventArgs e)
		{
			long.TryParse(this.txtPage.Text, out long lngPageIndex);
			long.TryParse(this.txtPageLength.Text, out long lngPageLength);
			lngPageIndex -= lngPageLength;
			if (lngPageIndex < 0)
				lngPageIndex = 0;

			this.txtPage.Text = lngPageIndex.ToString();

			await PageAsync(lngPageIndex, lngPageLength);
		}

		private async void btnNxt_Click(object sender, EventArgs e)
		{
			long.TryParse(this.txtPage.Text, out long lngPageIndex);
			long.TryParse(this.txtPageLength.Text, out long lngPageLength);
			lngPageIndex += lngPageLength;
			if (lngPageIndex >= MaxRecords)
				lngPageIndex = MaxRecords-1;

			this.txtPage.Text = lngPageIndex.ToString();

			await PageAsync(lngPageIndex, lngPageLength);
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
		{

		}
	}
}