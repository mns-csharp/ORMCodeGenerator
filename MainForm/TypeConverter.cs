using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace OrmCodeGeneratorApp
{
    public static class TypeConverter
    {
        #region SqlToDotNetType
        public static Type SqlToDotNetType(string str)
        {
            Type type = null;

            switch (str)
            {
                case "binary":
                case "binary(1)":
                case "image":
                case "rowversion":
                case "timestamp":
                case "varbinary":
                case "varbinary(1)":
                    type = typeof(Byte[]);
                    break;
                case "char":
                    type = typeof(char);
                    break;
                case "varchar":
                case "nvarchar":
                case "nvarchar(1)":
                case "nchar":
                case "nchar(1)":
                case "text":
                case "ntext":
                    type = typeof(string);
                    break;
                case "uniqueidentifier":
                    type = typeof(Guid);
                    break;

                case "bit":
                    type = typeof(Boolean);
                    break;
                case "tinyint":
                    type = typeof(Byte);
                    break;
                case "smallint":
                    type = typeof(Int16);
                    break;
                case "int":
                    type = typeof(Int32);
                    break;
                case "bigint":
                    type = typeof(Int64);
                    break;
                case "smallmoney":
                    type = typeof(Decimal);
                    break;
                case "money":
                    type = typeof(Decimal);
                    break;
                case "numeric":
                    type = typeof(Decimal);
                    break;
                case "decimal":
                    type = typeof(Decimal);
                    break;
                case "real":
                    type = typeof(Single);
                    break;
                case "float":
                    type = typeof(Double);
                    break;
                case "smalldatetime":
                    type = typeof(DateTime);
                    break;
                case "datetime":
                    type = typeof(DateTime);
                    break;
                case "sql_variant":
                    type = typeof(Object);
                    break;
                //User-defined type(UDT)        user-defined type     
                //table                         None                          None 
                //cursor                        None                          None
                //timestamp                     None                          None 
                //xml                           SqlXml                        None
            }

            return type;
        } 
        #endregion

        #region ToDotNetKeyword
        public static string ToDotNetKeyword(Type type)
        {
            string typeName = type.Name;

            string ret = string.Empty;
            switch (typeName)
            {
                case "Boolean":
                    ret = "bool";
                    break;
                case "Byte":
                    ret = "byte";
                    break;
                case "Byte[]":
                    ret = "byte[]";
                    break;
                case "SByte":
                    ret = "sbyte";
                    break;
                case "Char":
                    ret = "char";
                    break;
                case "Decimal":
                    ret = "decimal";
                    break;
                case "Double":
                    ret = "double";
                    break;
                case "Single":
                    ret = "single";
                    break;
                case "Int32":
                    ret = "int";
                    break;
                case "UInt32":
                    ret = "uint";
                    break;
                case "Int64":
                    ret = "long";
                    break;
                case "UInt64":
                    ret = "ulong";
                    break;
                case "Object":
                default:
                    ret = "object";
                    break;
                case "Short":
                    ret = "Int16";
                    break;
                case "UShort":
                    ret = "UInt16";
                    break;
                case "String":
                    ret = "string";
                    break;
            }

            

            return ret;
        } 
        #endregion

        #region ToFunctionName
        public static string ToFunctionName(Column col)
        {
            //StringBuilder sb = new StringBuilder();
            Type type = col.DataType;
            string typeName = type.Name;

            //sb.Append(typeName);

            

            string gets = string.Empty;

            switch (typeName)
            {
                case "Boolean":
                    gets = "GetBoolean";
                    break;
                case "Byte":
                    gets = "GetByte";
                    break;
                case "Byte[]":
                    gets = "GetByteArray";
                    break;
                case "SByte":
                    gets = "GetSByte";
                    break;
                case "Char":
                    gets = "GetChar";
                    break;
                case "Decimal":
                    gets = "GetDecimal";
                    break;
                case "Double":
                    gets = "GetDouble";
                    break;
                case "Float":
                    gets = "GetFloat";
                    break;
                case "DateTime":
                    gets = "GetDateTime";
                    break;
                case "Int16":
                    gets = "GetInt16";
                    break;
                case "Int32":
                    gets = "GetInt32";
                    break;
                case "Int64":
                    gets = "GetInt64";
                    break;
                case "Guid":
                    gets = "GetGuid";
                    break;
                case "String":
                    gets = "GetString";
                    break;
                case "Image":
                    gets = "GetImage";
                    break;
                case "Object":
                    gets = "GetObject";
                    break;
            }

            return gets;
        } 
        #endregion

        #region ToDbType
        public static DbType ToDbType(Type type)
        {
            DbType dbType = DbType.Object;

            string sType = type.Name;
            switch (sType)
            {
                case "Byte":
                    dbType = DbType.Byte;
                    break;
                case "SByte":
                    dbType = DbType.SByte;
                    break;
                case "Int16":
                    dbType = DbType.Int16;
                    break;
                case "UInt16":
                    dbType = DbType.UInt16;
                    break;
                case "Int32":
                    dbType = DbType.Int32;
                    break;
                case "UInt32":
                    dbType = DbType.UInt32;
                    break;
                case "Int64":
                    dbType = DbType.Int64;
                    break;
                case "UInt64":
                    dbType = DbType.UInt64;
                    break;
                case "Single":
                    dbType = DbType.Single;
                    break;
                case "Double":
                    dbType = DbType.Double;
                    break;
                case "Decimal":
                    dbType = DbType.Decimal;
                    break;
                case "Boolean":
                    dbType = DbType.Boolean;
                    break;
                case "String":
                    dbType = DbType.String;
                    break;
                case "Char":
                    dbType = DbType.StringFixedLength;
                    break;
                case "Guid":
                    dbType = DbType.Guid;
                    break;
                case "DateTime":
                    dbType = DbType.DateTime;
                    break;
                case "DateTimeOffset":
                    dbType = DbType.DateTimeOffset;
                    break;
                case "Byte[]":
                    dbType = DbType.Binary;
                    break;
            }

            return dbType;
        } 
        #endregion

        public static string ToCast(Column c)
        {
            //if(c.IsNullable)
            {
                string typeName = c.DataType.Name;
                if (typeName != "String" && typeName != "Byte[]" && typeName != "Image" && typeName != "Object")
                {
                    return "(" + ToDotNetKeyword(c.DataType) + ")";
                }
            }

            return string.Empty;
        }

        public static string ToControl(Column c)
        {
            string controlType = string.Empty;
            string sType = c.DataType.Name;
            switch (sType)
            {
                case "Byte":
                    controlType = "NumericUpDown";
                    break;
                case "SByte":
                    controlType = "NumericUpDown";
                    break;
                case "Int16":
                    controlType = "NumericUpDown";
                    break;
                case "UInt16":
                    controlType = "NumericUpDown";
                    break;
                case "Int32":
                    controlType = "NumericUpDown";
                    break;
                case "UInt32":
                    controlType = "NumericUpDown";
                    break;
                case "Int64":
                    controlType = "NumericUpDown";
                    break;
                case "UInt64":
                    controlType = "NumericUpDown";
                    break;
                case "Single":
                    controlType = "NumericUpDown";
                    break;
                case "Double":
                    controlType = "NumericUpDown";
                    break;
                case "Decimal":
                    controlType = "NumericUpDown";
                    break;
                case "Boolean":
                    controlType = "CheckBox";
                    break;
                case "String":
                    controlType = "TextBox";
                    break;
                case "Char":
                    controlType = "TextBox";
                    break;
                case "Guid":
                    controlType = "TextBox";
                    break;
                case "DateTime":
                    controlType = "DateTimePicker";
                    break;
                case "DateTimeOffset":
                    controlType = "DateTimePicker";
                    break;
                case "Byte[]":
                    controlType = "PictureBox";
                    break;
            }

            return controlType;
        }

        public static string ToControlValue(string controlType)
        {
            string property = string.Empty;

            switch (controlType)
            {
                case "NumericUpDown":
                    property = "Value";
                    break;
                case "CheckBox":
                    property = "Checked";
                    break;
                case "TextBox":
                    property = "Text";
                    break;
                case "DateTimePicker":
                    property = "Value";
                    break;
                case "PictureBox":
                    property = "Image";
                    break;
            }

            return property;
        }

        public static string ToControlClearValue(string controlType)
        {
            string property = string.Empty;

            switch (controlType)
            {
                case "NumericUpDown":
                    property = "0";
                    break;
                case "CheckBox":
                    property = "false";
                    break;
                case "TextBox":
                    property = "string.Empty";
                    break;
                case "DateTimePicker":
                    property = "DateTime.Now";
                    break;
                case "PictureBox":
                    property = "null";
                    break;
            }

            return property;
        }
    }
}
