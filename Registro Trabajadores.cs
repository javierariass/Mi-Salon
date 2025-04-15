using Mi_Salon.Modulos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mi_Salon
{
    public partial class Registro_Trabajadores : Form
    {
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MiSalon.db";


        public Registro_Trabajadores()
        {
            InitializeComponent();
            Actualizar();
        }

        private void Registro_Trabajadores_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(textBox2.Text) && !string.IsNullOrWhiteSpace(textBox1.Text) && !string.IsNullOrWhiteSpace(textBox3.Text))
                {
                    Functions.RegistrarPeluquero(appDataPath, textBox1.Text, int.Parse(textBox2.Text), textBox3.Text);
                    MessageBox.Show("Registrado con exito.", "Informe");
                    Actualizar();
                }
            }
            catch
            {
                MessageBox.Show("Registro Fallido", "Alerta");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {           
            try
            {
                string nombre = dataGridView1.SelectedRows[0].Cells["NombreT"].Value.ToString();
                int telefono = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["TelefonoT"].Value);
                string correo = dataGridView1.SelectedRows[0].Cells["CorreoT"].Value.ToString();

                if (Functions.EliminarTrabajador(appDataPath, nombre, telefono, correo))
                {
                    MessageBox.Show("Registro de tranajador eliminado.", "Aviso");
                }
                else
                {
                    MessageBox.Show("No se ha podido eliminar el registro", "Aviso");
                }
                Actualizar();
            }
            catch
            {
                MessageBox.Show("Aseguerese de haber marcado una fila", "Aviso");
            }
        }

        public void Actualizar()
        {
            dataGridView1.Rows.Clear();

            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={appDataPath};Version=3;"))
            {
                connection.Open();
                string query = "SELECT * FROM Peluqueros";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int rowIndex = dataGridView1.Rows.Add();
                            DataGridViewRow row = dataGridView1.Rows[rowIndex];

                            row.Cells["NombreT"].Value = reader["Nombre"];
                            row.Cells["TelefonoT"].Value = reader["Telefono"];
                            row.Cells["CorreoT"].Value = reader["Correo"];
                        }
                    }
                }
            }
        }
    }
}
