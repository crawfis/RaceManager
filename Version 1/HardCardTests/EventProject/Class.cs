using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventProject
{
    /// <summary>
    /// This struct represents bike class, e.g. 50 cc, 60 cc, etc.
    /// </summary>
    [Serializable()]
    public class Class
    {
        public int classNumber { get; set; }
        public String name { get; set; }
        public String description { get; set; }

        public Class(String name, String description)
            : this()
        {
            this.name = name;
            this.description = description;
        }

        public Class()
        {
            this.classNumber = DataManager.getNextID();
        }
    }
}
