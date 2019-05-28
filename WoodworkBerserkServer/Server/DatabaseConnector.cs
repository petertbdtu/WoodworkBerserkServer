using Npgsql;
using System;

namespace WoodworkBerserk.Controllers
{
    class DatabaseConnector
    {

        NpgsqlConnection connection;

        public DatabaseConnector()
        {
            initializeDatabase();
            connect();
        }

        public void initializeDatabase()
        {
            //SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            //builder.DataSource = "balarama.db.elephantsql.com,5432";
            //builder.UserID = "onyyvcau";
            //builder.Password = "uceLAbGSTVO4HyOLNczJinlQGWiKzVRR";
            //builder.InitialCatalog = "woodworkberserk";
            //string connectionString;
            var connString = "Host=balarama.db.elephantsql.com;Username=onyyvcau;Password=uceLAbGSTVO4HyOLNczJinlQGWiKzVRR;Database=onyyvcau";
            //connectionString = Properties.Settings.Default.Setting;
            connection = new NpgsqlConnection(connString);
            //connection = new SqlConnection(builder.ConnectionString);
            Console.WriteLine("Database initialized succesfully...");
        }
        public void connect()
        {
            Console.WriteLine("Database connected succesfully...");
        }

        public void disconnect()
        {
            connection.Close();
            Console.WriteLine("Database disconnected succesfully...");
        }

        public bool Authenticate(String username, String password)
        {
            //NpgsqlCommand command;
            //NpgsqlDataReader dataReader;
            String sql;


            sql = "SELECT COUNT(*) FROM player WHERE player_name='" + username + "' AND player_password='" + password + "';";
            using (var command = new NpgsqlCommand(sql, connection))
            {
                bool result;
                //dataReader = command.ExecuteReader();
                connection.Open();
                object cmd = command.ExecuteScalar();
                int cmd2 = int.Parse(string.Format("{0}", cmd));
                Console.WriteLine(cmd);
                if (cmd2 == 0)
                {
                    result = false;
                }
                else
                {
                    result = true;
                }

                //dataReader.Close();
                command.Dispose();
                disconnect();
                return result;
            }
        }
        //doesnt work? dunno why use auto_increment for player_id in SQL database
        public void createPlayer(String username, String password)
        {
            String sql;
            sql = "INSERT INTO player (player_name, player_password, player_position_x, player_position_y, player_active) VALUES (@name, @pass, 50, 50, false)";
            using (var command = new NpgsqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@name", username);
                command.Parameters.AddWithValue("@pass", password);
                connection.Open();
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                Console.WriteLine("Created player with username: " + username + ", and password: " + password);
                command.Dispose();
                disconnect();
            }

        }

        public int generateId()
        {
            NpgsqlCommand command;
            NpgsqlDataReader dataReader;
            String sql;

            sql = "SELECT COUNT(*) FROM player;";
            command = new NpgsqlCommand(sql, connection);
            //dataReader = command.ExecuteReader();
            connection.Open();
            object cmd = command.ExecuteScalar();
            int result = int.Parse(string.Format("{0}", cmd));
            //dataReader.Close();
            command.Dispose();
            disconnect();
            return result + 1;
        }

        public int getPlayer_Id(string username, string password)
        {
            NpgsqlCommand command;
            NpgsqlDataReader dataReader;
            string sql;
            int result = new int();
            sql = "SELECT player_id FROM player WHERE player_name = @name AND player_password = @pass ";
            using (var cmd = new NpgsqlCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("@name", username);
                cmd.Parameters.AddWithValue("@pass", password);
                connection.Open();
                dataReader = cmd.ExecuteReader();
                if (dataReader.Read())
                {
                    result = (int)dataReader.GetInt32(0);
                }
                dataReader.Close();
                cmd.Dispose();
            }
            connection.Close();
            return result;
        }

        public bool getPlayer_active(string username, string password)
        {
            NpgsqlDataReader dataReader;
            string sql;
            bool result = new bool();
            sql = "SELECT player_active FROM player WHERE player_name = @name AND player_password = @pass ";
            using (var cmd = new NpgsqlCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("@name", username);
                cmd.Parameters.AddWithValue("@pass", password);
                connection.Open();
                dataReader = cmd.ExecuteReader();
                if (dataReader.Read())
                {
                    result = (bool)dataReader.GetBoolean(0);
                }
                dataReader.Close();
                cmd.Dispose();
            }
            connection.Close();
            return result;
        }

        public void updatePlayer_active(int player_id, bool player_active)
        {
            string sql;
            sql = "UPDATE player SET player_active = @active WHERE player_id = @id;";
            using (var cmd = new NpgsqlCommand(sql, connection))
            {
                cmd.Parameters.AddWithValue("@id", player_id);
                cmd.Parameters.AddWithValue("@active", player_active);
                connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            connection.Close();
        }
    }
}
