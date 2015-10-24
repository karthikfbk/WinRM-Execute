using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Xml;
using System.IO;

namespace SampleApp
{
    public partial class Home : Form
    {
        // ArrayList of Command. Each Instance Represents a WinRm Command
        public ArrayList commands = new ArrayList();
        
        public Home()
        {
            InitializeComponent();
        }        

        /// <summary>
        /// Event to trigger on Click of 'Add' Button
        /// Opens up a Form3
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            Home f2 = this;
            AddCommand f3 = new AddCommand(ref f2);
            f3.ShowDialog();

            RefreshCombobox();
        }

        /// <summary>
        /// Refreshes the ComboBox(DropDown of List of Commands)
        /// Sets the default text to "<--Select Command-->"
        /// </summary>
        private void RefreshCombobox()
        {
            comboBox1.Items.Clear();
            foreach (Command cmd in commands)
            {
                comboBox1.Items.Add(cmd.commandNa);
            }
            this.comboBox1.Text = "<--Select Command-->";
        }

        /// <summary>
        /// Event to trigger on Click of 'Execute' Button 
        /// Executes the Selected Command from ComboBox
        /// Opens up Form4
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex < 0)
            {
                DialogResult result = MessageBox.Show("Command Not Selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                string Item = comboBox1.SelectedItem.ToString();
                if (Item != null && Item != string.Empty)
                {
                    Home f2 = this;
                    Command mycommand = null;
                    foreach (Command cmd in commands)
                    {
                        if (cmd.commandNa.Equals(Item))
                        {
                            mycommand = cmd;
                            break;
                        }
                    }
                    Arguments f4 = new Arguments(ref f2, Item);
                    f4.ShowDialog();
                }
                else
                {
                    DialogResult result = MessageBox.Show("Selected Command is Empty or Null", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        /// <summary>
        /// Event to trigger on Click of 'Load' Button
        /// Loads set of Commands from Backup XML
        /// Populates the Combo Drop Down Box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            XmlDocument MyDocument = new XmlDocument();
            DialogResult result;
            if (File.Exists(Constants.AppDataLocation))
            {
                MyDocument.Load(Constants.AppDataLocation);
            }
            else
            {
                result = MessageBox.Show("AppData.xml does not exist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            XmlNamespaceManager nsMan = new XmlNamespaceManager(MyDocument.NameTable);
            nsMan.AddNamespace("cmd", "https://commandnamespace");
            string NoOfArg = string.Empty;
            string SampleCommand = string.Empty;
            string ArgumentName = string.Empty;
            string CommandName = string.Empty;
            string[] ArgumentNameList;
            string[] ArgumentValues;
            short NoOfArgument = 0;
            bool CommandMerged = false;

            Command MyCommand;
            XmlNodeList CommandNodeList = MyDocument.SelectNodes("//cmd:Command",nsMan);

            if (CommandNodeList != null && CommandNodeList.Count > 0)
            {
                foreach (XmlNode CommandNode in CommandNodeList)
                {
                    if (CommandNode != null && CommandNode.ChildNodes.Count > 0)
                    {
                        ArgumentNameList = new string[CommandNode.ChildNodes.Count];
                        ArgumentValues = new string[CommandNode.ChildNodes.Count];

                        NoOfArg = CommandNode.Attributes["NoOfArgs"].Value.ToString();
                        CommandName = CommandNode.Attributes["Name"].Value.ToString();
                        if (NoOfArg != string.Empty)
                        {
                            NoOfArgument = Convert.ToInt16(NoOfArg);
                        }
                        SampleCommand = CommandNode.Attributes["SampleCommand"].Value.ToString();
                        int index = 0;
                        foreach (XmlNode Argument in CommandNode.ChildNodes)
                        {
                            ArgumentName = Argument.Attributes["Name"].Value.ToString();

                            if (ArgumentName != string.Empty)
                            {
                                ArgumentNameList[index] = ArgumentName;
                                if (Argument.Attributes["Value"] != null)
                                {
                                    ArgumentValues[index] = Argument.Attributes["Value"].Value.ToString();
                                }
                                index++;
                            }
                        }

                        //Number of Arguments cannot be zero(all WinRm commands has at least one argument(IP))
                        if (NoOfArgument > 0 && SampleCommand != string.Empty
                                && ArgumentNameList.Length > 0 && CommandName != string.Empty
                            && ArgumentValues != null)
                        {
                            MyCommand = new Command(CommandName, NoOfArgument, SampleCommand, ArgumentNameList, ArgumentValues);
                            if (!CommandExist(MyCommand))
                            {
                                this.commands.Add(MyCommand);
                            }
                            else
                            {
                                CommandMerged = true;
                            }
                        }
                        else
                        {

                            result = MessageBox.Show("AppData.xml is Incorrect or does not Contain Commands to Load", "Error INCORRECT | NO_COMMAND", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                    }
                    else
                    {
                        result = MessageBox.Show("AppData.xml is Empty or Incorrect", "Error EMPTY | INCORRECT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                }
                //Refresh ComboBox to reflect the results
                RefreshCombobox();
                if(!CommandMerged)
                    result = MessageBox.Show("Commands Successfully Loaded", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    result = MessageBox.Show("Command Already Exist or have been Merged", "Command EXISTS | MERGED", MessageBoxButtons.OK, MessageBoxIcon.Information);                
            }
            else
            {
                result = MessageBox.Show("AppData.xml is Incorrect or does not Contain Commands to Load", "Error INCORRECT | NO_COMMAND", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            

        }

        /// <summary>
        /// Checks if a Command is already present in the Array List
        /// </summary>
        /// <param name="MyCommand">Command Instance</param>
        /// <returns>True if Command already Exists else Returns false</returns>
        private bool CommandExist(Command MyCommand)
        {            
            foreach (Command cmd in commands)
            {
                if (cmd.commandNa.Equals(MyCommand.commandNa))
                {                    
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Event to trigger on Click of 'Remove' Button
        /// Removes the Command from the ArrayList
        /// Updates the AppData.xml File
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result;
            if (comboBox1.SelectedIndex < 0)
            {
                result = MessageBox.Show("Command Not Selected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            result = MessageBox.Show("Are you Sure to delete Command "+comboBox1.SelectedText+"?", "Yes or No", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                string Item = comboBox1.SelectedItem.ToString();
                if (Item != null && Item != string.Empty)
                {
                    Home f2 = this;
                    Command mycommand = null;
                    foreach (Command cmd in commands)
                    {
                        if (cmd.commandNa.Equals(Item))
                        {
                            mycommand = cmd;
                            break;
                        }
                    }
                    commands.Remove(mycommand);

                    //call to Refresh ComboBox
                    RefreshCombobox();

                    //Remove Command from XML
                    RemoveFromXml(mycommand);

                    result = MessageBox.Show("Command Successfully Removed", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                else
                {
                    result = MessageBox.Show("You have selected NULL or EMPTY Command", "Command NULL | EMPTY", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        /// <summary>
        /// Removes the Command From AppData.xml
        /// </summary>
        /// <param name="MyCommand">Command Instance</param>
        private void RemoveFromXml(Command MyCommand)
        {
            XmlDocument MyDocument = new XmlDocument();
            DialogResult result;
            if (File.Exists(Constants.AppDataLocation))
            {
                MyDocument.Load(Constants.AppDataLocation);
            }
            else
            {
                result = MessageBox.Show("AppData.xml does not exist", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            XmlNamespaceManager nsMan = new XmlNamespaceManager(MyDocument.NameTable);
            nsMan.AddNamespace("cmd", "https://commandnamespace");
   
            string CommandName = string.Empty;          

            if (MyCommand != null)
            {
                CommandName = MyCommand.commandNa;
                XmlNode RootNode = MyDocument.SelectSingleNode("//cmd:Appdata", nsMan);
                XmlNode NodeToRemove = MyDocument.SelectSingleNode("//cmd:Appdata/cmd:Command[@Name='" + CommandName + "']", nsMan);

                if (RootNode!= null && NodeToRemove != null)
                {
                    RootNode.RemoveChild(NodeToRemove);
                }

            }
            MyDocument.Save(Constants.AppDataLocation);
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult result = this.openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog1.FileName;
                if (!string.IsNullOrEmpty(file))
                {
                    Display form = new Display(file);
                    form.Show();
                }              
            }
        }               
       
    }
}
