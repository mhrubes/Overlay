using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverlayWPF
{
    class DatabaseManager
    {
        private SQLiteConnection connection;

        public DatabaseManager()
        {
            connection = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
            connection.Open();

            string createTableQuery = "CREATE TABLE IF NOT EXISTS Data (Id INTEGER PRIMARY KEY, Path TEXT)";
            ExecuteNonQuery(createTableQuery);
        }

        public string GetFirstFilePath()
        {
            string query = "SELECT Path FROM Data LIMIT 1";
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    return result.ToString();
                }
                else
                {
                    return "Database is empty.";
                }
            }
        }

        public void InsertFilePath(string filePath)
        {
            string selectQuery = "SELECT Id FROM Data ORDER BY Id LIMIT 1";
            using (SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, connection))
            {
                object result = selectCommand.ExecuteScalar();
                if (result != null)
                {
                    int firstId = Convert.ToInt32(result);
                    string updateQuery = "UPDATE Data SET Path = @Path WHERE Id = @Id";
                    using (SQLiteCommand updateCommand = new SQLiteCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@Id", firstId);
                        updateCommand.Parameters.AddWithValue("@Path", filePath);
                        updateCommand.ExecuteNonQuery();
                    }
                }
                else
                {
                    string insertQuery = "INSERT INTO Data (Path) VALUES (@Path)";
                    using (SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@Path", filePath);
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }



        private void ExecuteNonQuery(string query)
        {
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        private void ExecuteNonQueryWithParameter(string query, string parameterName, string parameterValue)
        {
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue(parameterName, parameterValue);
                command.ExecuteNonQuery();
            }
        }
    }
}
