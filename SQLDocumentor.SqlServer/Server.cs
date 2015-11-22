using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Xml.Linq;
using SQLDocumentor.Interfaces;
using SQLDocumentor.Model;
using SQLDocumentor.SqlServer.Properties;

namespace SQLDocumentor.SqlServer
{
    public class Server : IServer
    {
        private readonly string DatabaseType = "Database";
        private readonly string DefaultConstraintType = "Default or DEFAULT constraint";
        private readonly string ForeignKeyType = "FOREIGN KEY constraint";
        private readonly string FunctionType = "Function";
        private readonly string PrimaryKeyType = "PRIMARY KEY constraint";
        private readonly string ProcedureType = "Procedure";
        private readonly string TableType = "Table";
        private readonly string TableFunctionType = "Table function";
        private readonly string TriggerType = "Trigger";
        private readonly string UniqueConstraintType = "UNIQUE constraint";
        private readonly string ViewType = "View";

        public string Name { get { return "SQL Server";  } }
        public string Description { get { return "Expose SQL database schemas as data sources"; } }
        public string Query { get; set; }
        public string ServerName { get; set; }
        public string DatabaseName { get; set; }
        public string UserName { get; set; } // TODO: probably should not have these properties
        public string Password { get; set; } // TODO: probably should not have these properties
        public bool UseIntegratedSecurity { get; set; }

        private Schema _schema;

        public Schema GetDatabaseSchema()
        {
            _schema = new Schema();

            using (var conn = new SqlConnection(GetDatabaseConnectionString()))
            {
                conn.Open();

                using (var xmlCmd = conn.CreateCommand())
                {
                    // Set the text of the command 
                    xmlCmd.CommandText = Resources.query;
                    xmlCmd.Connection = conn;
                    xmlCmd.CommandType = CommandType.Text;

                    try
                    {
                        var xml = xmlCmd.ExecuteXmlReader();
                        if (xml.Read())
                        {
                            var xdoc = XDocument.Load(xml);
                            HydrateSchema(xdoc);
                        }
                    }
                    catch (Exception ex)
                    {
                        _schema = new Schema() { Name = "There was an error analyzing the schema", Summary = ex.ToString() };
                    }
                }

                conn.Close();
            }

            return _schema;
        }



        private Schema HydrateSchema(XDocument xdoc)
        {

            // query reutrns xml like:
            /**

        <database type="Database" name="Coles.SFS" summary="bunch of text here" server="TOWER-PC">
            <objects type="Database">
                <object type="Database" name="Coles.SFS" summary="bunch o text here">
                    <param name="Size" datatype="" summary="     38.56 MB" isPK="0" isFK="0" />
                    <param name="Owner" datatype="" summary="sa" isPK="0" isFK="0" />
                    <param name="Created" datatype="" summary="May 19 2011" isPK="0" isFK="0" />
                </object>
             * 
             *  and more objects, each with params
           </objects>
                           
              etc for each type
             **/

            _schema = (from o in xdoc.Elements("database")
                          where o.Attribute("type").Value == DatabaseType
                          select new Schema()
                             {
                                 Name = o.Attribute("name").Value,
                                 Summary = o.Attribute("summary").Value,
                             }).FirstOrDefault();

            _schema.Tables = GetSchemaType<Table>(xdoc, TableType);
            _schema.Views = GetSchemaType<View>(xdoc, ViewType);
            _schema.Procedures = GetSchemaType<Procedure>(xdoc, ProcedureType);
            _schema.Functions = GetSchemaType<Function>(xdoc, FunctionType);

            EnsureFKTables();

            return _schema;
        }

        private IEnumerable<T> GetSchemaType<T>(XDocument xdoc, string typeName) where T : DatabaseObject, new()
        {
            return (from o in xdoc.Element("database").Elements("objects").Elements("object")
                    where o.Attribute("type").Value == typeName
                     select GetDatabaseObject<T>(o)).ToArray();
        }

        private T GetDatabaseObject<T>(XElement element) where T : DatabaseObject, new()
        {
            var d = new T();

            d.Name = element.Attribute("name") != null ? element.Attribute("name").Value : "";
            d.Summary = element.Attribute("summary") != null ? element.Attribute("summary").Value : "";
            d.Parameters = GetObjectParameters(element.Elements("param"), d);
            d.Source = element.Attribute("SQL") != null ? element.Attribute("SQL").Value : "";
            return d;
        }

        private IEnumerable<Parameter> GetObjectParameters(IEnumerable<XElement> parameters, DatabaseObject parent)
        {
            return (from p in parameters
                    select new Parameter()
                    {
                        Datatype = p.Attribute("datatype") != null ? p.Attribute("datatype").Value : "",
                        Name = p.Attribute("name") != null ? p.Attribute("name").Value : "",
                        Summary = p.Attribute("summary") != null ? p.Attribute("summary").Value : "",
                        IsNullable = ToBoolean(p.Attribute("isNullable")),
                        Size = ToInt(p.Attribute("size")),
                        IsPrimaryKey = ToBoolean(p.Attribute("isPK")),
                        IsForeignKey = ToBoolean(p.Attribute("isFK")),
                        Parent = parent,
                        ForeignKeyTableName = p.Attribute("fKTableName") != null ? p.Attribute("fKTableName").Value : "",
                    }).ToArray(); 
        }

        private void EnsureFKTables()
        {
            var fkColumns = (from t in _schema.Tables
                      from p in t.Parameters
                      where p.IsForeignKey
                      select p).ToArray();

            foreach (var column in fkColumns)
            {
                column.ForeignKeyTable = (from t in _schema.Tables where t.Name == column.ForeignKeyTableName select t).FirstOrDefault();
            }
        }

        private int ToInt(XAttribute item)
        {
            if (item == null)
            {
                return 0;
            }

            return Convert.ToInt32(item.Value);
        }

        private bool ToBoolean(XAttribute item)
        {
            if (item == null)
            {
                return false;
            }

            return (item.Value == "1");
        }

        public IEnumerable<string> GetDatabases()
        {
            var databases = new List<string>();

            string connectionString = GetServerConnectionString();

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (var sqlCmd = conn.CreateCommand())
                {
                    // Set the text of the command 
                    sqlCmd.CommandText = "exec sp_databases";
                    sqlCmd.Connection = conn;
                    sqlCmd.CommandType = CommandType.Text;

                    SqlDataReader dr = sqlCmd.ExecuteReader();

                    while (dr.Read())
                    {
                        databases.Add(dr["DATABASE_NAME"].ToString());
                    }
                }

                conn.Close();
            }

            return databases;
        }

        public string GetServerConnectionString()
        {
            if (UseIntegratedSecurity)
            {
                return string.Format("data source={0};Integrated Security=SSPI;", ServerName);
            }

            return string.Format("data source={0};User Id={2};Password={3};", ServerName, UserName, Password);
        }

        public string GetDatabaseConnectionString()
        {
            return string.Format("{0}database={1};", GetServerConnectionString(), DatabaseName);
        }

    }
}
