using System;
using System.Windows;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using SQLDocumentor.Interfaces;

namespace SQLDocumentor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
         private static IWindsorContainer _container;

        public App()
            : base()
        {
            InitializeWindsor();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var w = _container.Resolve<Shell>();

            w.Show();
        }

        private void InitializeWindsor()
        {
            _container = new WindsorContainer()
                .Install(FromAssembly.This());

            _container.Register(Component.For<IWindsorContainer>().Instance(_container));

            _container.Register(Component.For<IServer>().ImplementedBy<CommerceServerCatalogXmlServer.Server>());
            //_container.Register(Component.For<IServer>().ImplementedBy<SqlServer.Server>());

            _container.Register(Component.For<IRenderer>().ImplementedBy<DiagramRenderer.Renderer>());
            //_container.Register(Component.For<IRenderer>().ImplementedBy<HtmlRenderer.Renderer>());
            //_container.Register(Component.For<IRenderer>().ImplementedBy<RazorRenderer.Renderer>());
            //_container.Register(Component.For<IRenderer>().ImplementedBy<CodeRenderer.Renderer>());

            _container.Register(AllTypes.FromThisAssembly()
                   .Where(type => type.Name.EndsWith("Service"))
                   .Configure(c => c.LifeStyle.Singleton));

            _container.Register(AllTypes.FromThisAssembly()
                   .Where(type => type.Name.EndsWith("ViewModel"))
                   .Configure(c => c.LifeStyle.Transient));

            _container.Register(AllTypes.FromThisAssembly()
                    .Where(type => type.Name.EndsWith("View"))
                    .Configure(c => c.LifeStyle.Transient));

            _container.Register(Component.For<Shell>());
        }
    }
}
