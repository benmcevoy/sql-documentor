using System;
using QuickGraph;
using SQLDocumentor.Model;

namespace SQLDocumentor.DiagramRenderer.Graph
{
    public class TableEdge : Edge<Object>
    {
        public string Relationship { get; private set; }

        public TableEdge(Parameter column)
            : base(column.Parent as Table, column.ForeignKeyTable)
        {
            Relationship = string.Format("{0}_{1} - {2}", column.Parent.Name, column.Name, column.ForeignKeyTableName);
        }
    }
}
