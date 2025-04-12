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

        static public void Connection(string ruta)
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={ruta};Version=3;"))
            {
                connection.Open();

                //Creacion de tablas

                //Tabla de reservas
                string createTableQuery = "CREATE TABLE IF NOT EXISTS Reservas (Nombre TEXT,Telefono INTEGER,Correo TEXT,Peluquero TEXT, Rebooking INTEGER,Fecha TEXT)"; //Rebooking 1- true 0- false
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

                //Tabla de Ventas
                createTableQuery = "CREATE TABLE IF NOT EXISTS Ausencia (Nombre TEXT,Telefono INTEGER,Correo TEXT,Operacion TEXT,Peluquero TEXT, Rebooking INTEGER,Fecha TEXT)";
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
        static public void ReservarCita(string ruta,string nombre,int telefono,string correo,string peluquero,int rebooking,string fecha)
        {
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={ruta};Version=3;"))
            {
                connection.Open();

                // Consulta para insertar un peluquero
                string insertQuery = "INSERT INTO Reservas (Nombre, Telefono, Correo,Peluquero,Rebooking,Fecha) VALUES (@Nombre, @Telefono, @Correo,@Peluquero,@Rebooking, @Fecha)";
                SQLiteCommand command = new SQLiteCommand(insertQuery, connection);

                // Agregar los parámetros
                command.Parameters.AddWithValue("@Nombre", nombre);
                command.Parameters.AddWithValue("@Telefono", telefono);
                command.Parameters.AddWithValue("@Correo", correo);
                command.Parameters.AddWithValue("@Peluquero", peluquero);
                command.Parameters.AddWithValue("@Rebooking", rebooking);
                command.Parameters.AddWithValue("@Fecha", fecha);

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
            return false;
        }

        //Eliminar de la base de datos las reservas del dia que no acudieron a su cita
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
                            // Consulta para eliminar cada registro
                            string deleteQuery = "DELETE FROM Reservas WHERE Nombre = @Nombre AND Telefono = @Telefono";
                            SQLiteCommand command = new SQLiteCommand(deleteQuery, connection);

                            // Agregar los parámetros
                            command.Parameters.AddWithValue("@Nombre", registro.Nombre);
                            command.Parameters.AddWithValue("@Telefono", registro.Telefono);

                            // Ejecutar la consulta
                            command.ExecuteNonQuery();
                        }

                        // Confirmar la transacción
                        transaction.Commit();
                    }
                    catch 
                    {
                        // Revertir la transacción si algo falla
                        transaction.Rollback();
                        MessageBox.Show("SAD");
                    }
                }
            }
        }
    }


}
