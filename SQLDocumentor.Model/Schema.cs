using System;
using System.Collections.Generic;

namespace SQLDocumentor.Model
{
    [Serializable]
    public class Schema : DatabaseObject
    {
        // TODO: Make serializeable?
        // include constraints, defaults, triggers other stuff


        public Schema()
            : base("Schema")
        {
            Tables = new List<Table>();
            Views = new List<View>();
            Procedures = new List<Procedure>();
            Functions = new List<Function>();
        }

        public IEnumerable<Table> Tables { get; set; }
        public IEnumerable<View> Views { get; set; }
        public IEnumerable<Procedure> Procedures { get; set; }
        public IEnumerable<Function> Functions { get; set; }
    }
}

