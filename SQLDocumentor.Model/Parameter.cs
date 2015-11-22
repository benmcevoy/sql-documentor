using System;

namespace SQLDocumentor.Model
{
    [Serializable]
    public class Parameter
    {
        public string Summary { get; set; }
        public string Name { get; set; }
        public bool IsNullable { get; set; }
        public string Datatype { get; set; }    // TODO: should probably be strongly typed or string and some converter
        public int Size { get; set; } // TODO: is this always int? what about decimal, or MAX?  probably string and some converter
        public bool IsPrimaryKey { get; set; } // TODO: probably raise this up to it's own type
        public bool IsForeignKey { get; set; } // TODO: probably raise this up to it's own type
        public Table ForeignKeyTable { get; set; }
        public string ForeignKeyTableName { get; set; }
        public DatabaseObject Parent { get; set; }

        public override string ToString()
        {
            if (IsPrimaryKey && IsForeignKey)
            {
                return string.Format("{0} (PK, FK)", Name);
            }

            if (IsPrimaryKey)
            {
                return string.Format("{0} (PK)", Name);
            }

            if (IsForeignKey)
            {
                return string.Format("{0} (FK)", Name);
            }

            return Name;
        }
    }
}
