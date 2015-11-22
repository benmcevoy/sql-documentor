using System;
using System.Collections.Generic;

namespace SQLDocumentor.Model
{
    [Serializable]
    
    public abstract class DatabaseObject
    {
        public DatabaseObject(string type)
        {
            Parameters = new List<Parameter>();
            Type = type;
        }

        public string Summary { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Source { get; set; }
        public IEnumerable<Parameter> Parameters { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
