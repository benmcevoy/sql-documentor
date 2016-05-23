using SQLDocumentor.Interfaces;

namespace SQLDocumentor.Services
{
    public class GeneratorService : IGeneratorService
    {
        public GeneratorService(IServer server, IRenderer renderer)
        {
            Server = server;
            Renderer = renderer;
        }

        public void Generate()
        {
            Renderer.Render(Server.GetSchema());
        }

        public IServer Server { get; }
        public IRenderer Renderer { get; }
    }
}
