using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLDocumentor.Interfaces;
using SQLDocumentor.Model;
using RazorHosting;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Web.Razor;

namespace SQLDocumentor.RazorRenderer
{
    public class Renderer : IRenderer
    {
        private RazorEngine<RazorTemplateBase> _host;

        public void Render(Schema schema)
        {
            _host = RazorEngineFactory<RazorTemplateBase>.CreateRazorHost();

            var document = _host.RenderTemplate(
                    Resources.DocumentTemplate,
                    new string[] { "SQLDocumentor.Model.dll", "SQLDocumentor.RazorRenderer.dll" },
                    schema);

            using (var sw = new StreamWriter("detail_razor.html"))
            {
                sw.Write(document);
                sw.Close();
            }

            Process.Start("detail_razor.html");
        }

        #region SQLDocumentor.Interfaces.IRenderer Stuff

        public string Name
        {
            get { return "Razor Renderer"; }
        }

        public string Description
        {
            get { return "Render HTML documentation via the Razor rendering engine."; }
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

        #endregion
    }
}
