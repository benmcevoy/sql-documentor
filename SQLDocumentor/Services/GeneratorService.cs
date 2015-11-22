using System;
using SQLDocumentor.Interfaces;

namespace SQLDocumentor.Services
{
    public class GeneratorService
    {
        IRenderer _renderer;
        IServer _server;

        public IServer Server { get { return _server; } }
        public IRenderer Renderer { get { return _renderer; } }

        public GeneratorService(IServer server, IRenderer renderer)
        {
            _server = server;
            _renderer = renderer;
        }

        public void Generate()
        {
            _renderer.Render(_server.GetDatabaseSchema());
        }
    }
}
