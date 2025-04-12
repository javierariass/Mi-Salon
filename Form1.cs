using Mi_Salon.Modulos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mi_Salon
{
    public partial class Form1 : Form
    {
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MiSalon.db";
        List<string> NCliente = new List<string>();

        public Form1()
        {
            InitializeComponent();
            Functions.Connection(appDataPath);
            Actualizar();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        //Actualizar Datos del form
        private void Actualizar()
        {
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd"); // Standardized date format
            label7.Text = currentDate; // Ensure label uses the same format as the database

            RellenarReservasDelDia(currentDate);
            RellenarReservas();

            // Peluqueros con los que puede reservar
            PeluquerosReserva.Items.Clear();
            PeluquerosReserva.Items.AddRange(Functions.Peluqueros.ToArray());
            if (Functions.Peluqueros.Count != 0) PeluquerosReserva.SelectedIndex = 0;

            // Clientes para facturar
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(NCliente.ToArray());
            if (NCliente.Count != 0) comboBox1.SelectedIndex = 0;

            // Peluqueros para facturar
            comboBox2.Items.Clear();
            comboBox2.Items.AddRange(Functions.Peluqueros.ToArray());
            if (Functions.Peluqueros.Count != 0) comboBox2.SelectedIndex = 0;
        }

        // Visualizar las reservaciones del día
        public void RellenarReservasDelDia(string fecha)
        {
            dataGridView1.Rows.Clear();
            NCliente.Clear();

            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={appDataPath};Version=3;"))
            {
                connection.Open();
                string query = "SELECT * FROM Reservas WHERE Fecha = @Fecha";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Fecha", fecha);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int rowIndex = dataGridView1.Rows.Add();
                            DataGridViewRow row = dataGridView1.Rows[rowIndex];

                            row.Cells["Nombre"].Value = reader["Nombre"];
                            NCliente.Add(row.Cells["Nombre"].Value.ToString());
                            row.Cells["Telefono"].Value = reader["Telefono"];
                            row.Cells["Correo"].Value = reader["Correo"];
                            row.Cells["PeluqueroReservado"].Value = reader["Peluquero"];
                            row.Cells["Rebooking"].Value = int.Parse(reader["Rebooking"].ToString()) == 1;
                        }
                    }
                }
            }
        }

        // Actualizar datos de las reservas
        public void RellenarReservas()
        {
            dataGridView3.Rows.Clear();

            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={appDataPath};Version=3;"))
            {
                connection.Open();
                string query = "SELECT * FROM Reservas";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int rowIndex = dataGridView3.Rows.Add();
                            DataGridViewRow row = dataGridView3.Rows[rowIndex];

                            row.Cells["NombreR"].Value = reader["Nombre"];
                            row.Cells["TelefonoR"].Value = reader["Telefono"];
                            row.Cells["CorreoR"].Value = reader["Correo"];
                            row.Cells["PeluqueroR"].Value = reader["Peluquero"];
                            row.Cells["RebookingR"].Value = int.Parse(reader["Rebooking"].ToString()) == 1;
                            row.Cells["FechaR"].Value = DateTime.Parse(reader["Fecha"].ToString()).ToString("yyyy-MM-dd"); // Ensure formatted date
                        }
                    }
                }
            }
        }

        // Botón para agregar reservas
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(textBox1.Text) && !string.IsNullOrWhiteSpace(textBox2.Text) && !string.IsNullOrWhiteSpace(textBox3.Text))
                {
                    Functions.ReservarCita(appDataPath, textBox1.Text, int.Parse(textBox2.Text), textBox3.Text, PeluquerosReserva.SelectedItem.ToString(), 0, DateTime.Parse(dateTimeReservation.Text).ToString("yyyy-MM-dd"));
                    MessageBox.Show("Reserva realizada", "Aviso");
                    textBox1.Text = "";
                    textBox2.Text = "";
                    textBox3.Text = "";
                    Actualizar();
                }
                else
                {
                    MessageBox.Show("Datos de la reserva incorrectos.", "Error");
                }
            }
            catch
            {
                MessageBox.Show("Datos de la reserva incorrectos.", "Error");
            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        // Botón para eliminar una reserva de la lista de reservas
        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                string nombre = dataGridView3.SelectedRows[0].Cells["NombreR"].Value.ToString();
                int telefono = Convert.ToInt32(dataGridView3.SelectedRows[0].Cells["TelefonoR"].Value);
                string fecha = dataGridView3.SelectedRows[0].Cells["FechaR"].Value.ToString();

                if (Functions.EliminarReserva(appDataPath, nombre, telefono, fecha))
                {
                    MessageBox.Show("Reserva de " + nombre + " ha sido eliminada", "Aviso");
                }
                else
                {
                    MessageBox.Show("No se ha podido eliminar la reserva", "Aviso");
                }

                Actualizar();
            }
            catch
            {
                MessageBox.Show("Aseguerese de haber marcado una fila", "Aviso");
            }
        }

        // Botón para cerrar el día
        private void button2_Click(object sender, EventArgs e)
        {
            List<(string Nombre, int Telefono)> registros = new List<(string, int)>();

            foreach (DataGridViewRow fila in dataGridView1.Rows)
            {
                if (!fila.IsNewRow)
                {
                    string nombre = fila.Cells["Nombre"].Value.ToString();
                    int telefono = Convert.ToInt32(fila.Cells["Telefono"].Value);

                    registros.Add((nombre, telefono));
                }
            }

            Functions.CierreDiario(appDataPath, registros);
            Actualizar();
        }
    }
}