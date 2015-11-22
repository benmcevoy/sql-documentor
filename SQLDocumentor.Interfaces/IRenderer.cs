using System;
using SQLDocumentor.Model;
using System.Collections.Generic;

namespace SQLDocumentor.Interfaces
{
    public interface IRenderer
    {
        void Render(Schema schema);

        Dictionary<string, object> RenderOptions { get; set; }

        string Name { get; }
        string Description { get; }
    }
}

