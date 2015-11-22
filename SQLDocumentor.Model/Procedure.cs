using System;

namespace SQLDocumentor.Model
{
    [Serializable]
    public class Procedure : DatabaseObject
    {
        public Procedure()
            : base("Procedures")
        {
        }
    }
}
