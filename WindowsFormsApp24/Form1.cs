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

    public partial class Form1 : Form
    {
        
        public static string connectString = "Provider = Microsoft.Jet.OLEDB.4.0; Data Source = Группа студентов.mdb";
        private OleDbConnection myConnection;

        // Состояние двойного подтверждения, встроенного в кнопку "Удалить"
        private bool deleteArmed;
        private int armedKod = -1;
        private readonly Timer confirmTimer = new Timer();

        public Form1()
        {
            InitializeComponent();
            myConnection = new OleDbConnection(connectString);
            myConnection.Open();

            confirmTimer.Interval = 5000;
            confirmTimer.Tick += (s, e) => ResetDeleteConfirmation();
            // Если код изменили, подтверждение больше не относится к этой записи
            textBox1.TextChanged += (s, e) => ResetDeleteConfirmation();
        }

        /// <summary>
        /// Сбрасывает «взведённое» состояние кнопки удаления.
        /// </summary>
        private void ResetDeleteConfirmation()
        {
            confirmTimer.Stop();
            deleteArmed = false;
            armedKod = -1;
            button3.Text = "Удалить";
            button3.BackColor = SystemColors.Control;
            button3.ForeColor = SystemColors.ControlText;
            button3.UseVisualStyleBackColor = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: данная строка кода позволяет загрузить данные в таблицу "группа_студентовDataSet.Студенты". При необходимости она может быть перемещена или удалена.
            this.студентыTableAdapter.Fill(this.группа_студентовDataSet.Студенты);

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            confirmTimer.Stop();
            myConnection.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int kod;
            if (!int.TryParse(textBox1.Text, out kod))
            {
                ResetDeleteConfirmation();
                MessageBox.Show("Введите корректный код записи для удаления.");
                return;
            }

            // Первый клик — взводим кнопку, второй клик по ней же — удаляем
            if (!deleteArmed || armedKod != kod)
            {
                deleteArmed = true;
                armedKod = kod;
                button3.Text = "Подтвердить?";
                button3.BackColor = Color.FromArgb(220, 53, 69);
                button3.ForeColor = Color.White;
                button3.UseVisualStyleBackColor = false;
                confirmTimer.Stop();
                confirmTimer.Start();
                return;
            }

            // Второй клик — подтверждение получено
            ResetDeleteConfirmation();

            using (OleDbCommand command = new OleDbCommand("DELETE FROM Студенты WHERE Код = ?", myConnection))
            {
                command.Parameters.AddWithValue("@kod", kod);
                int rows = command.ExecuteNonQuery();
                MessageBox.Show(rows > 0 ? "Элемент удален!" : "Запись с таким кодом не найдена.");
            }
            this.студентыTableAdapter.Fill(this.группа_студентовDataSet.Студенты);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.студентыTableAdapter.Fill(this.группа_студентовDataSet.Студенты);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            f.Owner = this;
            f.currentRow = null;
            f.Text = "Добавление";
            f.FormClosed += (a, b) => this.студентыTableAdapter.Fill(this.группа_студентовDataSet.Студенты);
            f.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2();
            f.Owner = this;
            f.currentRow = студентыBindingSource.Current as DataRowView;
            f.Text = "Редактирование";
            f.FormClosed += (a, b) => this.студентыTableAdapter.Fill(this.группа_студентовDataSet.Студенты);
            f.Show();
        }

        private void поискПоФамилииToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 f = new Form3();
            f.Owner = this;
            f.s = "Введите фамилию";
            f.Show();
        }

        private void поискПоНомеруДомаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 f = new Form3();
            f.Owner = this;
            f.s = "Введите номер дома";
            f.Show();
        }

        /// <summary>
        /// Открывает форму с результатом запроса из вкладки "Запросы".
        /// </summary>
        private void OpenQuery(string title, string sql, params OleDbParameter[] parameters)
        {
            Form4 f = new Form4(title, sql, parameters);
            f.Owner = this;
            f.Show();
        }

        // 1. Кто родился в 2001 году?
        private void запрос1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenQuery("Кто родился в 2001 году?",
                "SELECT * FROM Студенты WHERE [Дата рождения] >= ? AND [Дата рождения] < ?",
                new OleDbParameter("@from", OleDbType.Date) { Value = new DateTime(2001, 1, 1) },
                new OleDbParameter("@to", OleDbType.Date) { Value = new DateTime(2002, 1, 1) });
        }

        // 2. Кто живет на улице Пушкина?
        private void запрос2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenQuery("Кто живет на улице Пушкина?",
                "SELECT * FROM Студенты WHERE Улица LIKE ?",
                new OleDbParameter("@street", OleDbType.VarWChar) { Value = "%Пушкина%" });
        }

        // 3. У кого номер дома меньше 50?
        private void запрос3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenQuery("У кого номер дома меньше 50?",
                "SELECT * FROM Студенты WHERE Дом < 50");
        }

        // 4. У кого номер квартиры меньше 12?
        private void запрос4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenQuery("У кого номер квартиры меньше 12?",
                "SELECT * FROM Студенты WHERE Квартира < 12");
        }

        // 5. У кого мужские имена?
        private void запрос5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenQuery("У кого мужские имена?",
                "SELECT * FROM Студенты WHERE Пол = ?",
                new OleDbParameter("@male", OleDbType.Boolean) { Value = true });
        }

        // 6. У кого фамилия начинается на С?
        private void запрос6ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenQuery("У кого фамилия начинается на С?",
                "SELECT * FROM Студенты WHERE Фамилия LIKE ?",
                new OleDbParameter("@surname", OleDbType.VarWChar) { Value = "С%" });
        }

        // 7. У кого фамилия начинается на Е и имя на Н?
        private void запрос7ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenQuery("У кого фамилия начинается на Е и имя на Н?",
                "SELECT * FROM Студенты WHERE Фамилия LIKE ? AND Имя LIKE ?",
                new OleDbParameter("@surname", OleDbType.VarWChar) { Value = "Е%" },
                new OleDbParameter("@name", OleDbType.VarWChar) { Value = "Н%" });
        }

        // 8. Кто не занимается тяжелой атлетикой?
        private void запрос8ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Учитываем и "тяжелая", и "тяжёлая"
            OpenQuery("Кто не занимается тяжелой атлетикой?",
                "SELECT * FROM Студенты WHERE Хобби IS NULL OR (Хобби NOT LIKE ? AND Хобби NOT LIKE ?)",
                new OleDbParameter("@hobby1", OleDbType.VarWChar) { Value = "%тяжел%" },
                new OleDbParameter("@hobby2", OleDbType.VarWChar) { Value = "%тяжёл%" });
        }

        // 9. Кто родился в 2004 году?
        private void запрос9ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenQuery("Кто родился в 2004 году?",
                "SELECT * FROM Студенты WHERE [Дата рождения] >= ? AND [Дата рождения] < ?",
                new OleDbParameter("@from", OleDbType.Date) { Value = new DateTime(2004, 1, 1) },
                new OleDbParameter("@to", OleDbType.Date) { Value = new DateTime(2005, 1, 1) });
        }

        // 10. Кто родился сегодня?
        private void запрос10ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DateTime today = DateTime.Today;
            OpenQuery("Кто родился сегодня?",
                "SELECT * FROM Студенты WHERE Month([Дата рождения]) = ? AND Day([Дата рождения]) = ?",
                new OleDbParameter("@month", OleDbType.Integer) { Value = today.Month },
                new OleDbParameter("@day", OleDbType.Integer) { Value = today.Day });
        }

        // 11. У кого нет хобби?
        private void запрос11ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenQuery("У кого нет хобби?",
                "SELECT * FROM Студенты WHERE Хобби IS NULL");
        }
    }
}
