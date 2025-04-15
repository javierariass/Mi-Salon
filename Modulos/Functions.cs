using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows.Forms;


namespace Mi_Salon.Modulos
{
    internal class Functions
    {
        // Lista para almacenar los nombres de los peluqueros
        static public List<string> Peluqueros = new List<string>();
        static public List<string> Servicios = new List<string>();

        static public void Connection(string ruta)
        {
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
                createTableQuery = "CREATE TABLE IF NOT EXISTS Peluqueros (Nombre TEXT,Telefono INTEGER,Correo TEXT)"; 
                command = new SQLiteCommand(createTableQuery, connection);
                command.ExecuteNonQuery();

                //Tabla de Ventas
                createTableQuery = "CREATE TABLE IF NOT EXISTS Ventas (Nombre TEXT,Telefono INTEGER,Correo TEXT,Operacion TEXT,Peluquero TEXT, Rebooking INTEGER,Fecha TEXT)";
                command = new SQLiteCommand(createTableQuery, connection);
                command.ExecuteNonQuery();

                //Tabla de Ausencia de clientes
                createTableQuery = "CREATE TABLE IF NOT EXISTS Ausencia (Nombre TEXT,Telefono INTEGER,Correo TEXT,Operacion TEXT,Peluquero TEXT, Rebooking INTEGER,Fecha TEXT)";
                command = new SQLiteCommand(createTableQuery, connection);
                command.ExecuteNonQuery();

                //Tabla de servicios al cliente
                createTableQuery = "CREATE TABLE IF NOT EXISTS Servicios (Nombre TEXT,Precio TEXT)";
                command = new SQLiteCommand(createTableQuery, connection);
                command.ExecuteNonQuery();

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

        //Guardar en base de datos de peluqueros
        static public void RegistrarPeluquero(string ruta, string nombre, int telefono, string correo)
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={ruta};Version=3;"))
            {
                connection.Open();

                // Consulta para insertar un peluquero
                string insertQuery = "INSERT INTO Peluqueros (Nombre, Telefono, Correo) VALUES (@Nombre, @Telefono, @Correo)";
                SQLiteCommand command = new SQLiteCommand(insertQuery, connection);

                // Agregar los parámetros
                command.Parameters.AddWithValue("@Nombre", nombre);
                command.Parameters.AddWithValue("@Telefono", telefono);
                command.Parameters.AddWithValue("@Correo", correo);

                // Ejecutar la consulta
                command.ExecuteNonQuery();
            }
        }

        //Guardar en base de datos de reserva
        static public void ReservarCita(string ruta,string nombre,int telefono,string correo,string peluquero,string servicio,int rebooking,string fecha,string desde,string hasta)
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

        //Eliminar de la base de datos reserva
        static public bool EliminarReserva(string ruta, string nombre, int telefono, string fecha)
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={ruta};Version=3;"))
            {
                connection.Open();

                string deleteQuery = "DELETE FROM Reservas WHERE Nombre = @Nombre AND Telefono = @Telefono AND Fecha = @Fecha";
                SQLiteCommand command = new SQLiteCommand(deleteQuery, connection);

                command.Parameters.AddWithValue("@Nombre", nombre);
                command.Parameters.AddWithValue("@Telefono", telefono);
                command.Parameters.AddWithValue("@Fecha", fecha);

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

        //Eliminar de la base de datos reserva
        static public bool EliminarTrabajador(string ruta, string nombre, int telefono, string correo)
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={ruta};Version=3;"))
            {
                connection.Open();

                string deleteQuery = "DELETE FROM Peluqueros WHERE Nombre = @Nombre AND Telefono = @Telefono AND Correo = @Correo";
                SQLiteCommand command = new SQLiteCommand(deleteQuery, connection);
                
                try
                {
                    command.Parameters.AddWithValue("@Nombre", nombre);
                    command.Parameters.AddWithValue("@Telefono", telefono);
                    command.Parameters.AddWithValue("@Correo", correo);
                    command.ExecuteNonQuery();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        //Agregar a la base de datos ausencia las reservas del dia que no acudieron a su cita
        static public void CierreDiario(string ruta, List<(string Nombre, int Telefono)> registros)
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={ruta};Version=3;"))
            {
                connection.Open();

                using (SQLiteTransaction transaction = connection.BeginTransaction()) 
                {
                    try
                    {
                        foreach (var registro in registros)
                        {

                            // Obtener los datos de la reserva
                            string selectQuery = "SELECT Nombre, Telefono, Correo,Servicio, Peluquero, Rebooking, Fecha FROM Reservas WHERE Nombre = @Nombre AND Telefono = @Telefono";
                            SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, connection);
                            selectCommand.Parameters.AddWithValue("@Nombre", registro.Nombre);
                            selectCommand.Parameters.AddWithValue("@Telefono", registro.Telefono);

                            SQLiteDataReader reader = selectCommand.ExecuteReader();

                            if (reader.Read()) // Si encontramos la reserva
                            {
                                // Insertar los valores en la tabla Ausencia
                                string insertQuery = "INSERT INTO Ausencia (Nombre, Telefono, Correo,Operacion, Peluquero, Rebooking, Fecha) " +
                                    "VALUES (@Nombre, @Telefono, @Correo,@Operacion, @Peluquero, @Rebooking, @Fecha)";
                                SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, connection);

                                insertCommand.Parameters.AddWithValue("@Nombre", reader["Nombre"]);
                                insertCommand.Parameters.AddWithValue("@Telefono", reader["Telefono"]);
                                insertCommand.Parameters.AddWithValue("@Correo", reader["Correo"]);
                                insertCommand.Parameters.AddWithValue("@Operacion", reader["Servicio"]);
                                insertCommand.Parameters.AddWithValue("@Peluquero", reader["Peluquero"]);
                                insertCommand.Parameters.AddWithValue("@Rebooking", reader["Rebooking"]);
                                insertCommand.Parameters.AddWithValue("@Fecha", reader["Fecha"]);

                                insertCommand.ExecuteNonQuery();
                            }

                            reader.Close();

                            //Eliminar de la base de datos Pendiente
                            /*
                                                        // Consulta para eliminar cada registro
                                                        string deleteQuery = "DELETE FROM Reservas WHERE Nombre = @Nombre AND Telefono = @Telefono";
                                                        SQLiteCommand command = new SQLiteCommand(deleteQuery, connection);

                                                        // Agregar los parámetros
                                                        command.Parameters.AddWithValue("@Nombre", registro.Nombre);
                                                        command.Parameters.AddWithValue("@Telefono", registro.Telefono);

                                                        // Ejecutar la consulta
                                                        command.ExecuteNonQuery();
                            */
                        }


                        // Confirmar la transacción
                        transaction.Commit();
                        MessageBox.Show("Cierre diario realizado con exito", "Aviso");
                    }
                    catch 
                    {
                        // Revertir la transacción si algo falla
                        transaction.Rollback();
                        MessageBox.Show("Cierre diario fallido", "Aviso");
                    }
                }
            }
        }
    }


}
