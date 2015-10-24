using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Diagnostics;
using System.IO;

namespace SampleApp
{
    public partial class Arguments : Form
    {
        Command MyCommand;

        List<TextBox> VariableArgs;
        Home Form2_4;
        string Command;
        public Arguments(ref Home MyForm, string Item)
        {
            InitializeComponent();
            this.Form2_4 = MyForm;
            this.Command = Item;
            VariableArgs = new List<TextBox>();
            FindCommand();
            InitializeArgs();
        }        

        /// <summary>
        /// Initializes the GridView from an Input
        /// File located in 'FilePath'
        /// </summary>
        /// <param name="FilePath">Loccation of the InputFile</param>
        private void InitializeComponent(string FilePath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the Instance of Command
        /// required from Form2.Command ArrayList
        /// </summary>
        private void FindCommand()
        {
            foreach (Command cmd in Form2_4.commands)
            {
                if (cmd.commandNa.Equals(Command))
                {
                    MyCommand = cmd;
                    break;
                }
            }
        }

        /// <summary>
        /// Initializes the argument
        /// TextBoxes
        /// </summary>
        private void InitializeArgs()
        {     
            //Generate labels and text boxes
            for (int i = 0; i < MyCommand.NoOfArguments; i++)
            {
                //Create a new label and text box
                Label labelInput = new Label();
                TextBox textBoxNewInput = new TextBox();
                
                //Initialize label's property
                labelInput.Text = MyCommand.ArgList[i].ToString() + "      ";
                labelInput.Location = new Point(30, flowLayoutPanel1.Bottom + (i * 30));
                labelInput.AutoSize = true;

                //Initialize textBoxes Property
                textBoxNewInput.Location = new Point(labelInput.Width, labelInput.Top - 3);
                textBoxNewInput.Size = new System.Drawing.Size(331, 21);
                if (!string.IsNullOrEmpty(MyCommand.ArgValue[i]))
                {
                    textBoxNewInput.Text = MyCommand.ArgValue[i].ToString();
                }

                //Add the labels and text box to the form
                VariableArgs.Add(textBoxNewInput);
                this.flowLayoutPanel1.Controls.Add(labelInput);
                this.flowLayoutPanel1.Controls.Add(textBoxNewInput);
            }           
        }

        /// <summary>
        /// Event to trigger on click of 'Run' button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result;
            if (!ValidateArgumentValues())
            {
                result = MessageBox.Show("Empty Argument Values ", "Error ARGS_VALUES", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string FinalCommand = MyCommand.GetFinalCommand(VariableArgs);

            if (FinalCommand.Equals(string.Empty))
            {
                result = MessageBox.Show("Error deriving final WinRM Command ", "Error FINAL_COMMAND", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                
                string output = string.Empty;
                string error = string.Empty;

                ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd", "/c "+FinalCommand);
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.RedirectStandardError = true;
                processStartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                processStartInfo.CreateNoWindow = true;
                processStartInfo.UseShellExecute = false;

                Process process = Process.Start(processStartInfo);
                Loading load = new Loading();
                load.Show();
                load.Refresh();
                using (StreamReader streamReader = process.StandardOutput)
                {
                    output = streamReader.ReadToEnd();
                }

                using (StreamReader streamReader = process.StandardError)
                {
                    error = streamReader.ReadToEnd();
                }
                
                if (!string.IsNullOrEmpty(output))
                    WriteToOutputFile(output);
                else
                    WriteToOutputFile(error);

                MyCommand.UpdateArgValues(VariableArgs);
                load.Close();

                Display form5 = new Display();
                form5.ShowDialog();
            }
        
            
        }

        /// <summary>
        /// Writes the String Value to Output.txt
        /// </summary>
        /// <param name="Val">String Value to write</param>
        private void WriteToOutputFile(string Val)
        {
            if (!File.Exists(Constants.WinRmOutputLocation))
            {
                using (File.Create(Constants.WinRmOutputLocation))
                {
                    // Use using block to handle Resource opening.
                }
                TextWriter tw = new StreamWriter(Constants.WinRmOutputLocation);
                tw.WriteLine(Val);
                tw.Close();
            }
            else if (File.Exists(Constants.WinRmOutputLocation))
            {
                TextWriter tw = new StreamWriter(Constants.WinRmOutputLocation);
                tw.WriteLine(Val);
                tw.Close();
            }
        }

        /// <summary>
        /// Validates the Argument values
        /// entered for Null or Empty strings
        /// </summary>
        /// <returns>True if all arguments are valid</returns>
        private bool ValidateArgumentValues()
        {
            if (this.VariableArgs.Count > 0)
            {
                foreach (TextBox box in this.VariableArgs)
                {
                    if (string.IsNullOrWhiteSpace(box.Text))
                        return false;
                }
                return true;
            }
            return false;
        }
    }
}
