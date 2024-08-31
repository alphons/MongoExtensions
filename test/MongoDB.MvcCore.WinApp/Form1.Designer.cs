namespace MongoTesting.WinApp
{
	partial class Form1
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.txtConnectionString = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.button4 = new System.Windows.Forms.Button();
			this.button5 = new System.Windows.Forms.Button();
			this.txtInput = new System.Windows.Forms.RichTextBox();
			this.txtOutput = new System.Windows.Forms.RichTextBox();
			this.button6 = new System.Windows.Forms.Button();
			this.button7 = new System.Windows.Forms.Button();
			this.button8 = new System.Windows.Forms.Button();
			this.button9 = new System.Windows.Forms.Button();
			this.button10 = new System.Windows.Forms.Button();
			this.cmbDBName = new System.Windows.Forms.ComboBox();
			this.cmbCollectionName = new System.Windows.Forms.ComboBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.lblTotal = new System.Windows.Forms.Label();
			this.btnPrev = new System.Windows.Forms.Button();
			this.btnNxt = new System.Windows.Forms.Button();
			this.txtPage = new System.Windows.Forms.TextBox();
			this.txtPageLength = new System.Windows.Forms.TextBox();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 25);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(22, 15);
			this.label1.TabIndex = 1;
			this.label1.Text = "DB";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(157, 25);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(25, 15);
			this.label2.TabIndex = 3;
			this.label2.Text = "Col";
			// 
			// txtConnectionString
			// 
			this.txtConnectionString.Location = new System.Drawing.Point(58, 12);
			this.txtConnectionString.Name = "txtConnectionString";
			this.txtConnectionString.Size = new System.Drawing.Size(307, 23);
			this.txtConnectionString.TabIndex = 6;
			this.txtConnectionString.Text = "mongodb://127.0.0.1:27017";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(13, 15);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(39, 15);
			this.label3.TabIndex = 5;
			this.label3.Text = "Server";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(6, 320);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 8;
			this.button1.Text = "ToList()";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.Button1_Click);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(6, 210);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 9;
			this.button2.Text = "Insert";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.Button2_Click);
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(6, 51);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(75, 23);
			this.button3.TabIndex = 10;
			this.button3.Text = "Find";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.Button3_Click);
			// 
			// button4
			// 
			this.button4.Location = new System.Drawing.Point(6, 138);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(75, 23);
			this.button4.TabIndex = 11;
			this.button4.Text = "Aggregate";
			this.button4.UseVisualStyleBackColor = true;
			this.button4.Click += new System.EventHandler(this.Button4_Click);
			// 
			// button5
			// 
			this.button5.Location = new System.Drawing.Point(6, 239);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(75, 23);
			this.button5.TabIndex = 12;
			this.button5.Text = "Delete 1";
			this.button5.UseVisualStyleBackColor = true;
			this.button5.Click += new System.EventHandler(this.Button5_Click);
			// 
			// txtInput
			// 
			this.txtInput.AcceptsTab = true;
			this.txtInput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtInput.DetectUrls = false;
			this.txtInput.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.txtInput.Location = new System.Drawing.Point(11, 114);
			this.txtInput.Name = "txtInput";
			this.txtInput.Size = new System.Drawing.Size(441, 526);
			this.txtInput.TabIndex = 13;
			this.txtInput.Text = "";
			// 
			// txtOutput
			// 
			this.txtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtOutput.DetectUrls = false;
			this.txtOutput.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.txtOutput.Location = new System.Drawing.Point(3, 40);
			this.txtOutput.Name = "txtOutput";
			this.txtOutput.ReadOnly = true;
			this.txtOutput.Size = new System.Drawing.Size(365, 600);
			this.txtOutput.TabIndex = 14;
			this.txtOutput.Text = "";
			// 
			// button6
			// 
			this.button6.Location = new System.Drawing.Point(6, 109);
			this.button6.Name = "button6";
			this.button6.Size = new System.Drawing.Size(75, 23);
			this.button6.TabIndex = 15;
			this.button6.Text = "Sort";
			this.button6.UseVisualStyleBackColor = true;
			this.button6.Click += new System.EventHandler(this.Button6_Click);
			// 
			// button7
			// 
			this.button7.Location = new System.Drawing.Point(6, 80);
			this.button7.Name = "button7";
			this.button7.Size = new System.Drawing.Size(75, 23);
			this.button7.TabIndex = 16;
			this.button7.Text = "Project";
			this.button7.UseVisualStyleBackColor = true;
			this.button7.Click += new System.EventHandler(this.Button7_Click);
			// 
			// button8
			// 
			this.button8.Location = new System.Drawing.Point(6, 22);
			this.button8.Name = "button8";
			this.button8.Size = new System.Drawing.Size(75, 23);
			this.button8.TabIndex = 17;
			this.button8.Text = "Count";
			this.button8.UseVisualStyleBackColor = true;
			this.button8.Click += new System.EventHandler(this.Button8_Click);
			// 
			// button9
			// 
			this.button9.Location = new System.Drawing.Point(6, 268);
			this.button9.Name = "button9";
			this.button9.Size = new System.Drawing.Size(75, 23);
			this.button9.TabIndex = 18;
			this.button9.Text = "Delete";
			this.button9.UseVisualStyleBackColor = true;
			this.button9.Click += new System.EventHandler(this.Button9_Click);
			// 
			// button10
			// 
			this.button10.Location = new System.Drawing.Point(371, 12);
			this.button10.Name = "button10";
			this.button10.Size = new System.Drawing.Size(75, 23);
			this.button10.TabIndex = 19;
			this.button10.Text = "Connect";
			this.button10.UseVisualStyleBackColor = true;
			this.button10.Click += new System.EventHandler(this.Button10_Click);
			// 
			// cmbDBName
			// 
			this.cmbDBName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbDBName.FormattingEnabled = true;
			this.cmbDBName.Location = new System.Drawing.Point(37, 22);
			this.cmbDBName.Name = "cmbDBName";
			this.cmbDBName.Size = new System.Drawing.Size(114, 23);
			this.cmbDBName.TabIndex = 20;
			this.cmbDBName.SelectedIndexChanged += new System.EventHandler(this.CmbDBName_SelectedIndexChanged);
			// 
			// cmbCollectionName
			// 
			this.cmbCollectionName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbCollectionName.FormattingEnabled = true;
			this.cmbCollectionName.Location = new System.Drawing.Point(188, 22);
			this.cmbCollectionName.Name = "cmbCollectionName";
			this.cmbCollectionName.Size = new System.Drawing.Size(247, 23);
			this.cmbCollectionName.TabIndex = 21;
			this.cmbCollectionName.SelectedIndexChanged += new System.EventHandler(this.CmbCollectionName_SelectedIndexChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.cmbDBName);
			this.groupBox1.Controls.Add(this.cmbCollectionName);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Enabled = false;
			this.groupBox1.Location = new System.Drawing.Point(11, 42);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(441, 66);
			this.groupBox1.TabIndex = 22;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "settings";
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.button8);
			this.groupBox2.Controls.Add(this.button2);
			this.groupBox2.Controls.Add(this.button3);
			this.groupBox2.Controls.Add(this.button9);
			this.groupBox2.Controls.Add(this.button4);
			this.groupBox2.Controls.Add(this.button1);
			this.groupBox2.Controls.Add(this.button5);
			this.groupBox2.Controls.Add(this.button7);
			this.groupBox2.Controls.Add(this.button6);
			this.groupBox2.Enabled = false;
			this.groupBox2.Location = new System.Drawing.Point(458, 42);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(89, 610);
			this.groupBox2.TabIndex = 23;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "shortcuts";
			// 
			// lblTotal
			// 
			this.lblTotal.AutoSize = true;
			this.lblTotal.Location = new System.Drawing.Point(243, 11);
			this.lblTotal.Name = "lblTotal";
			this.lblTotal.Size = new System.Drawing.Size(31, 15);
			this.lblTotal.TabIndex = 24;
			this.lblTotal.Text = "total";
			// 
			// btnPrev
			// 
			this.btnPrev.Location = new System.Drawing.Point(44, 7);
			this.btnPrev.Name = "btnPrev";
			this.btnPrev.Size = new System.Drawing.Size(26, 23);
			this.btnPrev.TabIndex = 25;
			this.btnPrev.Text = "<";
			this.btnPrev.UseVisualStyleBackColor = true;
			this.btnPrev.Click += new System.EventHandler(this.BtnPrev_Click);
			// 
			// btnNxt
			// 
			this.btnNxt.Location = new System.Drawing.Point(117, 7);
			this.btnNxt.Name = "btnNxt";
			this.btnNxt.Size = new System.Drawing.Size(26, 23);
			this.btnNxt.TabIndex = 26;
			this.btnNxt.Text = ">";
			this.btnNxt.UseVisualStyleBackColor = true;
			this.btnNxt.Click += new System.EventHandler(this.BtnNxt_Click);
			// 
			// txtPage
			// 
			this.txtPage.Location = new System.Drawing.Point(76, 8);
			this.txtPage.Name = "txtPage";
			this.txtPage.Size = new System.Drawing.Size(35, 23);
			this.txtPage.TabIndex = 27;
			this.txtPage.Text = "0";
			// 
			// txtPageLength
			// 
			this.txtPageLength.Location = new System.Drawing.Point(169, 8);
			this.txtPageLength.Name = "txtPageLength";
			this.txtPageLength.Size = new System.Drawing.Size(35, 23);
			this.txtPageLength.TabIndex = 28;
			this.txtPageLength.Text = "1";
			// 
			// statusStrip1
			// 
			this.statusStrip1.Location = new System.Drawing.Point(0, 679);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(1006, 22);
			this.statusStrip1.TabIndex = 29;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(1006, 24);
			this.menuStrip1.TabIndex = 30;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(93, 22);
			this.exitToolStripMenuItem.Text = "Exit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 24);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.txtConnectionString);
			this.splitContainer1.Panel1.Controls.Add(this.label3);
			this.splitContainer1.Panel1.Controls.Add(this.txtInput);
			this.splitContainer1.Panel1.Controls.Add(this.button10);
			this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
			this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
			this.splitContainer1.Panel1MinSize = 550;
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.btnPrev);
			this.splitContainer1.Panel2.Controls.Add(this.txtOutput);
			this.splitContainer1.Panel2.Controls.Add(this.lblTotal);
			this.splitContainer1.Panel2.Controls.Add(this.txtPageLength);
			this.splitContainer1.Panel2.Controls.Add(this.btnNxt);
			this.splitContainer1.Panel2.Controls.Add(this.txtPage);
			this.splitContainer1.Panel2MinSize = 400;
			this.splitContainer1.Size = new System.Drawing.Size(1006, 655);
			this.splitContainer1.SplitterDistance = 550;
			this.splitContainer1.SplitterWidth = 20;
			this.splitContainer1.TabIndex = 31;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1006, 701);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "Form1";
			this.Text = "MongoDB - tester";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private Label label1;
		private Label label2;
		private TextBox txtConnectionString;
		private Label label3;
		private Button button1;
		private Button button2;
		private Button button3;
		private Button button4;
		private Button button5;
		private RichTextBox txtInput;
		private RichTextBox txtOutput;
		private Button button6;
		private Button button7;
		private Button button8;
		private Button button9;
		private Button button10;
		private ComboBox cmbDBName;
		private ComboBox cmbCollectionName;
		private GroupBox groupBox1;
		private GroupBox groupBox2;
		private Label lblTotal;
		private Button btnPrev;
		private Button btnNxt;
		private TextBox txtPage;
		private TextBox txtPageLength;
		private StatusStrip statusStrip1;
		private MenuStrip menuStrip1;
		private ToolStripMenuItem fileToolStripMenuItem;
		private ToolStripMenuItem exitToolStripMenuItem;
		private SplitContainer splitContainer1;
	}
}