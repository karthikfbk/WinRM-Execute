using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleApp
{
    class Constants
    {
        public static readonly string AppDataLocation =  AppDomain.CurrentDomain.BaseDirectory + "AppData.xml";
        public static readonly string WinRmOutputLocation = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Output.txt";        
    }
}
