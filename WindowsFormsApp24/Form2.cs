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
    public partial class Form2 : Form
    {
        public DataRowView currentRow;
        public static string connectString = "Provider = Microsoft.Jet.OLEDB.4.0; Data Source = Группа студентов.mdb";
        private OleDbConnection myConnection;
        public Form2()
        {
            InitializeComponent();
            myConnection = new OleDbConnection(connectString);
            myConnection.Open();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            // Выпадающий список групп: уже существующие группы из таблицы
            OleDbDataAdapter da = new OleDbDataAdapter(
                "SELECT DISTINCT Группа FROM Студенты WHERE Группа IS NOT NULL ORDER BY Группа", myConnection);
            DataTable groups = new DataTable();
            da.Fill(groups);
            foreach (DataRow r in groups.Rows)
                comboBox1.Items.Add(r["Группа"].ToString());

            if (currentRow != null)
            {
                textBox1.Text = currentRow.Row["Код"].ToString();
                textBox2.Text = currentRow.Row["Фамилия"].ToString();
                textBox3.Text = currentRow.Row["Имя"].ToString();

                DateTime dr;
                if (DateTime.TryParse(currentRow.Row["Дата рождения"].ToString(), out dr))
                    dateTimePicker1.Value = dr;

                checkBox1.Checked = currentRow.Row["Пол"] != DBNull.Value && Convert.ToBoolean(currentRow.Row["Пол"]);
                textBox6.Text = currentRow.Row["Улица"].ToString();
                textBox7.Text = currentRow.Row["Дом"].ToString();
                textBox8.Text = currentRow.Row["Квартира"].ToString();
                textBox9.Text = currentRow.Row["Класс"].ToString();
                comboBox1.Text = currentRow.Row["Группа"].ToString();
                textBox11.Text = currentRow.Row["Хобби"].ToString();
            }
            else
            {
                textBox1.Text = "авто"; // код присваивается базой (счётчик)
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int house, appart;
            if (!int.TryParse(textBox7.Text, out house) || !int.TryParse(textBox8.Text, out appart))
            {
                MessageBox.Show("Дом и квартира должны быть числами.");
                return;
            }

            string surname = textBox2.Text;
            string name = textBox3.Text;
            DateTime dr = dateTimePicker1.Value.Date;
            bool male = checkBox1.Checked;
            string street = textBox6.Text;
            string clas = textBox9.Text;
            string group = comboBox1.Text;
            string hobby = textBox11.Text;

            OleDbCommand comand;
            if (currentRow == null)
            {
                // Код не указываем - он автоинкрементный
                comand = new OleDbCommand(
                    "INSERT INTO Студенты ([Фамилия], [Имя], [Дата рождения], [Пол], [Улица], [Дом], [Квартира], [Класс], [Группа], [Хобби]) " +
                    "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)", myConnection);
            }
            else
            {
                comand = new OleDbCommand(
                    "UPDATE Студенты SET [Фамилия] = ?, [Имя] = ?, [Дата рождения] = ?, [Пол] = ?, [Улица] = ?, " +
                    "[Дом] = ?, [Квартира] = ?, [Класс] = ?, [Группа] = ?, [Хобби] = ? WHERE [Код] = ?", myConnection);
            }

            // Порядок параметров должен совпадать с порядком знаков ? в запросе
            comand.Parameters.AddWithValue("@surname", surname);
            comand.Parameters.AddWithValue("@name", name);
            comand.Parameters.Add("@dr", OleDbType.Date).Value = dr;
            comand.Parameters.AddWithValue("@male", male);
            comand.Parameters.AddWithValue("@street", street);
            comand.Parameters.AddWithValue("@house", house);
            comand.Parameters.AddWithValue("@appart", appart);
            comand.Parameters.AddWithValue("@clas", clas);
            comand.Parameters.AddWithValue("@group", group);
            comand.Parameters.AddWithValue("@hobby", hobby);
            if (currentRow != null)
                comand.Parameters.AddWithValue("@kod", Convert.ToInt32(currentRow.Row["Код"]));

            comand.ExecuteNonQuery();
            MessageBox.Show(currentRow == null ? "Строка добавлена!" : "Изменения сохранены!");
            Close();
        }
    }
}
