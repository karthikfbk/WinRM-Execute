using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace SampleApp
{
    public partial class AddCommand : Form
    {
        // Referrence for Home
        Home Firstform;
        // Items to display in the No Of Arguments
        string[] myitems = {"1", "2", "3", "4", "5", "6" };
        // Text Boxes to get the Argument Names
        List<TextBox> inputTextBoxes; 

        public AddCommand(ref Home P_Form)
        {
            Firstform = P_Form;            
            InitializeComponent();

            this.comboBox1.Items.AddRange(myitems);            
        }        

        /// <summary>
        /// Event to trigger on Click of 'Add' Button
        /// Validates the form Input data.
        /// Add a Command Instance to Form2.Command arrayList
        /// Updates the AppData.xml
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            int NoOfArgs;
            string[] ArgumentList;
            string[] ArgumentValues;
            DialogResult result;
            // Validate Text Boxes
            if (string.IsNullOrWhiteSpace(Title.Text))
            {
                result = MessageBox.Show("Enter a Command Name", "Error NAME", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }           
            else if(this.comboBox1.SelectedIndex < 0)
            {
                result = MessageBox.Show("Number of Arguments cannot be empty", "Error NO_OF_ARGS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if((inputTextBoxes.Count) != (Convert.ToInt32(comboBox1.SelectedItem.ToString())))
            {
                result = MessageBox.Show("Count of Number of Arguments and Argument Name List are different ", "Error NO_OF_ARGS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (!ValidateArgumentNameList())
            {
                result = MessageBox.Show("Empty Argument Names ", "Error ARGS_NAME", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (string.IsNullOrWhiteSpace(Command.Text))
            {
                result = MessageBox.Show("Empty Sample Command ", "Error SAMPLE_COMMAND", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            else
            {
                NoOfArgs = Convert.ToInt32(comboBox1.SelectedItem.ToString());
                int i = 0;
                //Argument Name List
                ArgumentList = new string[inputTextBoxes.Count];
                ArgumentValues = new string[inputTextBoxes.Count];
                foreach (TextBox argbox in inputTextBoxes)
                {
                    ArgumentList[i] = argbox.Text;
                    i++;
                }

                Command MyCommand = new Command(Title.Text, (short)NoOfArgs, Command.Text, ArgumentList, ArgumentValues);
                if (MyCommand.ValidateCommand())
                {
                    Firstform.commands.Add(MyCommand);
                    if (SaveToXml(MyCommand))
                    {
                        result = MessageBox.Show("Command Added Successfully ", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    else
                    {
                        result = MessageBox.Show("Encountered Error in updating AppData.xml ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    result = MessageBox.Show("Sample Command Validation failed ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; 
                }
            }
        }

        /// <summary>
        /// Validates the Argument Name List TextBoxes
        /// </summary>
        /// <returns>False if any text box are empty</returns>
        private bool ValidateArgumentNameList()
        {
            if (this.inputTextBoxes.Count > 0)
            {
                foreach (TextBox box in this.inputTextBoxes)
                {
                    if(string.IsNullOrWhiteSpace(box.Text))
                        return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Saves the CommandInstance to AppData.xml
        /// </summary>
        /// <param name="MyCommand">Command Instance</param>
        /// <returns>True for success</returns>
        private bool SaveToXml(SampleApp.Command MyCommand)
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
                return false;
            }

            XmlNamespaceManager nsMan = new XmlNamespaceManager(MyDocument.NameTable);
            nsMan.AddNamespace("cmd", "https://commandnamespace");
            string NoOfArg = string.Empty;
            string SampleCommand = string.Empty;            
            string CommandName = string.Empty;
            string[] ArgumentNameList;
           
            if (MyCommand != null)
            {
                NoOfArg = MyCommand.NoOfArguments.ToString();
                SampleCommand = MyCommand.SampleCom;
                ArgumentNameList = MyCommand.ArgList;
                CommandName = MyCommand.commandNa;
                XmlNode RootNode = MyDocument.SelectSingleNode("//cmd:Appdata", nsMan);
                
                if (RootNode != null && NoOfArg != string.Empty && SampleCommand != string.Empty && CommandName != string.Empty
                    && ArgumentNameList.Length > 0)
                {
                    XmlElement CoNode = MyDocument.CreateElement("cmd:Command", "https://commandnamespace");

                    XmlAttribute CoName = MyDocument.CreateAttribute("Name");
                    CoName.Value = CommandName;

                    XmlAttribute CoNoOfArgs = MyDocument.CreateAttribute("NoOfArgs");
                    CoNoOfArgs.Value = NoOfArg;

                    XmlAttribute CoSampleCommand = MyDocument.CreateAttribute("SampleCommand");
                    CoSampleCommand.Value = SampleCommand;


                    CoNode.SetAttributeNode(CoName);
                    CoNode.SetAttributeNode(CoNoOfArgs);
                    CoNode.SetAttributeNode(CoSampleCommand);

                    for (int i = 0; i < ArgumentNameList.Length; i++)
                    {
                        XmlElement ArNode = MyDocument.CreateElement("cmd:Argument", "https://commandnamespace");

                        XmlAttribute ArName = MyDocument.CreateAttribute("Name");
                        ArName.Value = ArgumentNameList[i];

                        ArNode.SetAttributeNode(ArName);

                        CoNode.AppendChild(ArNode);
                    }

                    RootNode.AppendChild(CoNode);
                    MyDocument.Save(Constants.AppDataLocation);

                    return true;
                }
                else
                    return false;
            }
            return false;
        }

        /// <summary>
        /// Event triggered on change of No of Arguments
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.flowLayoutPanel1.Controls.Clear();            
            string val = string.Empty;
            if (this.comboBox1.SelectedIndex >= 0)
            {
                val = comboBox1.SelectedItem.ToString();
            }
            int inputNumber = 0;
            if (val != string.Empty)
            {
                inputNumber = Convert.ToInt32(val);
            }

            inputTextBoxes = new List<TextBox>();
            
            //Generate labels and text boxes
            for (int i = 1; i <= inputNumber; i++)
            {
                //Create a new label and text box
                Label labelInput = new Label();
                TextBox textBoxNewInput = new TextBox();

                //Initialize label's property
                labelInput.Text = "Enter Name of Argument " + (i) + "      ";
                labelInput.Location = new Point(60,  167+(i * 36));
                labelInput.AutoSize = true;

                //Initialize textBoxes Property
                textBoxNewInput.Location = new Point(180, 167+(i*36));
                textBoxNewInput.Size = new System.Drawing.Size(331, 21);
                //Add the newly created text box to the list of input text boxes
                inputTextBoxes.Add(textBoxNewInput);

                //Add the labels and text box to the form
                
                this.flowLayoutPanel1.Controls.Add(labelInput);
                this.flowLayoutPanel1.Controls.Add(textBoxNewInput);
            }

        }
        
    }
}
