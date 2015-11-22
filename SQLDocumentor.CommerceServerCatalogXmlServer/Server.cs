using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using SQLDocumentor.Interfaces;
using SQLDocumentor.Model;

namespace SQLDocumentor.CommerceServerCatalogXmlServer
{
    public class Server : IServer
    {
        public Schema GetDatabaseSchema()
        {
            return new Schema { Tables = GetTables() };
        }

        private IEnumerable<Table> GetTables()
        {
            var tables = new List<Table>();
            var xml = XDocument.Parse(File.ReadAllText("catalog.xml"));
            var catalog = GetCatalog(xml);

            tables.AddRange(catalog.Select(ToTable));

            return tables;
        }

        private IEnumerable<ProductDefinition> GetCatalog(XDocument xml)
        {
            return xml.Element("MSCommerceCatalogCollection2")
                .Element("CatalogSchema")
                .Elements("Definition")
                .Select(x => new ProductDefinition
            {
                Name = x.Attribute("name").Value,
                Description = x.Attribute("description").Value,
                DefinitionType = x.Attribute("DefinitionType").Value,
                ProductProperties = x.Elements().Select(ToProductProperty)

            });
        }

        private ProductProperty ToProductProperty(XElement element)
        {
            return new ProductProperty
            {
                Name = element.Value,
                IsVariant = element.Name.ToString().Equals("DefVariantProperty")
            };
        }

        private static Table ToTable(ProductDefinition productDefinition)
        {
            var t = new Table
            {
                Name = productDefinition.Name,
                Summary = productDefinition.Description,
                Type = productDefinition.DefinitionType
            };

            t.Parameters = productDefinition.ProductProperties.Select(p => new Parameter
            {
                Parent = t,
                Name = p.Name + (p.IsVariant ? " (Variant)" : "")
            });

            return t;
        }

        public IEnumerable<string> GetDatabases()
        {
            return new[] { "Catalog" };
        }

        public string GetServerConnectionString()
        {
            return string.Empty;
        }

        public string GetDatabaseConnectionString()
        {
            return string.Empty;
        }

        public string Name { get { return "Commerce Server Catalog Schema"; } }
        public string Description { get { return "Expose Commerce Server Catalog schemas as data sources"; } }
        public string Query { get; set; }
        public string ServerName { get; set; }
        public string DatabaseName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool UseIntegratedSecurity { get; set; }
    }
}
