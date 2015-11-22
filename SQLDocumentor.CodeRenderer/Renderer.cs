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

namespace SQLDocumentor.CodeRenderer
{
    public class Renderer : IRenderer
    {
        private RazorEngine<RazorTemplateBase> _host;

        public Renderer()
        {
            _host = RazorEngineFactory<RazorTemplateBase>.CreateRazorHost();
        }

        public void Render(Schema schema)
        {
            RenderTemplate(schema, Resources.Model, "models.html");
            RenderTemplate(schema, Resources.Repository, "repositories.html");
        }

        private void RenderTemplate(Schema schema, string template, string outputName)
        {
            var document = _host.RenderTemplate(
                                template,
                                new string[] { "SQLDocumentor.Model.dll", "SQLDocumentor.CodeRenderer.dll" },
                                schema);

            using (var sw = new StreamWriter(outputName))
            {
                if (string.IsNullOrEmpty(_host.ErrorMessage))
                    sw.Write(document);
                else
                    sw.Write(string.Format("<pre>{0}</pre>", _host.ErrorMessage));

                sw.Close();
            }

            Process.Start(outputName);
        }

        #region SQLDocumentor.Interfaces.IRenderer Stuff

        public string Name
        {
            get { return "Code Renderer"; }
        }

        public string Description
        {
            get { return "Render basic POCO classes and ADO Repository."; }
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
