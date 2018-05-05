using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Npgsql;
namespace SearchBotUpdated.Database
{
    public class DatabaseHelper
    {// Database connection settings.
        private const String SERVER = "118.200.28.48";
        private const String PORT = "5432";
        private const String USER = "postgres";
        private const String PASSWORD = "";
        private const String DATABASE = "PSA";
        public NpgsqlConnection connection = null;

        // Constructor.
        public DatabaseHelper()
        {
            // Create connection object.
            connection = new NpgsqlConnection(
                "Server=" + SERVER + ";" +
                "Port=" + PORT + ";" +
                "User Id=" + USER + ";" +
                "Password=" + PASSWORD + ";" +
                "Database=" + DATABASE + ";"
            );
        }
        // Open database connection.
        public void openConnection()
        {
            try
            {
                connection.Open();
            }
            catch (NpgsqlException ex)
            {
                showError(ex);
            }
        }

        // Close database connection.
        public void closeConnection()
        {
            try
            {
                connection.Close();
            }
            catch (NpgsqlException ex)
            {
                showError(ex);
            }
        }
        // Show error to message box.
        private void showError(NpgsqlException ex)
        {
            //MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK,
            //    MessageBoxIcon.Error);
        }
        public int get()
        {
            openConnection();
            int result = 0;

            try
            {
           
                // Create select command.
                NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM public.company ORDER BY id ASC", connection);

                // Prepare the command.
                command.Prepare();

                // Execute SQL command.
                NpgsqlDataReader dr = command.ExecuteReader();

                // Fill results to music list.
                while (dr.Read())
                {
                    result = 1;
                   // result++;
                }
                

            }
            catch (NpgsqlException ex)
            {
                showError(ex);
                result = 2;

            }

            closeConnection();

            return result;
        }
    }
}