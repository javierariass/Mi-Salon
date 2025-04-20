using Mi_Salon.Modulos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using System.Security.Cryptography;
using System.Security.Policy;
using static System.Net.WebRequestMethods;

namespace Mi_Salon
{
    public partial class MiSalon : Form
    {
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MiSalon.db";
        List<string> NCliente = new List<string>();
        List<string> servicios = new List<string>();
        List<string> peluqueros = new List<string>();
        List<double> precios = new List<double>();
        double total = 0.00;

        public MiSalon()
        {
            InitializeComponent();
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
            Functions.Connection(appDataPath);
            ClientesEnBase();
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd"); // Standardized date format
            label7.Text = currentDate; // Ensure label uses the same format as the database

            RellenarReservasDelDia(currentDate);
            RellenarReservas();

            //Establecer la agenda
            RellenarAgenda(dataGridView4, Functions.Peluqueros.ToArray());

            // Peluqueros con los que puede reservar
            PeluquerosReserva.Items.Clear();
            PeluquerosReserva.Items.AddRange(Functions.Peluqueros.ToArray());
            if (Functions.Peluqueros.Count != 0) PeluquerosReserva.SelectedIndex = 0;

            comboBox2.Items.Clear();
            comboBox2.Items.AddRange(Functions.Peluqueros.ToArray());
            if (Functions.Peluqueros.Count != 0) comboBox2.SelectedIndex = 0;

            comboBox11.Items.Clear();
            comboBox11.Items.AddRange(Functions.Peluqueros.ToArray());
            if (Functions.Peluqueros.Count != 0) comboBox11.SelectedIndex = 0;

            //Servicios que puede solicitar
            comboBox9.Items.Clear();
            comboBox9.Items.AddRange(Functions.Servicios.ToArray());
            if (Functions.Servicios.Count != 0) comboBox9.SelectedIndex = 0;
            comboBox3.Items.Clear();
            comboBox3.Items.AddRange(Functions.Servicios.ToArray());
            if (Functions.Servicios.Count != 0) comboBox3.SelectedIndex = 0;

            comboBox10.Items.Clear();
            comboBox10.Items.AddRange(Functions.Servicios.ToArray());
            if (Functions.Servicios.Count != 0) comboBox10.SelectedIndex = 0;


            // Clientes para facturar
            comboBox1.Items.Clear();
            List<string> nombres = new List<string>();
            foreach (string name in NCliente)
            {
                if (nombres.Contains(name)) continue;
                nombres.Add(name);
                comboBox1.Items.Add(name);
            }            
            if (NCliente.Count != 0) comboBox1.SelectedIndex = 0;

            //Establecer Hora
            comboBox5.SelectedIndex = 0;
            comboBox6.SelectedIndex = 0;
            comboBox7.SelectedIndex = 0;
            comboBox8.SelectedIndex = 1;
            comboBox12.SelectedIndex = 0;
            comboBox13.SelectedIndex = 0;
            comboBox14.SelectedIndex = 0;
            comboBox15.SelectedIndex = 1;
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
                            row.Cells["Asistencia"].Value = int.Parse(reader["Asistencia"].ToString()) == 1;
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
                            row.Cells["AsistenciaR"].Value = int.Parse(reader["Asistencia"].ToString()) == 1;
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
                if (!string.IsNullOrWhiteSpace(textBox1.Text) && !string.IsNullOrWhiteSpace(textBox2.Text))
                {
                    string desde = comboBox5.Text + ":" + comboBox6.Text;
                    string hasta = comboBox7.Text + ":" + comboBox8.Text;
                    desde = int.Parse(comboBox5.Text) >= 10 && int.Parse(comboBox5.Text) < 12 ? desde + " a.m." : desde + " p.m.";
                    hasta = int.Parse(comboBox7.Text) >= 10 && int.Parse(comboBox7.Text) < 12 ? hasta + " a.m." : hasta + " p.m";
                    string date = DateTime.Parse(dateTimeReservation.Text).ToString("yyyy-MM-dd");
                    string peluquero = PeluquerosReserva.SelectedItem.ToString();
                    int number = int.Parse(textBox2.Text);
                    string correo = !string.IsNullOrWhiteSpace(textBox3.Text) ?  textBox3.Text : " ";
                    string nombre =  textBox1.Text;

                    Functions.ReservarCita(appDataPath, nombre, number, correo, peluquero, comboBox9.Text, 0, date, desde, hasta);
                    MessageBox.Show("Reserva realizada", "Aviso");

                    if (!Functions.IsNewClient(appDataPath, textBox1.Text))
                    {
                        Functions.RegisterNewClient(appDataPath, nombre, number, correo, peluquero, date);
                    }
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
                string servicio = dataGridView3.SelectedRows[0].Cells["ServicioR"].Value.ToString();

                if (Functions.EliminarReserva(appDataPath, nombre, telefono, fecha,servicio))
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

        private void dataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        //Actualizar Agenda
        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            RellenarAgenda(dataGridView4, Functions.Peluqueros.ToArray());
        }

        //Cargar datos a la agenda
        private void DatosAgenda()
        {
            string fecha = DateTime.Parse(dateTimePicker2.Value.ToString()).ToString("yyyy-MM-dd");
            string nota, peluquero, desde, hasta;

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
        private void UbicarDatosAgenda(string nota, string peluquero, string desde, string hasta)
        {
            bool existe = false;
            dataGridView4.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            for (int i = 0; i < dataGridView4.RowCount; i++)
            {
                DataGridViewRow row = dataGridView4.Rows[i];

                if (!row.IsNewRow && desde == row.Cells["Hora"].Value.ToString())
                {
                    foreach (DataGridViewColumn columna in dataGridView4.Columns)
                    {
                        if (columna.Name == peluquero)
                        {
                            existe = true;
                            if (row.Cells[peluquero].Value == null) row.Cells[peluquero].Value = nota;
                            else
                            {
                                row.Cells[peluquero].Value = row.Cells[peluquero].Value.ToString() + "\n" + nota;
                            }
                            CambiarColorCeldas(peluquero, desde, hasta);
                            break;
                        }
                    }
                    if (existe) break;
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

        //Menu de registro de trabajadores
        private void registroDeTrabajadoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Registro_Trabajadores Registro = new Registro_Trabajadores();
            Registro.ShowDialog();
            Actualizar();
        }

        private void informacionGeneralToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Informacion_General info = new Informacion_General();
            info.ShowDialog();
        }

        //Menu de reportes
        private void reportesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Reporte reporte = new Reporte();
            reporte.ShowDialog();
        }
     
        //Funcion de reserva de rebooking
        private void ReservarRebooking()
        {
            string desde = comboBox12.Text + ":" + comboBox13.Text;
            string hasta = comboBox14.Text + ":" + comboBox15.Text;
            desde = int.Parse(comboBox5.Text) >= 10 ? desde + " a.m." : desde + " p.m.";
            hasta = int.Parse(comboBox7.Text) >= 10 ? hasta + " a.m." : hasta + " p.m.";
            string nombre = comboBox1.Text;
            string peluquero = comboBox11.Text;
            string servicio = comboBox10.Text;
            string date = DateTime.Parse(dateTimePicker1.Text).ToString("yyyy-MM-dd");
            int number = 0;
            string correo = "";

            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={appDataPath};Version=3;"))
            {
                connection.Open();

                string selectQuery = "SELECT Telefono, Correo FROM Reservas WHERE Fecha = @Fecha";

                using (SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, connection))
                {
                    selectCommand.Parameters.AddWithValue("@Fecha", DateTime.Now.Date.ToString("yyyy-MM-dd"));

                    using (SQLiteDataReader reader = selectCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Asignar los valores obtenidos
                            number = Convert.ToInt32(reader["Telefono"]);
                            correo = reader["Correo"].ToString();
                        }
                    }
                }
            }

            Functions.ReservarCita(appDataPath, nombre, number, correo, peluquero, servicio, 1, date, desde, hasta);
            MessageBox.Show("Reserva realizada.", "Alerta");
            Actualizar();
        }
     

