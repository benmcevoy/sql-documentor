using SQLDocumentor.Interfaces;

namespace SQLDocumentor.Services
{
    public interface IGeneratorService
    {
        IRenderer Renderer { get; }
        IServer Server { get; }

        void Generate();
    }
}