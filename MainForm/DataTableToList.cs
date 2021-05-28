using System;
using System.Collections.Generic;
using System.Data;

using System.Reflection;
using System.Text;


namespace OrmCodeGeneratorApp
{
    public class SqlServerInstance
    {
        public string Name { get; set; }
        public string Server { get; set; }
        public string Instance { get; set; }
        public bool IsClustered { get; set; }
        public string Version { get; set; }
        public bool IsLocal { get; set; }
    }

    //public class Source
    //{
    //    public string SOURCES_NAME { get; set; }
    //    public string SOURCES_DESCRIPTION { get; set; }
    //    public int SOURCES_TYPE { get; set; }
    //    public bool SOURCES_ISPARENT { get; set; }
    //    public string SOURCES_CLSID { get; set; }
    //    public string SOURCES_PARSENAME { get; set; }
    //}

    public class DataTableToSourceList
    {
        public static List<T> DataTableToList<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }
        /// <summary>
        /// Converts a DataTable to a list with generic objects
        /// </summary>
        /// <typeparam name="T">Generic object</typeparam>
        /// <param name="table">DataTable</param>
        /// <returns>List with generic objects</returns>
        /*
        public static List<T> DataTableToList<T>(DataTable table) where T : class, new()
        {
            try
            {
                List<T> list = new List<T>();

                foreach (DataRow r in table.Rows)
                {
                    T obj = new T();

                    PropertyInfo [] props = obj.GetType().GetProperties();
                    int i = 0;
                    foreach (PropertyInfo p in props)
                    {
                        try
                        {
                            PropertyInfo propertyInfo = obj.GetType().GetProperty(p.Name);
                            object obj2 = r[i];
                            propertyInfo.SetValue(obj, Convert.ChangeType(obj2, propertyInfo.PropertyType), null);
                        }
                        catch
                        {
                            continue;
                        }
                        ++i;
                    }

                    list.Add(obj);
                }

                return list;
            }
            catch
            {
                return null;
            }
        }*/
    }
}
