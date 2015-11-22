using System.Collections.Generic;
using SQLDocumentor.Model;

namespace SQLDocumentor.Interfaces
{
    public interface IServer
    {
        string Name { get; }
        string Description { get; }

        string Query { get; set; }
        string ServerName { get; set; }
        string DatabaseName { get; set; }
        string UserName { get; set; } // TODO: probably should not have these properties
        string Password { get; set; } // TODO: probably should not have these properties
        bool UseIntegratedSecurity { get; set; }

        Schema GetDatabaseSchema();
        IEnumerable<string> GetDatabases();
        string GetServerConnectionString();
        string GetDatabaseConnectionString();
    }
}

