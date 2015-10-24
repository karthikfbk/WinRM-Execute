using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.Xml;
using System.IO;

namespace SampleApp
{
    class Command
    {
        string CommandName;
        Int16 NoOfArg;
        string SampleCommand;
        string OutputCommand;
        string[] ArgumentList;
        string[] ArgumentValue;
        public string commandNa
        {
            get { return CommandName; }
            set { CommandName = value; }
        }
        public short NoOfArguments
        {
            get { return NoOfArg; }
            set { NoOfArg = value; }
        }

        public string[] ArgList
        {
            get { return ArgumentList; }
            set { ArgumentList = value; }
        }

        public string[] ArgValue
        {
            get { return ArgumentValue; }
            set { ArgumentValue = value; }
        }
        public string SampleCom
        {
            get { return SampleCommand; }
            set { SampleCommand = value; }
        }

        public Command()
        {
            CommandName = string.Empty;
            SampleCommand = string.Empty;
            OutputCommand = string.Empty;
            ArgumentList = null;
            ArgumentValue = null;
            NoOfArg = 0;
        }

        public Command(string Name, Int16 args, string SamCommand, string[] argsList, string[] argsValue)
        {
            if (!Name.Equals(string.Empty))
            {
                CommandName = Name;
            }
            if (!SamCommand.Equals(string.Empty))
            {
                SampleCommand = SamCommand;
            }
            if (args != 0)
            {
                NoOfArg = args;
            }
            if (argsList != null)
            {
                ArgumentList = argsList;
            }
            if (argsValue != null)
            {
                ArgumentValue = argsValue;
            }

        }

        public string GetFinalCommand(List<TextBox> variableArgs)
        {
            List<int> foundIndexes = new List<int>();
            string finalString = string.Empty;
            int variableindex = 0;
           
            string[] seperatestrings = this.SampleCommand.Split('$');
            for (int i = 0; i < seperatestrings.GetLength(0); i++)
            {
                if (i % 2 == 0)
                {
                    finalString = finalString + seperatestrings[i];
                }
                else
                {
                    finalString = finalString + variableArgs[variableindex].Text;
                    variableindex++;
                }
            }


            return finalString;
        }

        public bool ValidateCommand()
        {
            int count = SampleCommand.Split('$').Length - 1;

            if ( ((count%2)!=0) || (NoOfArg != (count / 2)) )
            {
                return false;
            }
            foreach (string args in ArgumentList)
            {
                if (string.IsNullOrWhiteSpace(args))
                    return false;
            }
            return true;
        }
        

        public void UpdateArgValues(List<TextBox> VariableArgs)
        {
            if (this.ArgumentValue == null)
                return;
            if (this.ArgumentValue.GetLength(0) == this.ArgumentList.GetLength(0))
            {
                for (int i = 0; i < VariableArgs.Count; i++)
                {
                    if (VariableArgs[i].Text != string.Empty)
                    {
                        this.ArgumentValue[i] = VariableArgs[i].Text;
                    }
                }
            }

            //check for the ArgumentValues
            if (this.ArgumentValue != null && this.ArgumentValue.GetLength(0) > 0)
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
                string CommandName = string.Empty;

                XmlNode RootNode = MyDocument.SelectSingleNode("//cmd:Appdata/cmd:Command[@Name='" + this.CommandName + "']", nsMan);

                if (RootNode != null)
                {
                    int Index = 0;
                    foreach (XmlElement childNode in RootNode.ChildNodes)
                    {
                        if (childNode.Attributes["Name"] != null)
                        {
                            if (this.ArgumentList[Index].Equals(childNode.Attributes["Name"].Value))
                            {
                                //Child Node already has Value attribute
                                if (childNode.Attributes["Value"] != null)
                                {
                                    if(!string.IsNullOrEmpty(this.ArgumentValue[Index]))
                                        childNode.Attributes["Value"].Value = this.ArgumentValue[Index];
                                    Index++;
                                }
                                else // Create the Value attribute.
                                {
                                    XmlAttribute Value = MyDocument.CreateAttribute("Value");
                                    Value.Value = this.ArgumentValue[Index];
                                    childNode.SetAttributeNode(Value);
                                    Index++;
                                }
                            }
                            else
                                return;
                        }
                        else
                            return;
                    }
                    MyDocument.Save(Constants.AppDataLocation);
                }
                else
                    return;
            }
            else
                return;
        }

       
    }
}
