using System.Collections.Generic;

namespace SQLDocumentor.CommerceServerCatalogXmlServer
{
    internal class ProductDefinition
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string DefinitionType { get; set; }

        public IEnumerable<ProductProperty> ProductProperties { get; set; }
    }

    internal class ProductProperty
    {
        public string Name { get; set; }
        public bool IsVariant { get; set; }
    }
}
