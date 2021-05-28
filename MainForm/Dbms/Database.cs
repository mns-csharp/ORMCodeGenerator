using Simple.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace OrmCodeGeneratorApp
{
    

    public class Database
    {
        public string Name { get; set; }
        public Dictionary<string, Table> Tables { get; private set; }
        public bool IsSystemDatabase { get; set; }

        public void PopulateTables(TransactionManager tm)
        {
            // List all tables from the database


            tm.BeginTransaction();
            IQueryExecutor<sysobjects> queryExecutor = new QueryExecutor<sysobjects>(tm);
            queryExecutor.CreateSqlCommand(@"SELECT name FROM sysobjects WHERE xtype='U' ");
            ISafeDataReader dataReader = queryExecutor.ExecuteReader();

            //DataSet ds = queryExecutor.ExecuteDataSet();

            try
            {
                while (dataReader.Read())
                {
                    Table table = new Table();
                    table.DatabaseName = Name;
                    table.Name = dataReader.GetString("name");

                    if (Tables == null)
                    {
                        Tables = new Dictionary<string, Table>();
                    }

                    Tables.Add(table.Name, table);
                }
                dataReader.Close();
            }
            catch
            {
                if (dataReader != null)
                {
                    dataReader.Close();
                    dataReader = null;
                }

                throw;
            }

            foreach (Table t in Tables.Values)
            {
                t.PopulateColumns(this.Name, tm);
                t.PopulatePrimaryKey(tm);
                t.PopulateForeignKeys(tm);
            }

            tm.CommitTransaction();
        }
    }
}
