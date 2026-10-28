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
    }
}
