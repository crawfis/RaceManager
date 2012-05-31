using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HardCard.Scoring.Core
{
    public class Config
    {
        public int SleepTime = 1000;
        public int PortNumber = 3900;
        //public String UserKeyString = "default";
        public string DataDirectory = "C://Hardcard//";
        
        private static Config instance;

        public static Config Instance
        {
            get
            {
                if (instance == null)
                    instance = new Config();
                return instance;
            }
        }

        private Config()
        {
        }
    }
}
