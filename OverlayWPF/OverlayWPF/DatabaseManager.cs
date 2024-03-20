using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Input;

namespace OverlayWPF
{
    class DatabaseManager
    {
        private SQLiteConnection connection;

        public DatabaseManager()
        {
            connection = new SQLiteConnection("Data Source=WpfDatabase.sqlite;Version=3;");
            connection.Open();

            string createTableQuery = "CREATE TABLE IF NOT EXISTS Data (Id INTEGER PRIMARY KEY, Path TEXT, KeyOne TEXT, KeyTwo TEXT)";
            ExecuteNonQuery(createTableQuery);

            if (!IsInsertionDone())
            {
                InsertData(string.Empty, string.Empty, string.Empty);
                InsertData(string.Empty, "Q", string.Empty);
                InsertData(string.Empty, "Alt", "Q");
                InsertData(string.Empty, "Alt", "H");
            }
        }

        public void InsertData(string filePath, string keyOne, string keyTwo)
        {
            string insertQuery = "INSERT INTO Data (Path, KeyOne, KeyTwo) VALUES (@Path, @KeyOne, @KeyTwo)";
            using (SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, connection))
            {
                insertCommand.Parameters.AddWithValue("@Path", filePath);
                insertCommand.Parameters.AddWithValue("@KeyOne", keyOne);
                insertCommand.Parameters.AddWithValue("@KeyTwo", keyTwo);
                insertCommand.ExecuteNonQuery();
            }
        }

        private bool IsInsertionDone()
        {
            string selectQuery = "SELECT * FROM Data";
            using (SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, connection))
            {
                object result = selectCommand.ExecuteScalar();
                return result != null;
            }
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

        public Dictionary<int, string> GetDataFromIdMoreThanTwo()
        {
            Dictionary<int, string> dataKeys = new Dictionary<int, string>();

            string query = "SELECT Id, KeyOne, KeyTwo FROM Data WHERE Id >= 2";

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (Convert.ToInt32(reader["Id"]) == 2)
                        {
                            dataKeys.Add(Convert.ToInt32(reader["Id"]), reader["KeyOne"].ToString());
                        }
                        if (reader["KeyTwo"] != "" && Convert.ToInt32(reader["Id"]) > 2)
                        {
                            dataKeys.Add(Convert.ToInt32(reader["Id"]), reader["KeyTwo"].ToString());
                        }
                    }
                }
            }

            return dataKeys;
        }

        public void ResetHotkeys(Dictionary<int, string> dataKeys)
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=WpfDatabase.sqlite;Version=3;"))
            {
                connection.Open();

                foreach (var item in dataKeys)
                {
                    int id = item.Key;
                    string newKeyValue = "";

                    string updateQuery = "";
                    if (id == 2)
                    {
                        newKeyValue = "Q";
                        updateQuery = "UPDATE Data SET KeyOne = @NewValue WHERE Id = @Id";
                    }
                    else if (id == 3)
                    {
                        newKeyValue = "Q";
                        updateQuery = "UPDATE Data SET KeyTwo = @NewValue WHERE Id = @Id";
                    }
                    else if (id == 4)
                    {
                        newKeyValue = "H";
                        updateQuery = "UPDATE Data SET KeyTwo = @NewValue WHERE Id = @Id";
                    }

                    using (SQLiteCommand updateCommand = new SQLiteCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@Id", id);
                        updateCommand.Parameters.AddWithValue("@NewValue", newKeyValue);
                        updateCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        public void SaveHotkeys(Dictionary<int, string> keys)
        {
            Keybinds keybinds = new Keybinds();
            KeyConverter converter = new KeyConverter();

            using (SQLiteConnection connection = new SQLiteConnection("Data Source=WpfDatabase.sqlite;Version=3;"))
            {
                connection.Open();

                foreach (var item in keys)
                {
                    int id = item.Key;
                    string newKeyValue = item.Value;

                    string updateQuery = "";
                    if (id == 2) 
                    {
                        keybinds.WhenActive = (Key)converter.ConvertFromString(item.Value);
                        updateQuery = "UPDATE Data SET KeyOne = @NewValue WHERE Id = @Id";
                    }
                    else if (id == 3)
                    {
                        keybinds.WhenNotActive = (Key)converter.ConvertFromString(item.Value);
                        updateQuery = "UPDATE Data SET KeyTwo = @NewValue WHERE Id = @Id";
                    }
                    else if (id == 4)
                    {
                        keybinds.HideShow = (Key)converter.ConvertFromString(item.Value);
                        updateQuery = "UPDATE Data SET KeyTwo = @NewValue WHERE Id = @Id";
                    }

                    using (SQLiteCommand updateCommand = new SQLiteCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@Id", id);
                        updateCommand.Parameters.AddWithValue("@NewValue", newKeyValue);
                        updateCommand.ExecuteNonQuery();
                    }
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
