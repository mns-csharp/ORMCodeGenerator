using Simple.Framework;
using System;
using System.Collections.Generic;

using System.Text;


namespace OrmCodeGeneratorApp
{
    public class sysobjects
    {
        public string name { get; set; }
    }

    public class columns
    {
        public string column_name { get; set; }
        public bool is_nullable { get; set; }
        public string data_type { get; set; }
        public int? character_maximum_length { get; set; }
    }

    public class foreign_key
    {
        public string FkName { get; set; }
        public string Schema { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string ForeignTableName { get; set; }
        public string ForeignColumnName { get; set; }
        public string DataType { get; set; }
        public int DataTypeMaxLength { get; set; }
        public int DataTypePrecision { get; set; }
        public string Scale { get; set; }
        public bool IsNullable { get; set; }
        public bool IsAnsiPadded { get; set; }
    }

    public enum NodeTypeEnum 
    {
        Server,
        Database,
        Table,
        Column
    }

    public class TreeNodeTag
    {
        public NodeTypeEnum NodeType { get; set; }
        public Object Object { get; set; }
    }
}
