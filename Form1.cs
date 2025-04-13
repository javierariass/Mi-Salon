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

            //Establecer la agenda
            RellenarAgenda(dataGridView4,Functions.Peluqueros.ToArray());

            // Peluqueros con los que puede reservar
            PeluquerosReserva.Items.Clear();
            PeluquerosReserva.Items.AddRange(Functions.Peluqueros.ToArray());
            if (Functions.Peluqueros.Count != 0) PeluquerosReserva.SelectedIndex = 0;

            //Servicios que puede solicitar
            comboBox9.Items.Clear();
            comboBox9.Items.AddRange(Functions.Servicios.ToArray());
            if (Functions.Servicios.Count != 0) comboBox9.SelectedIndex = 0;

            // Clientes para facturar
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(NCliente.ToArray());
            if (NCliente.Count != 0) comboBox1.SelectedIndex = 0;

            // Peluqueros para facturar
            comboBox2.Items.Clear();
            comboBox2.Items.AddRange(Functions.Peluqueros.ToArray());
            if (Functions.Peluqueros.Count != 0) comboBox2.SelectedIndex = 0;

            //Servicios que puede Facturar
            comboBox4.Items.Clear();
            comboBox4.Items.AddRange(Functions.Servicios.ToArray());
            if (Functions.Servicios.Count != 0) comboBox9.SelectedIndex = 0;

            //Establecer Hora
            comboBox5.SelectedIndex = 0;
            comboBox6.SelectedIndex = 0;
            comboBox7.SelectedIndex = 0;
            comboBox8.SelectedIndex = 1;
        }

        //Horas en la agenda
        private void RellenarHoras12H(DataGridView dataGridView)
        {
            // Limpiar las filas existentes, si las hay
            dataGridView.Rows.Clear();
            dataGridView.Columns.Add("Hora", "Hora");
            // Configuración inicial de las horas
            DateTime horaInicial = new DateTime(2025, 1, 1, 10, 0, 0); // 10:00 AM
            DateTime horaFinal = new DateTime(2025, 1, 1, 18, 0, 0);  // 6:00 PM

            // Rellenar la primera columna con intervalos de 15 minutos en formato 12 horas
            while (horaInicial <= horaFinal)
            {
                // Crear una nueva fila
                DataGridViewRow fila = new DataGridViewRow();
                fila.CreateCells(dataGridView);

                // Establecer la celda de hora en formato 12 horas (hh:mm tt)
                fila.Cells[0].Value = horaInicial.ToString("hh:mm tt");

                // Agregar la fila al DataGridView
                dataGridView.Rows.Add(fila);

                // Incrementar la hora en 15 minutos
                horaInicial = horaInicial.AddMinutes(15);
            }
        }

        //Nombres de trabajadores
        private void RellenarAgenda(DataGridView dataGridView, string[] nombres)
        {
            dataGridView.Columns.Clear();
            RellenarHoras12H(dataGridView);
            // Crear columnas para cada nombre en el array
            foreach (string nombre in nombres)
            {
                DataGridViewTextBoxColumn Trabajador = new DataGridViewTextBoxColumn
                {
                    Name = nombre,
                    HeaderText = nombre,
                    Width = 250, // Ancho de la columna
                    ReadOnly = true,
                };
                dataGridView.Columns.Add(Trabajador); // El encabezado será el nombre
            }
            DatosAgenda();
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
                            row.Cells["Serv"].Value = reader["Servicio"];
                            row.Cells["Rebooking"].Value = int.Parse(reader["Rebooking"].ToString()) == 1;
                            row.Cells["Desde"].Value = reader["Desde"];
                            row.Cells["Hasta"].Value = reader["Hasta"];

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
                            row.Cells["ServicioR"].Value = reader["Servicio"];
                            row.Cells["RebookingR"].Value = int.Parse(reader["Rebooking"].ToString()) == 1;
                            row.Cells["FechaR"].Value = DateTime.Parse(reader["Fecha"].ToString()).ToString("yyyy-MM-dd"); // Ensure formatted date
                            row.Cells["DesdeR"].Value = reader["Desde"];
                            row.Cells["HastaR"].Value = reader["Hasta"];
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
                    string desde = comboBox5.Text + ":" + comboBox6.Text;
                    string hasta = comboBox7.Text + ":" + comboBox8.Text;
                    desde = int.Parse(comboBox5.Text) >= 10 ? desde + " AM" : desde + " PM";
                    hasta = int.Parse(comboBox6.Text) >= 10 ? hasta + " AM" : hasta + " PM";
                    string date = DateTime.Parse(dateTimeReservation.Text).ToString("yyyy-MM-dd");
                    Functions.ReservarCita(appDataPath, textBox1.Text, int.Parse(textBox2.Text), textBox3.Text, PeluquerosReserva.SelectedItem.ToString(),comboBox9.Text, 0,date ,desde,hasta);
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

        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        //Actualizar Agenda
        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            RellenarAgenda(dataGridView4,Functions.Peluqueros.ToArray());
        }

        //Cargar datos a la agenda
        private void DatosAgenda()
        {
            string fecha = DateTime.Parse(dateTimePicker2.Value.ToString()).ToString("yyyy-MM-dd");
            string nota,peluquero,desde,hasta;

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
                            DataGridViewRow row = dataGridView4.Rows[rowIndex];

                            nota = reader["Nombre"].ToString() + " " + reader["Servicio"].ToString();
                            peluquero = reader["Peluquero"].ToString();
                            desde = reader["Desde"].ToString();
                            hasta = reader["Hasta"].ToString();
                            UbicarDatosAgenda(nota, peluquero, desde, hasta);
                        }
                    }
                }
            }
        }

        //Ubicar datos agenda
        private void UbicarDatosAgenda(string nota,string peluquero,string desde,string hasta)
        {
            
            for(int i = 0; i < dataGridView4.RowCount;i++)
            {
                DataGridViewRow row = dataGridView4.Rows[i];
                if (!row.IsNewRow && desde == row.Cells["Hora"].Value.ToString())
                {
                    row.Cells[peluquero].Value = nota;
                    CambiarColorCeldas(peluquero, desde, hasta);
                    break;
                }
            }
        }

        //Colorear agenda
        private void CambiarColorCeldas(string peluquero, string desde, string hasta)
        {
            for (int i = 0; i < dataGridView4.RowCount; i++)
            {
                DataGridViewRow row = dataGridView4.Rows[i];
                if (!row.IsNewRow && desde == row.Cells["Hora"].Value.ToString())
                {
                    row.Cells[peluquero].Style.BackColor = Color.LightGreen;
                    while (hasta != row.Cells["Hora"].Value.ToString())
                    {
                        i++;
                        row = dataGridView4.Rows[i];
                        if (row.IsNewRow) break;
                        row.Cells[peluquero].Style.BackColor = Color.LightGreen;
                        
                    }
                    break;
                }
            }

        }
        //Menu actualizar
        private void actualizarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Actualizar();
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void PeluquerosReserva_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label19_Click(object sender, EventArgs e)
        {

        }
    }
}