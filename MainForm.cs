using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace SimpleKakeibo
{
	public partial class MainForm : Form
	{
		string version = "1.0";
		Copyright copyright = new Copyright(2021, 2022, "Sora Arakawa");

		string mode = "";
		int total;
		int year;
		int month;
		List<KakeiboData> database = new List<KakeiboData>();

		public MainForm()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			if (File.Exists("savedata"))
			{
				StreamReader sr = new StreamReader("savedata", Encoding.UTF8);
				char[] separator = new char[] { ';' };
				while (sr.Peek() != -1)
				{
					string[] splitted = sr.ReadLine().Split(separator, StringSplitOptions.RemoveEmptyEntries);
					database.Add(new KakeiboData(int.Parse(splitted[0]), DateTime.Parse(splitted[1]), int.Parse(splitted[2]), splitted[3]));
				}
				sr.Close();
			}
			versionLabel.Text = "Version " + version;
			copyrightLabel.Text = copyright.getText();
			SceneProcess("mainmenu");
		}

		private void pictureBox1_Click(object sender, EventArgs e)
		{
			Point sp = Cursor.Position;
			Point cp = this.PointToClient(sp);
			int x = cp.X;
			int y = cp.Y;
			if (mode == "mainmenu")
			{
				if (x >= 162 && x <= 357 && y >= 162 && y <= 357) { SceneProcess("input"); }
				if (x >= 442 && x <= 637 && y >= 162 && y <= 357) { SceneProcess("output"); }
				if (x >= 130 && x <= 669 && y >= 20 && y <= 109) { SceneProcess("history"); }
			}
			if (mode == "input" || mode == "output")
			{
				if (x >= 588 && x <= 791 && y >= 10 && y <= 53) { SceneProcess("mainmenu"); }
				if (x >= 479 && x <= 560 && y >= 172 && y <= 207) { dateTimePicker1.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day); }
				if (x >= 569 && x <= 650 && y >= 172 && y <= 207) { var yesterday = DateTime.Today.AddDays(-1);  dateTimePicker1.Value = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day); }
				if (x >= 597 && x <= 792 && y >= 247 && y <= 442)
				{
					if (mode == "input")
					{
						database.Add(new KakeiboData(0, dateTimePicker1.Value, decimal.ToInt32(numericUpDown1.Value), textBox1.Text));
						SaveData();
						SceneProcess("input2");
					}
					if (mode == "output")
					{
						database.Add(new KakeiboData(1, dateTimePicker1.Value, decimal.ToInt32(numericUpDown1.Value), textBox1.Text));
						SaveData();
						SceneProcess("output2");
					}
				}
			}
			if (mode == "input2" || mode == "output2")
			{
				if (x >= 263 && x <= 466 && y >= 380 && y <= 423) { SceneProcess("mainmenu"); }
				if (x >= 29 && x <= 232 && y >= 380 && y <= 423)
				{
					if (mode == "input2") { SceneProcess("input"); }
					if (mode == "output2") { SceneProcess("output"); }
				}
			}
			if (mode == "history")
			{
				if (x >= 567 && x <= 770 && y >= 393 && y <= 436) { SceneProcess("mainmenu"); }
				if (x >= 429 && x <= 530 && y >= 77 && y <= 112)
				{
					month--;
					if (month < 1) { year--;month = 12; }
					CreateMonthList();
				}
				if (x >= 669 && x <= 770 && y >= 77 && y <= 112)
				{
					month++;
					if (month > 12) { year++; month = 1; }
					CreateMonthList();
				}
			}
		}

		private void label1_Click(object sender, EventArgs e)
		{
			SceneProcess("history");
		}

		void SceneProcess(string m)
		{
			Bitmap canvas = new Bitmap(pictureBox1.Width, pictureBox1.Height);
			Graphics g = Graphics.FromImage(canvas);
			Image img = Image.FromFile(m + ".png");
			g.DrawImage(img, 0, 0);
			img.Dispose();
			if (m == "mainmenu")
			{
				CalculateTotal();
				string total2 = "" + total;
				label1.Text = total2.PadLeft(8, ' ');
				label1.Visible = true;
				label2.Visible = false;
				versionLabel.Visible = true;
				copyrightLabel.Visible = true;
				dateTimePicker1.Visible = false;
				numericUpDown1.Visible = false;
				textBox1.Visible = false;
				dataGridView1.Visible = false;
			}
			if (m == "input" || m == "output")
			{
				label1.Visible = false;
				versionLabel.Visible = false;
				copyrightLabel.Visible = false;
				dateTimePicker1.Visible = true;
				textBox1.Visible = true;
				textBox1.ReadOnly = false;
				numericUpDown1.Visible = true;
				numericUpDown1.ReadOnly = false;
				numericUpDown1.Increment = 1;
				dateTimePicker1.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
				numericUpDown1.Value = 0;
				textBox1.Text = "";
			}
			if (m == "input2" || m == "output2")
			{
				numericUpDown1.ReadOnly = true;
				numericUpDown1.Increment = 0;
				textBox1.ReadOnly = true;
			}
			if (m == "history")
			{
				year = DateTime.Today.Year;
				month = DateTime.Today.Month;
				label1.Visible = false;
				versionLabel.Visible = false;
				copyrightLabel.Visible = false;
				CreateMonthList();
				label2.Visible = true;
				dataGridView1.Visible = true;
			}
			g.Dispose();
			pictureBox1.Image = canvas;
			mode = m;
		}

		void SaveData()
		{
			string saveText = "";
			for (int i = 0; i < database.Count; i++)
			{
				saveText += "" + database[i].GetKakeiboMode() + ";" + database[i].GetKakeiboDateTime().Year + "/" + database[i].GetKakeiboDateTime().Month + "/" + database[i].GetKakeiboDateTime().Day + ";" + database[i].GetKakeiboMoney() + ";" + database[i].GetKakeiboMemo() + "\n";
			}
			StreamWriter sw = new StreamWriter("savedata", false, Encoding.UTF8);
			sw.Write(saveText);
			sw.Close();
		}

		void CalculateTotal()
		{
			total = 0;
			for (int i = 0; i < database.Count; i++)
			{
				if (database[i].GetKakeiboMode() == 0)
				{
					total += database[i].GetKakeiboMoney();
				}
				if (database[i].GetKakeiboMode() == 1)
				{
					total -= database[i].GetKakeiboMoney();
				}
			}
		}

		void CreateMonthList()
		{
			label2.Text = year + "年" + month.ToString().PadLeft(2, ' ') + "月";
			dataGridView1.Columns.Clear();
			dataGridView1.ColumnCount = 4;
			dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
			dataGridView1.Columns[0].HeaderText = "日付"; dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.Programmatic;
			dataGridView1.Columns[1].HeaderText = "メモ"; dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
			dataGridView1.Columns[2].HeaderText = "収入"; dataGridView1.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
			dataGridView1.Columns[3].HeaderText = "支出"; dataGridView1.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
			dataGridView1.Columns[2].DefaultCellStyle.BackColor = Color.FromArgb(218, 237, 251);
			dataGridView1.Columns[3].DefaultCellStyle.BackColor = Color.FromArgb(249, 224, 224);
			int inputMoney = 0;
			int outputMoney = 0;
			for (int i = 0; i < database.Count; i++)
			{
				if (database[i].GetKakeiboDateTime().Year == year && database[i].GetKakeiboDateTime().Month == month)
				{
					if (database[i].GetKakeiboMode() == 0)
					{
						inputMoney += database[i].GetKakeiboMoney();
						dataGridView1.Rows.Add(database[i].GetKakeiboDateTime().Year + "/" + database[i].GetKakeiboDateTime().Month.ToString().PadLeft(2, '0') + "/" + database[i].GetKakeiboDateTime().Day.ToString().PadLeft(2, '0'), database[i].GetKakeiboMemo(), database[i].GetKakeiboMoney(), "");
					}
					if (database[i].GetKakeiboMode() == 1)
					{
						outputMoney += database[i].GetKakeiboMoney();
						dataGridView1.Rows.Add(database[i].GetKakeiboDateTime().Year + "/" + database[i].GetKakeiboDateTime().Month.ToString().PadLeft(2, '0') + "/" + database[i].GetKakeiboDateTime().Day.ToString().PadLeft(2, '0'), database[i].GetKakeiboMemo(), "", database[i].GetKakeiboMoney());
					}
				}
			}
			dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);
			/*
			dataGridView1.Rows.Add("", "", "", "");
			dataGridView1.Rows.Add("収入の合計", inputMoney, "", "");
			dataGridView1.Rows.Add("支出の合計", outputMoney, "", "");
			int checker = inputMoney - outputMoney;
			if (checker >= 0)
			{
				dataGridView1.Rows.Add("", checker + "円の黒字です", "", "");
			}
			if (checker < 0)
			{
				dataGridView1.Rows.Add("", Math.Abs(checker) + "円の赤字です", "", "");
			}
			*/
		}
	}
}
