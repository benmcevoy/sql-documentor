using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLDocumentor.Model;

namespace SQLDocumentor.RazorRenderer
{
    public static class Helpers
    {
        public static string ToRequired(bool isNullable)
        {
            if (!isNullable)
            {
                return "Required";
            }
            return "";
        }

        public static string ToSize(int size)
        {
            if (size == 0)
            {
                return "";
            }

            return size.ToString();
        }

        public static string ToKey(bool pk, bool fk, Table referencedTable)
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

        public static string MakeName(params string[] names)
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
    }
}