        //Boton de Guardado de la factura
        private void button7_Click(object sender, EventArgs e)
        {
            string nombre = comboBox1.Text;
            Functions.RegistrarFactura(appDataPath, peluqueros, nombre, DateTime.Now.ToString("yyyy-MM-dd"), precios);
            if (checkBox1.Checked) ReservarRebooking();
            dataGridView2.Rows.Clear();
            textBox4.Text = "0.00 CUP";
            peluqueros.Clear();
            precios.Clear();
            servicios.Clear();
        }

        //Boton de factura
        private void button5_Click(object sender, EventArgs e)
        {
            total = 0.00;

            dataGridView2.Rows.Clear();
            string cadenaConexion = $"Data Source={appDataPath};Version=3;";
            string nombre = comboBox1.Text;
            

            foreach (DataGridViewRow fila in dataGridView1.Rows)
            {
                if (fila.Cells["Nombre"].Value != null && fila.Cells["Nombre"].Value.ToString() == nombre)
                {
                    servicios.Add(fila.Cells["Serv"].Value.ToString());
                    peluqueros.Add(fila.Cells["PeluqueroReservado"].Value.ToString());

                    using (SQLiteConnection conexion = new SQLiteConnection(cadenaConexion))
                    {
                        conexion.Open();

                        string consulta = "SELECT Precio FROM Servicios WHERE Nombre = @nombre";

                        using (SQLiteCommand comando = new SQLiteCommand(consulta, conexion))
                        {
                            comando.Parameters.AddWithValue("@nombre", fila.Cells["Serv"].Value.ToString());

                            object resultado = comando.ExecuteScalar();
                            precios.Add(double.Parse(resultado.ToString()));
                        }
                    }
                }
            }

            //Total de precio
            foreach (double valor in precios)
            {
                total += valor;
            }
            textBox4.Text = total.ToString() + " CUP";

            //Ubicar en la tabla de factura
            for (int i = 0; i < servicios.Count; i++)
            {
                int rowIndex = dataGridView2.Rows.Add();
                DataGridViewRow row = dataGridView2.Rows[rowIndex];
                row.Cells["Peluquero"].Value = peluqueros[i];
                row.Cells["Servicio"].Value = servicios[i];
                row.Cells["Precio"].Value = precios[i] + " CUP";


                //Actualizar base de datos
                using (SQLiteConnection conexion = new SQLiteConnection(cadenaConexion))
                {
                    conexion.Open();

                    string consulta = "UPDATE Reservas SET Asistencia = @asistencia WHERE Nombre = @nombre AND Servicio = @servicio";

                    using (SQLiteCommand comando = new SQLiteCommand(consulta, conexion))
                    {
                        comando.Parameters.AddWithValue("@asistencia", 1);
                        comando.Parameters.AddWithValue("@nombre", nombre);
                        comando.Parameters.AddWithValue("@servicio", servicios[i]);

                        comando.ExecuteNonQuery();

                    }
                }
            }          
            Actualizar();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            servicios.Add(comboBox3.Text);
            peluqueros.Add(comboBox2.Text);
            double precio;

            string cadenaConexion = $"Data Source={appDataPath};Version=3;";
            string nombre = comboBox1.Text;
            using (SQLiteConnection conexion = new SQLiteConnection(cadenaConexion))
            {
                conexion.Open();

                string consulta = "SELECT Precio FROM Servicios WHERE Nombre = @nombre";

                using (SQLiteCommand comando = new SQLiteCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@nombre", comboBox3.Text);

                    object resultado = comando.ExecuteScalar();
                    precios.Add(double.Parse(resultado.ToString()));
                    precio = double.Parse(resultado.ToString());
                }
            }
            int rowIndex = dataGridView2.Rows.Add();
            DataGridViewRow row = dataGridView2.Rows[rowIndex];
            row.Cells["Peluquero"].Value = comboBox2.Text;
            row.Cells["Servicio"].Value = comboBox3.Text;
            row.Cells["Precio"].Value = precio + " CUP";
            total += precio;
            textBox4.Text = total.ToString() + " CUP";

        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView2.Rows.Clear();
            textBox4.Text = "0.00 CUP";
            peluqueros.Clear();
            precios.Clear();
            servicios.Clear();
            total = 0.00;
        }

        private void ClientesEnBase()
        {
            string cadenaConexion = $"Data Source={appDataPath};Version=3;";
            dataGridView5.Rows.Clear();
            using (SQLiteConnection conexion = new SQLiteConnection(cadenaConexion))
            {
                conexion.Open();

                string consulta = "SELECT * FROM NuevosClientes ";

                using (SQLiteCommand comando = new SQLiteCommand(consulta, conexion))
                {
                    using (SQLiteDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int rowIndex = dataGridView5.Rows.Add();
                            DataGridViewRow row = dataGridView5.Rows[rowIndex];

                            row.Cells["NameA"].Value = reader["Nombre"];
                            row.Cells["TelefonoA"].Value = reader["Telefono"];
                            row.Cells["CorreoA"].Value = reader["Correo"];
                        }
                    }

                }
            }

        }

        private void label27_Click(object sender, EventArgs e)
        {

        }
    }
}
