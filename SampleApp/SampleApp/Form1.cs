using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace SampleApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //string output = string.Empty;
            //string error = string.Empty;

            //ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd", "/c dir");
            //processStartInfo.RedirectStandardOutput = true;
            //processStartInfo.RedirectStandardError = true;
            //processStartInfo.WindowStyle = ProcessWindowStyle.Normal;
            //processStartInfo.UseShellExecute = false;

            //Process process = Process.Start(processStartInfo);
            //using (StreamReader streamReader = process.StandardOutput)
            //{
            //    output = streamReader.ReadToEnd();
            //}

            //using (StreamReader streamReader = process.StandardError)
            //{
            //    error = streamReader.ReadToEnd();
            //}

            //Console.WriteLine("The following output was detected:");
            //Console.WriteLine(output);

            //if (!string.IsNullOrEmpty(error))
            //{
            //    Console.WriteLine("The following error was detected:");
            //    Console.WriteLine(error);
            //}

            //System.Diagnostics.Process.Start("Cmd.exe", "/c dir");
            //System.Diagnostics.Process.Start("Cmd.exe", "/c ping 198.163.2.1");

            //Console.ReadKey();
            Form2 f2 = new Form2();
            f2.ShowDialog();
 
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
