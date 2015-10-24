using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SampleApp
{
    public partial class Display : Form
    {
        string SearchColumnValue;
        string Firstline;
        List<string> Columns;        
        List<List<string>> Rows;
        public Display()
        {
            InitializeComponent();
            SearchColumnValue = string.Empty;
            Firstline = string.Empty;
            Columns = new List<string>();
            Rows = new List<List<string>>();
            this.label4.Visible = false;
            this.label5.Visible = false;
            InitializeGridView();            
        }

        public Display(string FilePath)
        {
            InitializeComponent();
            SearchColumnValue = string.Empty;
            Firstline = string.Empty;
            Columns = new List<string>();
            Rows = new List<List<string>>();
            this.label4.Visible = false;
            this.label5.Visible = false;
            InitializeGridView(FilePath);
        }

        /// <summary>
        /// Initializes the Gridview from Input file
        /// </summary>
        /// <param name="FilePath">Path of the Input File</param>
        private void InitializeGridView(string FilePath)
        {
            //string totalstring = null;
            System.IO.StreamReader file = null;
            if (string.IsNullOrEmpty(FilePath))
                return;
            if (File.Exists(FilePath))
            {
                file = new System.IO.StreamReader(FilePath);
            }
            if (file != null)
            {
                Firstline = ReadSparClass(ref file);
                Columns = ReadColumns(ref file, Firstline);
                Rows = ReadRows(ref file, Firstline);

                if ((Firstline != null) && (Columns.Count > 0) && (Rows.Count > 0)
                    && Firstline.StartsWith("SPAR"))
                {
                    DisplayGrid(Columns, Rows);
                    PopulateComboListBox(Columns);
                    PopulateCheckListBox(Columns);
                    this.label4.Text = Firstline + " : " + this.dataGridView1.RowCount.ToString() + " Instances";
                    this.label4.Visible = true;
                }
                else
                {
                    string value = file.ReadToEnd();
                    this.textBox1.Text = value;
                    this.TextboxContent.Text = "Command Output";
                    this.TextboxContent.Visible = true;
                }
                file.Close();
            }
        }

        /// <summary>
        /// Initializes Grid View from 
        /// executing Winrm Command.
        /// </summary>
        private void InitializeGridView()
        {
            //string totalstring = null;
            System.IO.StreamReader file=null;
            if (File.Exists(Constants.WinRmOutputLocation))
            {
                file = new System.IO.StreamReader(Constants.WinRmOutputLocation);
            }
            if (file != null)
            {
                Firstline = ReadSparClass(ref file);
                Columns = ReadColumns(ref file, Firstline);
                Rows = ReadRows(ref file, Firstline);

                if ((Firstline != null) && (Columns.Count > 0) && (Rows.Count > 0)
                    && Firstline.StartsWith("SPAR"))
                {
                    DisplayGrid(Columns, Rows);
                    PopulateComboListBox(Columns);
                    PopulateCheckListBox(Columns);
                    this.label4.Text = Firstline + " : " + this.dataGridView1.RowCount.ToString() + " Instances";
                    this.label4.Visible = true;
                }
                else
                {
                    string value = file.ReadToEnd();
                    this.textBox1.Text = value;
                    this.TextboxContent.Text = "Command Output";
                    this.TextboxContent.Visible = true;
                }
                file.Close();
            }
        }

        private void PopulateCheckListBox(List<string> Columns)
        {
            if (Columns.Count > 0)
            {
                for (int i = 0; i < Columns.Count; i++)
                {
                    this.checkedListBox1.Items.Add(Columns[i].ToString());
                }
            }
        }

        private void PopulateComboListBox(List<string> Columns)
        {
            if (Columns.Count > 0)
            {
                for (int i = 0; i < Columns.Count; i++)
                {                    
                    this.comboBox1.Items.Add(Columns[i].ToString());
                }
            }
        }

       

        private void DisplayGrid(List<string> Columns, List<List<string>> Rows)
        {
            DataTable dt = new DataTable();
            foreach (string c in Columns)
            {
                dt.Columns.Add(c);
            }            

            foreach (List<string> Row in Rows)
            {
                DataRow dr = dt.NewRow();
                for (int i = 0; i < Row.Count; i++)
                {
                    dr[i] = Row[i];
                }
                if(Row.Count > 0)
                    dt.Rows.Add(dr);
            }          
            this.dataGridView1.DataSource = dt;
        }

        private List<List<string>> ReadRows(ref System.IO.StreamReader  file, string FirstLine)
        {
            List<List<string>> Rows = new List<List<string>>();
            List<string> RowData = new List<string>();
            string line = string.Empty;            
            while ((line = file.ReadLine()) != null)
            {                
                string[] elements;                
                if ((line.Equals(FirstLine)))
                    continue;
                if ((line.Equals(Environment.NewLine)) || (line.Equals(string.Empty)))
                {
                    Rows.Add(RowData);                    
                    RowData = new List<string>();
                }
                else if (line.Contains('='))
                {
                    elements = line.Split('=');
                    RowData.Add(elements[1].Trim());                    
                }
                else
                {
                    // Do nothing
                }
                
            }            
            if (file.EndOfStream)
            {
                Rows.Add(RowData);
            }
            file.BaseStream.Position = 0;
            file.DiscardBufferedData();
            return Rows;
        }

        private List<string> ReadColumns(ref System.IO.StreamReader file, string FirstLine)
        {
            List<string> col = new List<string>();
            string line = string.Empty;
            string[] elements;
            int Index = 0;
            while ((line = file.ReadLine()) != null)
            {
                if((line.Equals(FirstLine)))
                    continue;
                if ((line.Equals(Environment.NewLine)) || (line.Equals(string.Empty)))
                    break;
                elements = line.Split('=');
                col.Add(elements[0].Trim());
                Index++;
            }
            file.BaseStream.Position = 0;
            file.DiscardBufferedData();
            return col;
        }

        private string ReadSparClass(ref System.IO.StreamReader file)
        {
            string line = string.Empty;
            if ((line = file.ReadLine()) != null)
            {
                file.BaseStream.Position = 0;
                file.DiscardBufferedData();
                return line;
            }
            return line;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                {
                    TextboxContent.Text = dataGridView1.Columns[e.ColumnIndex].HeaderText.ToString();
                    TextboxContent.Visible = true;
                    textBox1.Text = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                }
            }
        }        

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex < 0)
            {
                DialogResult result = MessageBox.Show("Invalid Column", "the question", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string Item = comboBox1.SelectedItem.ToString();
                string SearchValue = this.textBox2.Text;
                int columnIndex = -1;
                int Totalhits = 0;
                if (!SearchValue.Equals(string.Empty))
                {
                    for (int i = 0; i < this.dataGridView1.Columns.Count; i++)
                    {
                        if (this.dataGridView1.Columns[i].HeaderText.Equals(Item))
                        {
                            columnIndex = i;
                            break;
                        }
                    }

                    for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
                    {
                        if (this.dataGridView1.Rows[i].Cells[columnIndex].Value != null)
                        {
                            if (this.dataGridView1.Rows[i].Cells[columnIndex].Value.ToString().Equals(SearchValue))
                            {                                
                                this.dataGridView1.Rows[i].Visible = true;
                                Totalhits++;
                            }
                            else
                            {
                                this.dataGridView1.CurrentCell = null;
                                this.dataGridView1.Rows[i].Visible = false;                                
                            }
                        }
                    }
                    this.label5.Text = "Total Hits : " + Totalhits.ToString();
                    this.label5.Visible = true;
                }

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.Rows.Count > 0)
            {
                for (int i = 0; i < this.dataGridView1.Rows.Count; i++)
                {
                    this.dataGridView1.Rows[i].Visible = true;
                }
                for (int i = 0; i < this.dataGridView1.Columns.Count; i++)
                {
                    this.dataGridView1.Columns[i].Visible = true;
                }
                foreach (int i in checkedListBox1.CheckedIndices)
                {
                    checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                }
                // Make the search hits invisible on Refresh
                this.label5.Visible = false;
            }
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (this.checkedListBox1.Items.Count > 0)
            {
                if (this.checkedListBox1.CheckedItems.Count == 0)
                {
                    DialogResult result = MessageBox.Show("Invalid Column", "the question", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    for (int i = 0; i < this.dataGridView1.Columns.Count; i++)
                    {
                        bool selected = false;
                        for (int j = 0; j < this.checkedListBox1.CheckedItems.Count; j++)
                        {
                            if(this.dataGridView1.Columns[i].HeaderText.Equals(this.checkedListBox1.CheckedItems[j].ToString())){
                                selected = true;
                            }
                            if (selected)
                            {
                                this.dataGridView1.Columns[i].Visible = true;
                            }
                            else
                            {
                                this.dataGridView1.Columns[i].Visible = false;
                            }
                        }
                    }
                    this.label5.Visible = false;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }                
    }
}
