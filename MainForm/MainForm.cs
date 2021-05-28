using Microsoft.SqlServer;
using Simple.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace OrmCodeGeneratorApp
{
    public partial class MainForm : Form
    {
        string CurrentApplicationDirectory = string.Empty;
        enum CellNo {Select, Parent, ParentID, ChildID, Child}
        SqlInfoEnumerator __sqlEnum = new SqlInfoEnumerator();
        string __provider = "System.Data.SqlClient";
        SqlConnectionStringBuilder __connStrBuilder = new SqlConnectionStringBuilder();
        Server __server = new Server();

        public MainForm()
        {
            InitializeComponent();

            CurrentApplicationDirectory = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

            #region SqlServerEnum
            //// Create a instance of the SqlDataSourceEnumerator class
            //SqlDataSourceEnumerator objSqlDS = SqlDataSourceEnumerator.Instance;
            //// Fetch all visible SQL server 2000 or SQL Server 2005 instances
            //DataTable objTbl = objSqlDS.GetDataSources(); 
            #endregion

            #region OleDbEnum
            //OleDbEnumerator enums = new OleDbEnumerator();
            //DataTable tbl = enums.GetElements();
            //List<Source> list = DataTableToSourceList.DataTableToList<Source>(tbl); 
            #endregion

            string str = string.Empty;
        }

        #region Display Server & Databases
        private void MainForm_Shown(object sender, EventArgs e)
        {
            ServerSelectionForm f = new ServerSelectionForm(__sqlEnum);
            f.ShowDialog();

            if (!string.IsNullOrEmpty(f.SelectedServerName))
            {
                string serverName = __sqlEnum.SQLServer = f.SelectedServerName;

                string[] dbNames = __sqlEnum.EnumerateSQLServersDatabases();

                TreeNodeTag serverTag = new TreeNodeTag();
                serverTag.NodeType = NodeTypeEnum.Server;
                serverTag.Object = __server;

                TreeNode root = new TreeNode(serverName, 0, 0);
                root.Tag = serverTag;
                treeView1.Nodes.Add(root);

                foreach (string dbName in dbNames)
                {
                    TreeNodeTag dbTag = new TreeNodeTag();
                    dbTag.NodeType = NodeTypeEnum.Database;

                    TreeNode dbNode = new TreeNode(dbName, 0, 0);
                    dbNode.Tag = dbTag;
                    root.Nodes.Add(dbNode);
                }

                __server.Name = serverName;
                __server.Provider = __provider;

                string str = string.Empty;
            }
        } 
        #endregion

        #region """TreeView load, check, uncheck"
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode selectedNode = treeView1.SelectedNode;

            TreeNodeTag tag = selectedNode.Tag as TreeNodeTag;

            if (tag != null)
            {
                if (tag.NodeType == NodeTypeEnum.Database)
                {
                    string dbName = selectedNode.Text;

                    __server.ConnectionString = @"Data Source=.\sqlexpress;Initial Catalog=" + dbName + ";Integrated Security=True";

                    if (__server.Databases == null)
                    {
                        __server.PopulateDatabases(new string[] { dbName });
                    }
                    else if (!__server.Databases.ContainsKey(dbName))
                    {
                        __server.PopulateDatabases(new string[] { dbName });
                    }

                    tag.Object = __server.Databases[dbName];

                    if (__server.Databases != null)
                    {
                        if (__server.Databases.Count > 0)
                        {
                            // Create 'Table' nodes
                            if (selectedNode.Nodes.Count == 0)
                            {
                                foreach (Table t in __server.Databases[dbName].Tables.Values)
                                {
                                    TreeNodeTag tTag = new TreeNodeTag();
                                    tTag.NodeType = NodeTypeEnum.Table;
                                    tTag.Object = t;

                                    TreeNode tableNode = new TreeNode(t.Name, 0, 0);
                                    tableNode.Tag = tTag;

                                    if (t.Columns != null)
                                    {
                                        if (t.Columns.Count > 0)
                                        {
                                            if (tableNode.Nodes.Count == 0)
                                            {
                                                // Create 'Column' nodes
                                                foreach (Column c in t.Columns.Values)
                                                {
                                                    TreeNodeTag cTag = new TreeNodeTag();
                                                    cTag.NodeType = NodeTypeEnum.Table;
                                                    cTag.Object = c;

                                                    TreeNode columnNode = new TreeNode(c.GetAsProperty(), 0, 0);
                                                    columnNode.Tag = cTag;

                                                    if (tableNode.Checked) columnNode.Checked = true;

                                                    tableNode.Nodes.Add(columnNode);
                                                }
                                            }
                                        }
                                    }

                                    if (selectedNode.Checked) tableNode.Checked = true;

                                    selectedNode.Nodes.Add(tableNode);
                                }
                            }
                        }
                    }
                }
            }
            string str = string.Empty;
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode selectedNode = e.Node;
            bool checkedStatus = e.Node.Checked;

            //Children must be processes before parent
            if (HasChildrens(selectedNode))
            {
                CheckAllChildNodes(selectedNode, checkedStatus);
            }
            if (HasParent(selectedNode))
            {
                CheckAllParentNodes(selectedNode, checkedStatus);
            }
        }

        private bool HasParent(TreeNode node)
        {
            return node.Parent == null ? false : true;
        }

        public bool HasChildrens(TreeNode node)
        {
            if (node.Nodes == null) return false;
            else return true;
        }

        void CheckAllParentNodes(TreeNode node, bool check)
        {
            while (node != null)
            {
                bool anyChildChecked = IsAnyChildChecked(node);

                if (anyChildChecked == true)
                {
                    node.Checked = true;
                }
                else
                {
                    node.Checked = check;
                }

                //Traversing
                node = node.Parent;
            }
        }

        void CheckAllChildNodes(TreeNode node, bool check)
        {
            foreach (TreeNode n in node.Nodes)
            {
                n.Checked = check;

                CheckAllChildNodes(n, check);
            }
        }

        bool IsAnyChildChecked(TreeNode node)
        {
            bool returns = false;

            foreach (TreeNode n in node.Nodes)
            {
                if (n.Checked == true) return true;

                returns = IsAnyChildChecked(n);
            }

            return returns;
        } 
        #endregion        

        #region Load Tables in DataGridView
        private void loadButton_Click(object sender, EventArgs e)
        {
            try
            {
                TreeNode selectedNode = treeView1.SelectedNode;// obtain the selected tree-node

                TreeNodeTag tag = selectedNode.Tag as TreeNodeTag;// obtain the tag of the selected tree-node

                NodeTypeEnum nodeType = tag.NodeType;// get the type of the selected tree-node

                // ontain the appropriate Database, or, 
                // Table, or, Column based on the node-type.
                string databaseName = string.Empty;
                Database database = null;
                if (nodeType == NodeTypeEnum.Database)
                {
                    database = tag.Object as Database;
                    databaseName = database.Name;
                }
                else if (nodeType == NodeTypeEnum.Table)
                {
                    Table table = tag.Object as Table;
                    databaseName = table.DatabaseName;
                    database = __server.Databases[databaseName];
                }
                else if (nodeType == NodeTypeEnum.Column)
                {
                    Column col = tag.Object as Column;
                    databaseName = col.DatabaseName;
                    database = __server.Databases[databaseName];
                }

                // populate the DataGridView
                if (database != null)
                {
                    childDataGridView1Column1.Items.Clear();
                    dataGridView1.Rows.Clear();

                    IList<Table> tables = new List<Table>(database.Tables.Values);
                    List<Table> tablesCopy = new List<Table>(tables);
                    tablesCopy.Insert(0, new Table("Select"));

                    // Loading ComboBox columns              
                    int i = 0;
                    foreach (Table t1 in tables)
                    {
                        dataGridView1.Rows.Add(true, t1.Name, t1.PrimaryKey);//Set Child's text to "None"
                        dataGridView1.Rows[i].Tag = t1;

                        // Load 'Database.Tables' to 'Child' column
                        DataGridViewComboBoxCell dataGridview1ChildComboBoxCell = (DataGridViewComboBoxCell)dataGridView1.Rows[i].Cells[(int)CellNo.Child];
                        dataGridview1ChildComboBoxCell.Items.Clear();

                        foreach (Table t2 in tablesCopy)
                        {
                            dataGridview1ChildComboBoxCell.Items.Add(t2.Name);
                        }
                        dataGridView1.Rows[i].Cells[(int)CellNo.Child].Value = "Select";

                        // Load 'Table.Columns' to 'ChildID' column
                        DataGridViewComboBoxCell dataGridview1ChildIDComboBoxCell = (DataGridViewComboBoxCell)dataGridView1.Rows[i].Cells[(int)CellNo.ChildID];
                        dataGridview1ChildIDComboBoxCell.Items.Clear();
                        //if (tablesCopy[0].Columns != null)
                        {
                            List<Column> columns = new List<Column>(tablesCopy[0].Columns.Values);
                            foreach (Column c in columns)
                            {
                                dataGridview1ChildIDComboBoxCell.Items.Add(c.Name);
                            }
                            dataGridView1.Rows[i].Cells[(int)CellNo.ChildID].Value = columns[0].Name;
                        }
                        i++;
                    }
                }
            }
            catch
            {
                throw;
            }
        } 
        #endregion

        #region dataGridView1 comboBox change
        // 
        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            string newTableName = dataGridView1.CurrentCell.EditedFormattedValue as string;

            if (!string.IsNullOrEmpty(newTableName))
            {
                int i = dataGridView1.CurrentCell.RowIndex;

                // obtain the database info
                DataGridViewRow row = dataGridView1.Rows[i];
                Table table = row.Tag as Table;
                string databaseName = table.DatabaseName;
                Database database = __server.Databases[databaseName];

                // Obtain the Table object of the comboBox
                Table t = null;
                try
                {
                    t = database.Tables[newTableName];
                }
                catch
                {
                    t = new Table("Select");
                }
                // Load 'Table.Columns' to 'ChildID' column
                DataGridViewComboBoxCell dataGridview1ChildIDComboBoxCell = (DataGridViewComboBoxCell)dataGridView1.Rows[i].Cells[(int)CellNo.ChildID];
                dataGridview1ChildIDComboBoxCell.Items.Clear();
                foreach (Column c in t.Columns.Values)
                {
                    dataGridview1ChildIDComboBoxCell.Items.Add(c.Name);
                }
                List<Column> columns = new List<Column>(t.Columns.Values);
                dataGridView1.Rows[i].Cells[(int)CellNo.ChildID].Value = columns[0].Name;
            }

            string str = string.Empty;
        } 
        #endregion

        #region generateButton
        private void generateButton_Click(object sender, EventArgs e)
        {
            // delete all existing folders and files
            string folderPath = Consts.CodeGenFolderName;
            System.IO.DirectoryInfo di = new DirectoryInfo(folderPath);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }

            // Generate new files
            DataGridViewRowCollection rows = dataGridView1.Rows;
            TextFile tf = new TextFile();
            foreach (DataGridViewRow dgvRow in rows)
            {
                Table t = dgvRow.Tag as Table;

                tf.Clear();//empty the string-builder
                tf.Append(t.GetAsClass());
                tf.WritePath = TextFile.ApplicationDirectory + "\\" + Consts.CodeGenFolderName + "\\POCO\\" + t.Name + ".cs";
                tf.Write();

                //sb.Capacity = 0;//empty the string-builder
                tf.Clear();//empty the string-builder
                tf.Append(t.GetAsBusinessLogic());
                tf.WritePath = TextFile.ApplicationDirectory + "\\" + Consts.CodeGenFolderName + "\\BLL\\" + t.Name + "BLL.cs";
                tf.Write();

                tf.Clear();//empty the string-builder
                tf.Append(t.GetAsDataAccessObject());
                tf.WritePath = TextFile.ApplicationDirectory + "\\" + Consts.CodeGenFolderName + "\\DAO\\" + t.Name + "DAO.cs";
                tf.Write();
            }
        } 
        #endregion

        private void openLocationButton_Click(object sender, EventArgs e)
        {
            string folderPath = Consts.CodeGenFolderName;

            if (Directory.Exists(folderPath))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo("explorer.exe", folderPath);

                Process.Start(startInfo);
            }
            else
            {
                MessageBox.Show(string.Format("{0} Directory does not exist!", folderPath));
            }
        }
    }
}

