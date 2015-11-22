using System;
using System.Collections.Generic;
using System.Linq;
using SQLDocumentor.DiagramRenderer.Graph;
using SQLDocumentor.Interfaces;
using SQLDocumentor.Model;

namespace SQLDocumentor.DiagramRenderer
{
    public class Renderer : IRenderer
    {
        public void Render(Schema schema)
        {
            BuildDocument(schema);
        }

        public string Name
        {
            get { return "Diagram Renderer"; }
        }

        public string Description
        {
            get { 
                return 
@"Render a diagram of the database. 
               
Thanks to QuickGraph and the Graph# libraries.
"; 
            }
        }

        private void BuildDocument(Schema schema)
        {
            var graph = GetGraph(schema.Tables);
            var window = new DiagramView();

            window.GraphToVisualize = graph;
            window.Show();
        }

        private TableGraph GetGraph(IEnumerable<Table> tables)
        {
            var columns = from t in tables from p in t.Parameters where p.IsForeignKey select p ;
            var g = new TableGraph();

            // add all the tables as a vertex
            foreach (var table in tables)
            {
                g.AddVertex(table);
            }

            foreach (var c in columns)
            {
                if (c.ForeignKeyTable == null)
                {
                    continue;
                }

                // add an edge for each FK relationship
                g.AddEdge(new TableEdge(c));
            }
            
            return g;
        }

        public Dictionary<string, object> RenderOptions
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
