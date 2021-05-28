using System;
using System.Collections.Generic;
using System.Text;

namespace OrmCodeGeneratorApp
{
    public class ParameterGenerator
    {
        private Column PrimaryKey;
        private Dictionary<string, Column> Columns;

        public ParameterGenerator(Column PK, Dictionary<string, Column> dictionary)
        {
            PrimaryKey = PK;
            Columns = new Dictionary<string, Column>(dictionary);
        }

        public string GetAssignments()
        {
            StringBuilder sb = new StringBuilder();
            if (PrimaryKey != null)
            {
                sb.Append("item." + PrimaryKey.Name + " = dataReader." + TypeConverter.ToFunctionName(PrimaryKey) + "(\"" + PrimaryKey.Name + "\");\r\n");
            }
            int i = 0;
            foreach (Column c in Columns.Values)
            {
                sb.Append("\t\t\t\t\t");
                sb.Append("item." + c.Name + " = " + TypeConverter.ToCast(c) + "dataReader." + TypeConverter.ToFunctionName(c) + "(\"" + c.Name + "\");\r\n");
                ++i;
            }

            return sb.ToString();
        }

        public string GetParameters()
        {
            StringBuilder sb = new StringBuilder();

            int i = 0;
            foreach (Column c in Columns.Values)
            {
                if (i > 0)
                {
                    sb.Append("\t\t\t\t");
                }
                sb.Append("queryExecutor.AddParameter(item." + c.Name + ", DbType." + TypeConverter.ToDbType(c.DataType).ToString() + ");");

                if(i < Columns.Values.Count-1)
                {
                    sb.Append("\r\n");
                }
                ++i;
            }

            return sb.ToString();
        }

        public string GetPrimaryKey()
        {
            StringBuilder sb = new StringBuilder();

            if (PrimaryKey != null)
            {
                sb.Append("queryExecutor.AddParameter(item." + PrimaryKey.Name + ", DbType.Int32);");
            }

            return sb.ToString();
        }
    }
}
