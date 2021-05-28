using Simple.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace OrmCodeGeneratorApp
{
    public class Column
    {
        public string DatabaseName { get; set; }
        public string TableName { get; set; }
        public string Name { get; set; }
        public Type DataType { get; set; }
        public bool IsPrimaryKey { get; set; }
        public foreign_key ForeignKey { get; set; }
        public bool HasForeignKey { get; set; }
        public bool IsNullable { get; set; }
        public int? Size { get; set; }

        public Column()
        {
            ForeignKey = new foreign_key();
        }

        public Column(string name)
        {
            Name = name;
            ForeignKey = new foreign_key();
        }

        public string GetAsProperty()
        {
            StringBuilder sb = new StringBuilder("public");
            sb.Append(" " + TypeConverter.ToDotNetKeyword(DataType));

            string typeName = DataType.Name;
            if (this.IsNullable)
            {
                if (typeName != "String" && typeName != "Byte[]" && typeName != "Image" && typeName != "Object")
                {
                    sb.Append("? ");
                }
            }

            sb.Append(" " + Name);
            sb.Append(" { get; set; }");            
            return sb.ToString();
        }
    }
}
