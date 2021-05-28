using Simple.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace OrmCodeGeneratorApp
{
    public partial class ServerSelectionForm : Form
    {
        private SqlInfoEnumerator SqlEnumerator {get;set;}
        private LoadingWaitForm f = new LoadingWaitForm();
        public string SelectedServerName { get; set; }

        public ServerSelectionForm(SqlInfoEnumerator enums)
        {
            InitializeComponent();

            SqlEnumerator = enums;
        }

        private void ServerSelectionForm_Shown(object sender, EventArgs e)
        {            
            //f.Show(this);
            
            //LoadServernames();

            //f.Close();
        }

        private void LoadServernames()
        {
            // Need .NET 4.0
            //DataTable dataTable = SmoApplication.EnumAvailableSqlServers(true);
            //List<SqlServerInstance> listOfServers = DataTableToSourceList.DataTableToList<SqlServerInstance>(dataTable);

            //List<string> listOfServers = new List<string>(SqlEnumerator.EnumerateSQLServers());

            //listBox1.Items.AddRange(listOfServers.ToArray());
            listBox1.Items.Add(@"ABC-PC\SQLEXPRESS");
        }

        private void ServerSelectionForm_Load(object sender, EventArgs e)
        {            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Enabled = false;

            //f.Show(this);

            LoadServernames();
            //SelectedServerName = "ABC-PC\\SQLEXPRESS";

            //f.Close();

            this.Enabled = true;
            selectButton.Enabled = true;
        }

        private void selectButton_Click(object sender, EventArgs e)
        {
            SelectedServerName = "ABC-PC\\SQLEXPRESS";//listBox1.SelectedItem.ToString();

            this.Close();
        }
    }
}
