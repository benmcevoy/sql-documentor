using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using SQLDocumentor.Interfaces;
using SQLDocumentor.Model;

namespace SQLDocumentor.AssemblyEntityRelationshipServer
{
    public class Server : IServer
    {
        public Schema GetSchema()
        {
            return new Schema()
            {
                Tables = GetTables()
            };
        }

        private IEnumerable<Table> GetTables()
        {
            // TODO:

            // table is a class, columns is a proprty

            // foreign key is a

            var assembly = Assembly.LoadFrom(DatabaseName);
            var results = new List<Table>();

            foreach (var exportedType in assembly.GetExportedTypes())
            {
                var table = new Table
                {
                    Name = exportedType.Name,
                    Summary = exportedType.FullName
                };

                table.Parameters = exportedType.GetProperties().Select(info => new Parameter
                {
                    Name = info.Name,
                    Datatype = info.GetType().Name,
                    Parent = table,
                    Summary = info.GetType().FullName
                });

                results.Add(table);
            }

            foreach (var exportedType in assembly.GetExportedTypes())
            {
                // ahh.... inheritance is not something we model...
                var matches = new List<Tuple<Table, Table>>();
                var matchTable = results.First(table => table.Summary.Equals(exportedType.FullName));

                foreach (var i in exportedType.GetInterfaces())
                {
                    var m = results.FirstOrDefault(table => table.Summary.Equals(i.FullName));

                    if (m == null) continue;
                    if (m.Summary == matchTable.Summary) continue;

                    matches.Add(new Tuple<Table, Table>(matchTable, m));
                }

                var matchBaseType = results.FirstOrDefault(table => table.Summary.Equals(exportedType.BaseType?.FullName));
                if (matchBaseType != null) matches.Add(new Tuple<Table, Table>(matchTable, matchBaseType));

                foreach (var match in matches)
                {
                    InsertProperty(match.Item1, match.Item2);
                }
            }

            // foreach property
            // if p.GetType in tables then FK

            foreach (var p in results.SelectMany(table => table.Parameters))
            {
                var match = results.FirstOrDefault(table => table.Summary.Equals(p.Summary));

                p.IsForeignKey = match != null;
                p.ForeignKeyTable = match;
                p.ForeignKeyTableName = match?.Name;
            }

            return results;
        }

        private static void InsertProperty(Table match, Table fk)
        {
            var properties = (match.Parameters ?? Enumerable.Empty<Parameter>()).ToList();

            properties.Insert(0, new Parameter
            {
                Name = "Is A " + fk.Name,
                Datatype = fk.Name,
                Parent = match,
                Summary = fk.Summary
            });

            match.Parameters = properties;
        }

        public IEnumerable<string> GetDatabases()
        {
            return Directory
                .EnumerateFiles(ServerName)
                .Where(s => s.EndsWith(".dll") || s.EndsWith(".exe"));
        }

        public string GetServerConnectionString()
        {
            return string.Empty;
        }

        public string GetDatabaseConnectionString()
        {
            return string.Empty;
        }

        public string Name { get { return "Assembly Entity Relationship Schema"; } }
        public string Description { get { return "Expose an assembly as data sources"; } }
        public string Query { get; set; }
        public string ServerName { get; set; }
        public string DatabaseName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool UseIntegratedSecurity { get; set; }
    }
}
