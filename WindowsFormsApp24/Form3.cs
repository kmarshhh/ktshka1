using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace WindowsFormsApp24
{
    public partial class Form3 : Form
    {
        public string s;
        public static string connectString = "Provider = Microsoft.Jet.OLEDB.4.0; Data Source = Группа студентов.mdb";
        private OleDbConnection myConnection;
        public Form3()
        {
            InitializeComponent();
            myConnection = new OleDbConnection(connectString);
            myConnection.Open();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            label1.Text = s;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (s == "Введите фамилию")
            {
                string Name = textBox1.Text;
                string query = $"SELECT Фамилия, Имя FROM Студенты WHERE Фамилия = '{Name}'";
                OleDbDataAdapter command = new OleDbDataAdapter(query, myConnection);
                DataTable d = new DataTable();
                command.Fill(d);
                dataGridView1.DataSource = d;
                myConnection.Close();
            }
            else
            {
                int Name = Convert.ToInt32(textBox1.Text);
                string query = $"SELECT Фамилия, Имя FROM Студенты WHERE Дом = {Name}";
                OleDbDataAdapter command = new OleDbDataAdapter(query, myConnection);
                DataTable d = new DataTable();
                command.Fill(d);
                dataGridView1.DataSource = d;
                myConnection.Close();
            }
            
        }
    }
}
