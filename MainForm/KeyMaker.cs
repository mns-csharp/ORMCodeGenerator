using System;
using System.Collections.Generic;
using System.Text;

namespace OrmCodeGeneratorApp
{
    public class KeyMaker
    {
        public static string GetFullyQualifiedName(Table mapping)
        {
            StringBuilder sb = new StringBuilder(mapping.DatabaseName);
            sb.Append(".");
            sb.Append(mapping.Name);

            return sb.ToString();
        }

        public static string GetKey(Table mapping, string queryName)
        {
            return GetFullyQualifiedName(mapping) + "." + queryName;
        }

        public static string GetKey(Table mapping, string queryName, string fieldName)
        {
            return GetFullyQualifiedName(mapping) + "." + queryName + fieldName;
        }
    }
}
