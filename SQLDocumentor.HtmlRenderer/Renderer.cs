using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SQLDocumentor.Interfaces;
using SQLDocumentor.Model;
using System.Linq;
using System.Diagnostics;

namespace SQLDocumentor.HtmlRenderer
{
    public class Renderer : IRenderer
    {
        // TODO: possibly use Razor or NVelocity or something to make this maintainable
        // save the output in a folder, prompt to save?
        // add in styles, icons
        // include constraints, defaults, triggers etc
        // support IRenderOptions

        public void Render(Schema schema)
        {
            BuildDocument(schema);
        }

        public string Name
        {
            get { return "HTML Document Renderer"; }
        }

        public string Description
        {
            get { return "Render an HTML document.  Document is generated to the bin called detail.html"; }
        }

        private void BuildDocument(Schema schema)
        {
            var document = new StringBuilder(@"<html><head><title>SQL Documentor</title><link rel=""stylesheet"" type=""text/css"" href=""site.css"" /></head><body>");

            document.Append(AddSchema(schema, "Database"));

            document.Append(AddTopNav());

            document.Append(AddSection(schema.Tables, "Tables"));
            document.Append(AddSection(schema.Views, "Views"));
            document.Append(AddSection(schema.Procedures, "Procedures"));
            document.Append(AddSection(schema.Functions, "Functions"));

            document.Append("</body></html>");

            using (var sw = new StreamWriter("detail.html"))
            {
                sw.Write(document);
                sw.Close();
            }

            Process.Start("detail.html");
            
        }

        private string AddSchema(Schema schema, string title)
        {
            var document = new StringBuilder();

            document.Append(@"<div class=""schema"">");
            document.Append(@"<div class=""sectionObject"">");
            document.AppendFormat(@"<h1><span class=""objectType"">{0}</span> - <span class=""objectName"">{1}</span></h1>", title, schema.Name);
            document.AppendFormat(@"<div class=""objectSummary""><pre>{0}</pre></div>", schema.Summary);
            document.Append(AddObjectParameters(schema.Parameters));
            document.Append("</div>");
            document.Append("</div>");

            return document.ToString();
        }

        private string AddTopNav()
        {
            var document = new StringBuilder();

            document.Append(@"<div id=""types"">");
            document.Append(@"<h2><a name=""Types"">Types</a></h2>");
            document.Append(@"<ul>");
            document.Append(@"<li><a href=""#Tables"">Tables</a></li>");
            document.Append(@"<li><a href=""#Views"">Views</a></li>");
            document.Append(@"<li><a href=""#Procedures"">Procedures</a></li>");
            document.Append(@"<li><a href=""#Functions"">Functions</a></li>");
            document.Append(@"</ul>");
            document.Append(@"</div>");

            return document.ToString();
        }

        private string AddSection(IEnumerable<DatabaseObject> items, string title)
        {
            var document = new StringBuilder();

            document.Append(@"<div class=""section"">");
            document.AppendFormat(@"<h2><a name=""{0}"" href=""#Types""><span class=""sectionTitle"">{0}</span></a></h2>", title);
            document.Append(AddSectionObjectLinks(items));
            document.Append(AddSectionObjects(items));
            document.Append("</div>");

            return document.ToString();
        }

        private string AddSectionObjectLinks(IEnumerable<DatabaseObject> items)
        {
            var document = new StringBuilder();

            document.Append(@"<ul>");

            foreach (var o in items)
            {
                document.AppendFormat(@"<li><a href=""#{0}"">{1}</a></li>", MakeName(o.Type, o.Name), o.Name);
            }

            document.Append(@"</ul>");


            return document.ToString();
        }

        private string AddSectionObjects(IEnumerable<DatabaseObject> items)
        {
            var document = new StringBuilder();

            foreach (var o in items)
            {
                document.Append(@"<div class=""sectionObject"">");
                document.AppendFormat(@"<a name=""{0}""></a>", MakeName(o.Type, o.Name));
                document.AppendFormat(@"<h3><a href=""#{0}""><span class=""objectType"">{0}</span></a> - <span class=""objectName"">{1}</span></h3>", o.Type, o.Name);
                document.AppendFormat(@"<div class=""objectSummary""><pre>{0}</pre></div>", o.Summary);
                document.Append(AddObjectParameters(o.Parameters));
                if (!string.IsNullOrEmpty(o.Source))
                {
                    document.AppendFormat(@"<div class=""objectSource""><pre><code>{0}</code></pre></div>", o.Source);
                }
                document.Append("</div>");
            }

            return document.ToString();
        }

        private string AddObjectParameters(IEnumerable<Parameter> items)
        {
            if (!items.Any())
            {
                return "";
            }

            var document = new StringBuilder();

            document.Append(@"
<div class=""objectParameters"">
<table><thead>
<tr>
<th>Key</th><th>Name</th><th>Summary</th><th>Datatype</th><th>Size</th><th>Required?</th>
</tr>
</thead><tbody>");

            foreach (var o in items)
            {
                document.AppendFormat(@"<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td></tr>",
                    ToKey(o.IsPrimaryKey, o.IsForeignKey, o.ForeignKeyTable), o.Name, o.Summary, o.Datatype, ToSize(o.Size), ToRequired(o.IsNullable));
            }

            document.Append(@"</tbody></table></div>");

            return document.ToString();
        }

        private object ToRequired(bool isNullable)
        {
            if (!isNullable)
            {
                return "Required";
            }
            return "";
        }

        private string ToSize(int size)
        {
            if (size == 0)
            {
                return "";
            }

            return size.ToString();
        }

        private string ToKey(bool pk, bool fk, Table referencedTable)
        {
            var result = "";
            if (pk)
            {
                result = "PK ";
            }

            if (fk)
            {
                result += string.Format(@"<a href=""#{0}"" title=""{1}"">FK ({1})</a>", MakeName(referencedTable.Type, referencedTable.Name), referencedTable.Name);
            }

            return result;
        }

        private string MakeName(params string[] names)
        {
            var sb = new StringBuilder();

            foreach (var item in names)
            {
                // TOOD: this will brak for dodgy table names
                // sould probably use a unique Id (e.g. sys.objects.object_id)
                sb.AppendFormat("{0}_", item.Replace(" ", "_"));
            }

            return sb.ToString();
        }

        private string AddSectionItems(IEnumerable<DatabaseObject> items)
        {
            var document = new StringBuilder();

            foreach (var item in items)
            {
                document.AppendFormat(@"<li class=""{0}"">{1}</li>", item.Type, item.Name);
            }

            return document.ToString();
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
