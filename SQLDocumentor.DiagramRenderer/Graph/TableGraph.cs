using System;
using QuickGraph;

namespace SQLDocumentor.DiagramRenderer.Graph
{
    public class TableGraph: BidirectionalGraph<object, TableEdge>
    {
        public TableGraph() { }

        public TableGraph(bool allowParallelEdges)
            : base(allowParallelEdges) { }

        public TableGraph(bool allowParallelEdges, int vertexCapacity)
            : base(allowParallelEdges, vertexCapacity) { }
    }
}
