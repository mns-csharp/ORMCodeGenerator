using Simple.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace OrmCodeGeneratorApp
{
    public class Server
    {
        public string Name { get; set; }
        public Dictionary<string, Database> Databases { get; private set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConnectionString { get; set; }
        public string Provider { get; set; }

        //
        public void PopulateDatabases(string[] databaseNames)
        {
            TransactionManager tm = new TransactionManager(ConnectionString, Provider);

            foreach (string dbn in databaseNames)
            {
                Database db = new Database();
                db.Name = dbn;
                db.PopulateTables(tm);

                if (Databases == null)
                {
                    Databases = new Dictionary<string, Database>();
                }

                Databases.Add(dbn, db);
            }
        }
    }
}
