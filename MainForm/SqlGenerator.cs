using System;
using System.Collections.Generic;
using System.Text;

namespace OrmCodeGeneratorApp
{
    public class SqlGenerator
    {
        private Table table;

        public SqlGenerator(Table t)
        {
            table = t;
        }

        #region Get
        private string GetQuery;
        public string GetAll()
        {
            if (!string.IsNullOrEmpty(GetQuery))
            {
                return GetQuery;
            }
            else
            {
                StringBuilder sb = new StringBuilder("SELECT ");

                if (table.PrimaryKey != null)
                {
                    sb.Append(table.PrimaryKey.Name + ", ");
                }

                foreach (Column col in table.Columns.Values)
                {
                    sb.Append(col.Name + ", ");
                }
                sb = sb.Remove(sb.Length - 2, 2);
                sb.Append(" FROM ");
                sb.Append(table.Name);

                GetQuery = sb.ToString();

                return GetQuery;
            }
        }
        #endregion

        #region GetByID
        private string GetByIDQuery;
        public string GetByID()
        {
            if (!string.IsNullOrEmpty(GetByIDQuery))
            {
                return GetByIDQuery;
            }
            else
            {
                if (table.PrimaryKey!=null)
                {
                    StringBuilder sb = new StringBuilder(this.GetAll());
                    sb.Append(" WHERE " + table.PrimaryKey.Name);
                    sb.Append("=@" + table.PrimaryKey.Name);

                    GetByIDQuery = sb.ToString();
                }
                else
                {
                    GetByIDQuery = string.Empty;
                }

                return GetByIDQuery;
            }
        }
        #endregion

        #region GetByFieldValue
        public string GetByFieldValue(string fieldName)
        {
            StringBuilder sb = new StringBuilder(this.GetAll());
            sb.Append(" WHERE " + fieldName);
            sb.Append("=@" + fieldName);

            return sb.ToString();
        }
        #endregion

        #region InsertByID
        private string InsertByIDQuery;
        public string InsertByID()
        {
            if (!string.IsNullOrEmpty(InsertByIDQuery))
            {
                return InsertByIDQuery;
            }
            else
            {
                StringBuilder sb = new StringBuilder("INSERT INTO " + table.Name + "(");

                if (table.PrimaryKey!=null)
                {
                    sb.Append(table.PrimaryKey.Name + ", ");
                }

                foreach (Column col in table.Columns.Values)
                {
                    sb.Append(col.Name + ", ");
                }
                sb = sb.Remove(sb.Length - 2, 2);
                sb.Append(") VALUES(");

                if (table.PrimaryKey!=null)
                {
                    sb.Append("@" + table.PrimaryKey.Name + ", ");
                }

                foreach (Column p in table.Columns.Values)
                {
                    sb.Append("@" + p.Name + ", ");
                }
                sb = sb.Remove(sb.Length - 2, 2);
                sb.Append(")");

                InsertByIDQuery = sb.ToString();

                return InsertByIDQuery;
            }
        }
        #endregion

        #region UpdateByID
        private string UpdateByIdQuery;
        public string UpdateByID()
        {
            if (!string.IsNullOrEmpty(UpdateByIdQuery))
            {
                return UpdateByIdQuery;
            }
            else
            {
                StringBuilder sb = new StringBuilder("UPDATE " + table.Name + " SET ");

                foreach (Column p in table.Columns.Values)
                {
                    sb.Append(p.Name + "=@" + p.Name + ", ");
                }
                sb = sb.Remove(sb.Length - 2, 2);

                if (table.PrimaryKey!=null)
                {
                    sb.Append(" WHERE ");
                    sb.Append(table.PrimaryKey.Name + "=@" + table.PrimaryKey.Name);
                }

                UpdateByIdQuery = sb.ToString();

                return UpdateByIdQuery;
            }
        }
        #endregion

        #region UpdateByFieldValue
        public string UpdateByFieldValue(string fieldName)
        {
            string updateQuery;

            StringBuilder sb = new StringBuilder("UPDATE " + table.Name + " SET ");

            foreach (Column p in table.Columns.Values)
            {
                sb.Append(p.Name + "=@" + p.Name + ", ");
            }
            sb = sb.Remove(sb.Length - 2, 2);
            sb.Append(" WHERE ");
            sb.Append(fieldName + "=@" + fieldName);

            updateQuery = sb.ToString();

            return updateQuery;
        }
        #endregion

        #region DeleteByID
        private string DeleteByIDQuery;
        public string DeleteByID()
        {
            if (!string.IsNullOrEmpty(DeleteByIDQuery))
            {
                return DeleteByIDQuery;
            }
            else
            {

                StringBuilder sb = new StringBuilder("DELETE FROM " + table.Name);

                if (table.PrimaryKey!=null)
                {
                    sb.Append(" WHERE ");
                    sb.Append(table.PrimaryKey.Name + "=@" + table.PrimaryKey.Name);
                }

                DeleteByIDQuery = sb.ToString();

                return DeleteByIDQuery;
            }
        }
        #endregion

        #region DeleteByFieldValue
        public string DeleteByFieldValue(string fieldName)
        {
            string deleteQuery;

            StringBuilder sb = new StringBuilder("DELETE FROM " + table.Name + " WHERE ");

            sb.Append(fieldName + "=@" + fieldName);

            deleteQuery = sb.ToString();

            return deleteQuery;
        }
        #endregion
    }
}
