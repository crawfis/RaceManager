using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace EventProject
{
    public class PropertyUpdateEventArgs : EventArgs
    {        
        public string PropertyName { get; internal set; }  
        public object OldValue { get; internal set; }  
        public object NewValue { get; internal set; }  
   
        public PropertyUpdateEventArgs(string propertyName, object oldValue, object newValue)
        {  
            this.PropertyName = propertyName;  
            this.OldValue = oldValue;  
            this.NewValue = newValue;  
        }
    }
}
