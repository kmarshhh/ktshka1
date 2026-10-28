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
    /// <summary>
    /// Универсальная форма для вывода результата любого запроса из вкладки "Запросы".
    /// </summary>
    public partial class Form4 : Form
    {
        public static string connectString = "Provider = Microsoft.Jet.OLEDB.4.0; Data Source = Группа студентов.mdb";

        private readonly string queryTitle;
        private readonly string querySql;
        private readonly OleDbParameter[] queryParameters;

        // Конструктор без параметров нужен только конструктору форм Visual Studio
        public Form4()
            : this("Запрос", "SELECT * FROM Студенты")
        {
        }

        public Form4(string title, string sql, params OleDbParameter[] parameters)
        {
            InitializeComponent();
            queryTitle = title;
            querySql = sql;
            queryParameters = parameters ?? new OleDbParameter[0];
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            Text = "Запросы";
            label1.Text = queryTitle;

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectString))
                {
                    connection.Open();

                    using (OleDbCommand command = new OleDbCommand(querySql, connection))
                    {
                        if (queryParameters.Length > 0)
                            command.Parameters.AddRange(queryParameters);

                        using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
                        {
                            DataTable table = new DataTable();
                            adapter.Fill(table);

                            dataGridView1.DataSource = table;
                            label2.Text = "Найдено записей: " + table.Rows.Count;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка выполнения запроса: " + ex.Message, "Запросы",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
