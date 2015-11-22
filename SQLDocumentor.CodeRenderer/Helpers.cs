using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLDocumentor.Model;

namespace SQLDocumentor.CodeRenderer
{
    public static class Helpers
    {
        public static string AsDotNetType(this string dataType)
        {
            switch (dataType)
            {
                case "nvarchar":
                case "nchar":
                case "varchar":
                case "char":
                case "text":
                case "ntext":
                    return DotNetType.String;
                case "int":
                    return DotNetType.Int32;
                case "bit":
                    return DotNetType.Boolean;
                case "uniqueidentifier":
                    return DotNetType.Guid;
                case "datetime":
                case "datetime2":
                case "date":
                case "time":
                    return DotNetType.DateTime;
            }

            return "object";
        }

        private static string _breakCharacters = @" _-,.:;'[]{}|\\/?!@#$%^&*()<>""=+";

        public static string Pascalize(this string name)
        {
            var result = new StringBuilder(name.Length);
            var isUpperCase = true;

            foreach(var character in name)
            {
                if(_breakCharacters.Contains(character))
                {
                    isUpperCase = true;
                    continue;
                }

                if (isUpperCase)
                {
                    result.Append(character.ToString().ToUpper());
                    isUpperCase = false;
                }
                else
                {
                    result.Append(character);
                }
            }

            return result.ToString();
        }

        public static string Pluralize(this string name)
        {
            if (name.EndsWith("s"))
                return name;

            return name + "s";
        }

        public static string Privatize(this string name)
        {
            return string.Format("_{0}{1}", name.ToLower()[0], name.Substring(1));
        }

        private static Parameter GetPK(Table table)
        {
            foreach (var col in table.Parameters)
            {
                if (col.IsPrimaryKey)
                {
                    return col;
                }
            }

            return null;
        }

        public static string GetPKType(Table table)
        {
            var pk = GetPK(table);
            if (pk != null)
            {
                return pk.Datatype.AsDotNetType();
            }

            return DotNetType.Object;
        }

        public static string GetPKName(Table table)
        {
            var pk = GetPK(table);
            if (pk != null)
            {
                return pk.Name.Pascalize();
            }

            return "";
        }
    }
}
