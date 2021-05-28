using Simple.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace OrmCodeGeneratorApp
{
    public class Table
    {
        public string DatabaseName { get; set; }
        public string Name { get; set; }
        public Column PrimaryKey { get; set; }
        public Dictionary<string, Column> Columns { get; set; }

        public Table()
        {
            Columns = new Dictionary<string, Column>();
        }

        public Table(string name)
        {
            Name = name;
            Columns = new Dictionary<string, Column>();
            Columns.Add("None", new Column("None"));
        }

        // Acquire column info from database
        #region PopulateColumns
        public void PopulateColumns(string database, TransactionManager tm)
        {
            IQueryExecutor<sysobjects> queryExecutor = new QueryExecutor<sysobjects>(tm);
            queryExecutor.CreateSqlCommand(@"SELECT column_name, 
                                                        is_nullable, 
                                                        data_type, 
                                                        character_maximum_length
                                                FROM " + database + @".INFORMATION_SCHEMA.COLUMNS
                                                WHERE TABLE_NAME = @TABLE_NAME");
            queryExecutor.AddParameter(Name, DbType.String);
            ISafeDataReader dataReader = queryExecutor.ExecuteReader();

            try
            {
                while (dataReader.Read())
                {
                    Column col = new Column();
                    col.DatabaseName = database;
                    col.TableName = Name;
                    col.Name = dataReader.GetString("column_name");

                    string is_nullable = dataReader.GetString("is_nullable");

                    col.IsNullable = ((is_nullable == "NO") ? false : true);
                    col.DataType = TypeConverter.SqlToDotNetType(dataReader.GetString("data_type"));

                    string char_max_len = dataReader.GetString("character_maximum_length");

                    int? val = (string.IsNullOrEmpty(char_max_len)) ? null : (int?)Convert.ToInt32(char_max_len);

                    col.Size = val;


                    if (Columns == null)
                    {
                        Columns = new Dictionary<string, Column>();
                    }

                    Columns.Add(col.Name, col);
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
        }
        #endregion

        #region POCO
        public string GetAsClass()
        {
            TextFile tf = new TextFile();
            tf.ReadPath = TextFile.ApplicationDirectory + Consts.ClassTemplate;
            tf.Read();

            StringBuilder sb = new StringBuilder();
            sb.Append("public class " + Name + " : BaseClass<"+Name+">\r\n");
            sb.Append("\t{\r\n");
            foreach (Column c in Columns.Values)
            {
                sb.Append("\t\t" + c.GetAsProperty() + "\r\n");
            }
            sb.Append("\t}\r\n");

            string str = sb.ToString();

            tf.Replace(Consts.Namspace, this.DatabaseName);
            tf.Replace(Consts.Poco, str);

            return tf.Text;
        }
        #endregion

        #region BLL
        public string GetAsBusinessLogic()
        {
            // obtain all DAO files
            List<FileInfo> fileNames = TextFile.GetFileNames(TextFile.ApplicationDirectory + Consts.DaoFunctionsTemplate);

            TextFile bllFuncTemplate = new TextFile();
            bllFuncTemplate.ReadPath = TextFile.ApplicationDirectory + Consts.ParentCodeBLLTemplate;
            bllFuncTemplate.Read();

            // get names and parameters of each function
            // and, create BLL functions of the same name 
            // and parameters.
            TextFile bllAllFunctions = new TextFile();
            foreach (FileInfo item in fileNames)
            {
                TextFile daoXml = new TextFile();
                daoXml.ReadPath = item.FullName;
                daoXml.ReadXml();

                Function daoF = daoXml.Function;

                // create BLL functions from DAO functions.
                Function busLofFunc = new Function();
                busLofFunc.Content = bllFuncTemplate.Text;
                busLofFunc.Replace(Consts.ReturnType, daoF.ReturnType);
                busLofFunc.Replace(Consts.DefaultValue, daoF.DefaultValue);
                busLofFunc.Replace(Consts.BllFunctionName, daoF.Name);
                busLofFunc.Replace(Consts.Parameters, daoF.Arguments.GetParameters());

                if (daoF.Arguments.Argumentso.Count > 1)
                {
                    busLofFunc.Replace(Consts.Arguments, daoF.Arguments.GetArguments());
                }
                else if (daoF.Arguments.Argumentso.Count <= 1)
                {
                    busLofFunc.Replace(Consts.CommaPlusArguments, daoF.Arguments.GetArguments());
                }

                // accumulate all BLL functions
                bllAllFunctions.Append(busLofFunc.ToString());
            }

            TextFile bll = new TextFile();
            bll.ReadPath = TextFile.ApplicationDirectory + Consts.ParentBLLTemplate;
            bll.Read();
            bll.Replace(Consts.Namspace, this.DatabaseName);
            bll.Replace(Consts.Functions, bllAllFunctions.Text);
            bll.Replace(Consts.Table, this.Name);
            bll.Replace(Consts.TableLowercase, this.Name.ToLower());

            return bll.Text;
        }
        #endregion

        #region DAO
        public string GetAsDataAccessObject()
        {
            SqlGenerator sql = new SqlGenerator(this);
            string path = TextFile.ApplicationDirectory + Consts.DaoFunctionsTemplate;
            List<FileInfo> fileNames = TextFile.GetFileNames(path);
            

            TextFile daoAllFunctions = new TextFile();
            foreach (FileInfo item in fileNames)
            {
                TextFile daoFunction = new TextFile();
                daoFunction.ReadPath = item.FullName;
                daoFunction.ReadXml();

                switch(daoFunction.SqlType)
                { 
                    case SqlType.GetAll:
                    daoFunction.Replace(Consts.Sql, sql.GetAll());
                    break;
                    case SqlType.GetByID:
                    daoFunction.Replace(Consts.Sql, sql.GetByID());
                    break;
                    case SqlType.InsertByID:
                    daoFunction.Replace(Consts.Sql, sql.InsertByID());
                    break;
                    case SqlType.UpdateByID:
                    daoFunction.Replace(Consts.Sql, sql.UpdateByID());
                    break;
                    case SqlType.DeleteByID:
                    daoFunction.Replace(Consts.Sql, sql.DeleteByID());
                    break;
                }

                daoAllFunctions.Append(daoFunction.Text);
            }

            TextFile dao = new TextFile();
            dao.ReadPath = TextFile.ApplicationDirectory + Consts.DaoTemplate;
            dao.Read();
            dao.Replace(Consts.Namspace, this.DatabaseName);
            dao.Replace(Consts.Functions, daoAllFunctions.Text);
            dao.Replace(Consts.Table, this.Name);

            ParameterGenerator asGen = new ParameterGenerator(this.PrimaryKey, this.Columns);
            dao.Replace(Consts.Assignments, asGen.GetAssignments());
            dao.Replace(Consts.Parameters, asGen.GetParameters());
            dao.Replace(Consts.PrimaryKey, asGen.GetPrimaryKey());

            return dao.Text;
        }
        #endregion

        public string GetEntityForm()
        {
            StringBuilder control_to_object = new StringBuilder();
            StringBuilder object_to_controls = new StringBuilder();
            StringBuilder clear_controls = new StringBuilder();
            foreach (Column c in this.Columns.Values)
            {
                //$controls_to_object$
                string controlType = TypeConverter.ToControl(c);
                control_to_object.Append("item." + c.Name + " = " + c.Name.ToLower() + controlType + "."+TypeConverter.ToControlValue(controlType)+";\r\n");

                //$object_to_controls$
                object_to_controls.Append(c.Name.ToLower() + controlType + "." + TypeConverter.ToControlValue(controlType) + " = item." + c.Name+";\r\n" );

                //$clear_controls$
                clear_controls.Append(c.Name.ToLower() + controlType + "." + TypeConverter.ToControlValue(controlType) + " = " + TypeConverter.ToControlClearValue(controlType) + ";\r\n");
            }

            TextFile entityForm = new TextFile();
            entityForm.ReadPath = TextFile.ApplicationDirectory + Consts.GuiEntityForm;
            entityForm.Read();
            entityForm.Replace(Consts.ControlsToObject, control_to_object.ToString());
            entityForm.Replace(Consts.ObjectToControls, object_to_controls.ToString());
            entityForm.Replace(Consts.ClearControls, clear_controls.ToString());

            return entityForm.Text;
        }

        #region Foreign Key
        public void PopulateForeignKeys(TransactionManager tm)
        {
            IQueryExecutor<foreign_key> queryExecutor = new QueryExecutor<foreign_key>(tm);
            queryExecutor.CreateSqlCommand(@"SELECT  obj.name AS FkName,
                                                sch.name AS [Schema],
                                                tab1.name AS [TableName],
                                                col1.name AS [ColumnName],
                                                tab2.name AS [ForeignTableName],
                                                col2.name AS [ForeignColumnName],
                                                TY.[name] AS [DataType], 
                                                col1.[max_length] as [DataTypeMaxLength],
                                                col1.[precision] as [DataTypePrecision], 
                                                col1.[scale] as [Scale], 
                                                col1.[is_nullable] as IsNullable, 
                                                col1.[is_ansi_padded] as [IsAnsiPadded]
                                            FROM sys.foreign_key_columns fkc
                                            INNER JOIN sys.objects obj
                                                ON obj.object_id = fkc.constraint_object_id
                                            INNER JOIN sys.tables tab1
                                                ON tab1.object_id = fkc.parent_object_id
                                            INNER JOIN sys.schemas sch
                                                ON tab1.schema_id = sch.schema_id
                                            INNER JOIN sys.columns col1
                                                ON col1.column_id = parent_column_id AND col1.object_id = tab1.object_id
                                            INNER JOIN sys.tables tab2
                                                ON tab2.object_id = fkc.referenced_object_id
                                            INNER JOIN sys.columns col2
                                                ON col2.column_id = referenced_column_id AND col2.object_id = tab2.object_id
                                            INNER JOIN sys.[types] TY ON col1.[system_type_id] = TY.[system_type_id] AND col1.[user_type_id] = TY.[user_type_id] 
                                            AND tab1.name = @TableName1");

            queryExecutor.AddParameter(Name, DbType.String);
            ISafeDataReader dataReader = queryExecutor.ExecuteReader();

            try
            {
                while (dataReader.Read())
                {
                    foreign_key fk = new foreign_key();
                    fk.FkName = dataReader.GetString("FkName");
                    fk.Schema = dataReader.GetString("Schema");
                    fk.TableName = dataReader.GetString("TableName");
                    fk.ColumnName = dataReader.GetString("ColumnName");
                    fk.ForeignTableName = dataReader.GetString("ForeignTableName");
                    fk.ForeignColumnName = dataReader.GetString("ForeignColumnName");
                    fk.DataType = dataReader.GetString("DataType");
                    fk.DataTypeMaxLength = (int)dataReader.GetInt32("DataTypeMaxLength");
                    fk.DataTypePrecision = (int)dataReader.GetInt32("DataTypePrecision");
                    fk.Scale = dataReader.GetString("Scale");
                    fk.IsNullable = (bool)dataReader.GetBoolean("IsNullable");
                    fk.IsAnsiPadded = (bool)dataReader.GetBoolean("IsAnsiPadded");

                    Columns[fk.ColumnName].ForeignKey = fk;
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
        }
        #endregion

        #region Primary Key
        public void PopulatePrimaryKey(TransactionManager tm)
        {
            IQueryExecutor<string> queryExecutor = new QueryExecutor<string>(tm);
            queryExecutor.CreateSqlCommand(@"SELECT Col.Column_Name from 
                                                INFORMATION_SCHEMA.TABLE_CONSTRAINTS Tab, 
                                                INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE Col 
                                            WHERE 
                                                Col.Constraint_Name = Tab.Constraint_Name
                                                AND Col.Table_Name = Tab.Table_Name
                                                AND Constraint_Type = 'PRIMARY KEY'
                                                AND Col.Table_Name = @TableName1");

            queryExecutor.AddParameter(Name, DbType.String);
            ISafeDataReader dataReader = queryExecutor.ExecuteReader();

            try
            {
                while (dataReader.Read())
                {
                    if (PrimaryKey == null)
                    {
                        PrimaryKey = new Column();
                    }
                    PrimaryKey.Name = dataReader.GetString("Column_Name");
                }
                dataReader.Close();

                if (PrimaryKey != null)
                {
                    Column PK = Columns[PrimaryKey.Name];
                    Columns.Remove(PrimaryKey.Name);
                    PrimaryKey = PK;
                }
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
        }
        #endregion
    }
}
