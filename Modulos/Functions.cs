﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.Remoting.Contexts;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace Mi_Salon.Modulos
{
    internal class Functions
    {
        // Lista para almacenar los nombres de los peluqueros
        static public List<string> Peluqueros = new List<string>();
        static public List<string> Servicios = new List<string>();

        static public void Connection(string ruta)
        {
            Peluqueros.Clear();
            Servicios.Clear();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={ruta};Version=3;"))
            {
                connection.Open();

                //Creacion de tablas

                //Tabla de reservas
                string createTableQuery = "CREATE TABLE IF NOT EXISTS Reservas (Nombre TEXT,Telefono INTEGER,Correo TEXT,Peluquero TEXT,Servicio TEXT, " +
                    "Rebooking INTEGER,Fecha TEXT,Desde TEXT,Hasta TEXT,Asistencia INTEGER)"; //Rebooking 1- true 0- false
                SQLiteCommand command = new SQLiteCommand(createTableQuery, connection);
                command.ExecuteNonQuery();

                //Tabla de peluqueros
                createTableQuery = "CREATE TABLE IF NOT EXISTS Peluqueros (Nombre TEXT,Telefono INTEGER)";
                command = new SQLiteCommand(createTableQuery, connection);
                command.ExecuteNonQuery();

                //Tabla de Ventas
                createTableQuery = "CREATE TABLE IF NOT EXISTS Ventas (Nombre TEXT,Telefono INTEGER,Correo TEXT,Operacion TEXT,Peluquero TEXT, " +
                    "Rebooking INTEGER,Fecha TEXT)";
                command = new SQLiteCommand(createTableQuery, connection);
                command.ExecuteNonQuery();

                //Tabla de clientes nuevos
                createTableQuery = "CREATE TABLE IF NOT EXISTS NuevosClientes (Nombre TEXT,Telefono INTEGER,Correo TEXT,Peluquero TEXT,Fecha TEXT)";
                command = new SQLiteCommand(createTableQuery, connection);
                command.ExecuteNonQuery();

                //Tabla de servicios al cliente
                createTableQuery = "CREATE TABLE IF NOT EXISTS Servicios (Nombre TEXT,Precio TEXT)";
                command = new SQLiteCommand(createTableQuery, connection);
                command.ExecuteNonQuery();

                //Tabla de facturas
                createTableQuery = "CREATE TABLE IF NOT EXISTS Facturas (Id TEXT, Cliente TEXT,Peluquero TEXT,Total TEXT,Fecha TEXT)";
                command = new SQLiteCommand(createTableQuery, connection);
                command.ExecuteNonQuery();

                //Tabla de productos de las facturas
                createTableQuery = "CREATE TABLE IF NOT EXISTS Comprobantes (Factura INTEGER, Nombre TEXT,Precio TEXT)";
                command = new SQLiteCommand(createTableQuery, connection);
                command.ExecuteNonQuery();

                //---------------------------------------------------------------------------------------------------------------------------------------------------------
                //Registro de Datos
                string query = $"SELECT COUNT(*) FROM Servicios;";

                using (SQLiteCommand comando = new SQLiteCommand(query, connection))
                {
                    int cantidadRegistros = Convert.ToInt32(comando.ExecuteScalar());
                    if(cantidadRegistros == 0)
                    {
                        RegistrarServicio(ruta, "Corte", "2000.00");
                        RegistrarServicio(ruta, "Peinado", "2800.00");
                        RegistrarServicio(ruta, "Tinte", "5200.00");
                        RegistrarServicio(ruta, "Mechas", "14800.00");
                        RegistrarServicio(ruta, "Balayage", "26800.00");
                        RegistrarServicio(ruta, "Decoloración", "10800.00");
                        RegistrarServicio(ruta, "Derriz", "8000.00");
                        RegistrarServicio(ruta, "Peinado Extensiones", "4400.00");
                        RegistrarServicio(ruta, "Keratinas", "16000.00");
                        RegistrarServicio(ruta, "Braziliam", "28000.00");
                        RegistrarServicio(ruta, "Adicional tinte", "4000.00");
                        RegistrarServicio(ruta, "Adicional Decoración", "7200.00");
                        RegistrarServicio(ruta, "Retoque extensiones", "200.00");
                        RegistrarServicio(ruta, "Tratamiento 6000", "6000.00");
                        RegistrarServicio(ruta, "Tratamiento 8000", "8000.00");
                        RegistrarServicio(ruta, "Tratamiento 10000", "10000.00");
                        RegistrarServicio(ruta, "Tratamiento 20000", "20000.00");
                    }
                }

                //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                //Extraccion de datos

                // Consulta para obtener los nombres de la tabla Peluqueros
                string selectQuery = "SELECT Nombre FROM Peluqueros";

                command = new SQLiteCommand(selectQuery, connection);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    // Agregar los nombres a la lista
                    Peluqueros.Add(reader["Nombre"].ToString());
                }

                // Consulta para obtener los nombres de la tabla Servicios
                selectQuery = "SELECT Nombre FROM Servicios";

                command = new SQLiteCommand(selectQuery, connection);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    // Agregar los nombres a la lista
                    Servicios.Add(reader["Nombre"].ToString());
                }
            }

        }

        //Guardar en base de datos de servicios
        static public void RegistrarServicio(string ruta, string nombre, string precio)
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={ruta};Version=3;"))
            {
                connection.Open();

                // Consulta para insertar un peluquero
                string insertQuery = "INSERT INTO Servicios (Nombre, Precio) VALUES (@Nombre, @Precio)";
                SQLiteCommand command = new SQLiteCommand(insertQuery, connection);

                // Agregar los parámetros
                command.Parameters.AddWithValue("@Nombre", nombre);
                command.Parameters.AddWithValue("@Precio", precio);

                // Ejecutar la consulta
                command.ExecuteNonQuery();
            }
        }

        //Guardar en base de datos de peluqueros
        static public void RegistrarPeluquero(string ruta, string nombre, int telefono)
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={ruta};Version=3;"))
            {
                connection.Open();

                // Consulta para insertar un peluquero
                string insertQuery = "INSERT INTO Peluqueros (Nombre, Telefono) VALUES (@Nombre, @Telefono)";
                SQLiteCommand command = new SQLiteCommand(insertQuery, connection);

                // Agregar los parámetros
                command.Parameters.AddWithValue("@Nombre", nombre);
                command.Parameters.AddWithValue("@Telefono", telefono);

                // Ejecutar la consulta
                command.ExecuteNonQuery();
            }
        }

        //Guardar en base de datos de reserva
        static public void ReservarCita(string ruta, string nombre, int telefono, string correo, string peluquero, string servicio, int rebooking, string fecha, string desde, string hasta)
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={ruta};Version=3;"))
            {
                connection.Open();

                // Consulta para insertar un peluquero
                string insertQuery = "INSERT INTO Reservas (Nombre, Telefono, Correo,Peluquero,Servicio,Rebooking,Fecha,Desde,Hasta,Asistencia) VALUES (@Nombre, @Telefono, @Correo," +
                    "@Peluquero,@Servicio,@Rebooking, @Fecha,@Desde, @Hasta,@Asistencia)";
                SQLiteCommand command = new SQLiteCommand(insertQuery, connection);

                // Agregar los parámetros
                command.Parameters.AddWithValue("@Nombre", nombre);
                command.Parameters.AddWithValue("@Telefono", telefono);
                command.Parameters.AddWithValue("@Correo", correo);
                command.Parameters.AddWithValue("@Peluquero", peluquero);
                command.Parameters.AddWithValue("@Servicio", servicio);
                command.Parameters.AddWithValue("@Rebooking", rebooking);
                command.Parameters.AddWithValue("@Fecha", fecha);
                command.Parameters.AddWithValue("@Desde", desde);
                command.Parameters.AddWithValue("@Hasta", hasta);
                command.Parameters.AddWithValue("@Asistencia", 0);

                // Ejecutar la consulta
                command.ExecuteNonQuery();
            }
        }

        //Guardar en base de datos facturas
        static public void RegistrarFactura(string ruta,List<string> peluqueros, string cliente,string fecha,List<double> precios)
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={ruta};Version=3;"))
            {
                connection.Open();
                List<string> nombres = new List<string>();
                double valor = 0.00;

                foreach (string name in peluqueros)
                {
                    valor = 0.00;
                    if (nombres.Contains(name)) continue;
                    nombres.Add(name);
                    for (int i = 0; i < peluqueros.Count; i++)
                    {
                        if (name == peluqueros[i])
                        {
                            valor += precios[i];
                        }
                    }

                    // Consulta para insertar un peluquero
                    string insertQuery = "INSERT INTO Facturas (Id,Cliente, Peluquero,Total, Fecha) VALUES (@Id, @Cliente, @Peluquero,@Total,@Fecha)";
                    SQLiteCommand command = new SQLiteCommand(insertQuery, connection);

                    // Agregar los parámetros
                    command.Parameters.AddWithValue("@Id", GenerarCodigoFactura());
                    command.Parameters.AddWithValue("@Cliente", cliente);
                    command.Parameters.AddWithValue("@Peluquero", name);
                    command.Parameters.AddWithValue("@Total", valor);
                    command.Parameters.AddWithValue("@Fecha", fecha);

                    // Ejecutar la consulta
                    command.ExecuteNonQuery();
                }              
            }

            //Codigo de factura
            string GenerarCodigoFactura()
            {
                // Crear una instancia de Random
                Random random = new Random();

                // Generar letras aleatorias (dos caracteres)
                char letra1 = (char)random.Next('A', 'Z' + 1);
                char letra2 = (char)random.Next('A', 'Z' + 1);

                // Generar números aleatorios (4 dígitos)
                int numeros = random.Next(1000, 9999);

                // Combinar las letras y números en un código
                return $"{letra1}{letra2}-{numeros}";
            }
        }

        //Eliminar reserva de la base de datos 
        static public bool EliminarReserva(string ruta, string nombre, int telefono, string fecha,string servicio)
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={ruta};Version=3;"))
            {
                connection.Open();

                string deleteQuery = "DELETE FROM Reservas WHERE Nombre = @Nombre AND Telefono = @Telefono AND Fecha = @Fecha AND Servicio = @Servicio";
                SQLiteCommand command = new SQLiteCommand(deleteQuery, connection);

                command.Parameters.AddWithValue("@Nombre", nombre);
                command.Parameters.AddWithValue("@Telefono", telefono);
                command.Parameters.AddWithValue("@Fecha", fecha);
                command.Parameters.AddWithValue("@Servicio", servicio);

                try
                {
                    command.ExecuteNonQuery();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        //Eliminar trabajador de la base de datos 
        static public bool EliminarTrabajador(string ruta, string nombre, int telefono)
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={ruta};Version=3;"))
            {
                connection.Open();

                string deleteQuery = "DELETE FROM Peluqueros WHERE Nombre = @Nombre AND Telefono = @Telefono";
                SQLiteCommand command = new SQLiteCommand(deleteQuery, connection);

                try
                {
                    command.Parameters.AddWithValue("@Nombre", nombre);
                    command.Parameters.AddWithValue("@Telefono", telefono);
                    command.ExecuteNonQuery();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        //Ver si el cliente es nuevo
        static public bool IsNewClient(string rutaBD, string nombre)
        {
            bool existe = false;
            string conexionString = $"Data Source={rutaBD};Version=3;";

            using (SQLiteConnection conexion = new SQLiteConnection(conexionString))
            {
                conexion.Open();
                string query = "SELECT COUNT(*) FROM NuevosClientes WHERE Nombre = @Nombre";

                using (SQLiteCommand comando = new SQLiteCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@Nombre", nombre);
                    existe = Convert.ToInt32(comando.ExecuteScalar()) > 0;
                }
            }

            return existe;
        }

        //Guardar nuevo cliente
        static public void RegisterNewClient(string ruta, string nombre, int telefono, string correo, string peluquero, string fecha)
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={ruta};Version=3;"))
            {
                connection.Open();

                // Consulta para insertar un peluquero
                string insertQuery = "INSERT INTO NuevosClientes (Nombre, Telefono, Correo,Peluquero,Fecha) VALUES (@Nombre, @Telefono, @Correo," +
                    "@Peluquero, @Fecha)";
                SQLiteCommand command = new SQLiteCommand(insertQuery, connection);

                // Agregar los parámetros
                command.Parameters.AddWithValue("@Nombre", nombre);
                command.Parameters.AddWithValue("@Telefono", telefono);
                command.Parameters.AddWithValue("@Correo", correo);
                command.Parameters.AddWithValue("@Peluquero", peluquero);
                command.Parameters.AddWithValue("@Fecha", fecha);

                // Ejecutar la consulta
                command.ExecuteNonQuery();
            }
        }

        //Obtencion de nuevos clientes
        static public List<(string Nombre, string Peluquero, string Fecha)> ObtenerClientes(string rutaBD, DateTime inicio, DateTime fin)
        {
            var resultados = new List<(string Nombre, string Peluquero, string Fecha)>();
            string conexionString = $"Data Source={rutaBD};Version=3;";

            using (SQLiteConnection conexion = new SQLiteConnection(conexionString))
            {
                conexion.Open();
                string query = @"SELECT Nombre, Peluquero, Fecha FROM NuevosClientes WHERE Fecha BETWEEN @fechaInicio AND @fechaFin;";

                using (SQLiteCommand comando = new SQLiteCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@fechaInicio", inicio.ToString("yyyy-MM-dd"));
                    comando.Parameters.AddWithValue("@fechaFin", fin.ToString("yyyy-MM-dd"));

                    using (SQLiteDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            resultados.Add((reader["Nombre"].ToString(), reader["Peluquero"].ToString(), reader["Fecha"].ToString()));
                        }
                    }
                }
            }

            return resultados;
        }

        //Obtencion de rebooking
        static public List<(string Peluquero, string TotalReservas, string PorcentajeRebooking)> ObtenerRebooking(string rutaBD, DateTime inicio, DateTime fin)
        {
            var resultados = new List<(string Peluquero, string TotalReservas, string PorcentajeRebooking)>();
            string conexionString = $"Data Source={rutaBD};Version=3;";

            using (SQLiteConnection conexion = new SQLiteConnection(conexionString))
            {
                conexion.Open();

                string query = @"SELECT Peluquero,COUNT(*) AS TotalReservas,SUM(CASE WHEN Rebooking = 1 THEN 1 ELSE 0 END) AS RebookingCount FROM Reservas WHERE Fecha BETWEEN @fechaInicio AND @fechaFin GROUP BY Peluquero;";

                using (SQLiteCommand comando = new SQLiteCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@fechaInicio", inicio.ToString("yyyy-MM-dd"));
                    comando.Parameters.AddWithValue("@fechaFin", fin.ToString("yyyy-MM-dd"));

                    using (SQLiteDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string peluquero = reader["Peluquero"].ToString();
                            int totalReservas = Convert.ToInt32(reader["TotalReservas"]);
                            int rebookingCount = Convert.ToInt32(reader["RebookingCount"]);

                            // Calcular el porcentaje de Rebooking
                            string porcentaje = totalReservas > 0
                                ? ((rebookingCount / (double)totalReservas) * 100).ToString("0.00") + "%"
                                : "0.00%";

                            resultados.Add((peluquero, totalReservas.ToString(), porcentaje));
                        }
                    }
                }
            }

            return resultados;
        }

        //Obtencion de ventas
        static public List<(string Peluquero, string Cliente,string Total, string Fecha)> ObtenerVentasPorPeluquero(string rutaBD, 
            DateTime inicio, DateTime fin)
        {
            var resultados = new List<(string Peluquero, string Cliente,string Total, string Fecha)>();
            string connection = $"Data Source={rutaBD};Version=3;";

            using (SQLiteConnection conexion = new SQLiteConnection(connection))
            {
                conexion.Open();

                string query = @"SELECT Peluquero, Cliente, Fecha, Total FROM Facturas WHERE Fecha BETWEEN @fechaInicio AND @fechaFin;";

                using (SQLiteCommand comando = new SQLiteCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@fechaInicio", inicio.ToString("yyyy-MM-dd"));
                    comando.Parameters.AddWithValue("@fechaFin", fin.ToString("yyyy-MM-dd"));

                    using (SQLiteDataReader reader = comando.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string peluquero = reader["Peluquero"].ToString();
                            string cliente = reader["Cliente"].ToString();
                            string fecha = reader["Fecha"].ToString();
                            string total = reader["Total"].ToString();

                            resultados.Add((peluquero, cliente,total, fecha));
                        }
                    }
                }
            }
            return resultados;
        }

        //Obtencion del porciento de retenencion de clientes 
        


        //Obtencion de clientes que vuelven en un periodo
    }
}
