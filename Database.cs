using Rocket.Core.Logging;
using Steamworks;
using System;
using System.Collections.Generic;
using MySqlConnector;
using Rocket.Unturned.Player;

namespace OasisRanks
{
    internal class Database
    {
        private static string CompileArguments(string command, string[] args, string[] argsValues)
        {
            string NewCommand = command;
            int i = 0;
            foreach (string arg in args)
            {
                NewCommand = NewCommand.Replace(arg, argsValues[i]);
                i++;
            }
            return NewCommand;
        }


        private static MySqlConnection CreateConnection()
        {
            MySqlConnection connection = null;
            var builder = new MySqlConnectionStringBuilder
            {
                Server = OasisPlugin.Instance.Configuration.Instance.DatabaseAddress,
                UserID = OasisPlugin.Instance.Configuration.Instance.DatabaseUsername,
                Password = OasisPlugin.Instance.Configuration.Instance.DatabasePassword,
                Database = OasisPlugin.Instance.Configuration.Instance.DatabaseName,
                Port = (uint)OasisPlugin.Instance.Configuration.Instance.DatabasePort,
                AllowUserVariables = true
            };

            try
            {
                connection = new MySqlConnection(builder.ConnectionString);
                connection.Open();
            }
            catch (Exception ex)
            {
                Logger.LogWarning("MySql Connection Failed");
                Logger.LogException(ex);
            }

            return connection;
        }
        public static void CheckSchema()
        {
            MySqlConnection mySqlConnection = Database.CreateConnection();
            if (mySqlConnection == null)
            {
                Logger.LogWarning("MySql Schema Failed - Connection null");
                return;
            }

            try
            {
                var getCommand = mySqlConnection.CreateCommand();

                string[] args = { "@Tablename" };
                string[] argsValues = { OasisPlugin.Instance.Configuration.Instance.DatabaseTableName };
                string cmd = "SHOW TABLES LIKE '@Tablename'";

                getCommand.CommandText = CompileArguments(cmd, args, argsValues);
                var reader = getCommand.ExecuteReader();
                if (reader.Read().ToString() == "False")
                {
                    reader.Close();
                    var setCommand = mySqlConnection.CreateCommand();

                    string[] args2 = { "@Tablename" };
                    string[] argsValues2 = { OasisPlugin.Instance.Configuration.Instance.DatabaseTableName };
                    string cmd2 = "CREATE TABLE `@Tablename` (`id` int(11) NOT NULL AUTO_INCREMENT,`kills` int(11) NOT NULL,`deaths` int(11) NOT NULL,`playtimeSeconds` int(11) NOT NULL,`hdkills` int(11) NOT NULL,`csteamid` varchar(32) NOT NULL,`username` varchar(32) NOT NULL ,PRIMARY KEY (`id`))";
                    
                    setCommand.CommandText = CompileArguments(cmd2, args2, argsValues2);
                    setCommand.ExecuteNonQuery();
                }
                mySqlConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogWarning("MySql Schema Failed");
                Logger.LogException(ex);
                mySqlConnection.Close();
            }
        }

        internal static PlayerRankComponent RetrieveUser(CSteamID csteamid)
        {
            PlayerRankComponent ReturnStats = new PlayerRankComponent();

            MySqlConnection mySqlConnection = Database.CreateConnection();
            if (mySqlConnection == null)
            {
                Logger.LogWarning("MySql RetrieveUser Failed - Connection null");
                return new PlayerRankComponent();
            }

            try
            {
                int num = 0;

                string[] args = { "@Tablename", "@cSteamId" };
                string[] argsValues = { OasisPlugin.Instance.Configuration.Instance.DatabaseTableName, csteamid.ToString() };
                string cmd = "SELECT COUNT(*) FROM `@Tablename` WHERE `csteamid` = '@cSteamId';";
                MySqlCommand command1 = mySqlConnection.CreateCommand();
                command1.CommandText = CompileArguments(cmd, args, argsValues);
                var reader = command1.ExecuteReader();
                while (reader.Read())
                {
                    num = reader.GetInt32(0);
                }
                reader.Close();
                if (num == 0)
                {
                    return new PlayerRankComponent();
                }

                string[] args2 = { "@Tablename", "@cSteamId" };
                string[] argsValues2 = { OasisPlugin.Instance.Configuration.Instance.DatabaseTableName, csteamid.ToString() };
                string cmd2 = "SELECT * FROM `@Tablename` WHERE `csteamid` = '@cSteamId' ORDER BY id;";
                MySqlCommand command2 = mySqlConnection.CreateCommand();
                command2.CommandText = CompileArguments(cmd2, args2, argsValues2);
                var reader2 = command2.ExecuteReader();

                while (reader2.Read())
                {
                    ReturnStats.Kills = reader2.GetInt32("kills");
                    ReturnStats.Deaths = reader2.GetInt32("deaths");
                    ReturnStats.HDKills = reader2.GetInt32("hdkills");
                    ReturnStats.PlaytimeSeconds = reader2.GetInt32("playtimeSeconds");
                }
                reader2.Close();

                string[] args3 = { "@Tablename" };
                string[] argsValues3 = { OasisPlugin.Instance.Configuration.Instance.DatabaseTableName };
                string cmd3 = "SELECT * FROM `@Tablename` ORDER BY kills DESC;";
                MySqlCommand command3 = mySqlConnection.CreateCommand();
                command3.CommandText = CompileArguments(cmd3, args3, argsValues3);
                var reader3 = command3.ExecuteReader();

                int i = 0;
                while (reader3.Read())
                {
                    i++;
                    if (reader3.GetString("csteamid") == csteamid.ToString())
                    {
                        break;
                    }
                }
                reader3.Close();

                ReturnStats.Rank = i;

                if ( ReturnStats.Kills != 0)
                {
                    ReturnStats.Accuracy = (float)ReturnStats.HDKills / (float)ReturnStats.Kills;
                }
                else
                {
                    ReturnStats.Accuracy = 0;
                }

                if (ReturnStats.Deaths != 0)
                {
                    ReturnStats.KDR = (float)ReturnStats.Kills / (float)ReturnStats.Deaths;
                }
                else
                {
                    ReturnStats.KDR = ReturnStats.Kills;
                }
                ReturnStats.Accuracy *= 100;
                mySqlConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogWarning("MySql RetrieveUser Failed");
                Logger.LogException(ex);
                mySqlConnection.Close();
            }

            return ReturnStats;
        }

        internal static void UpdateUser(CSteamID cSteamID, PlayerRankComponent Stats)
        {
            MySqlConnection mySqlConnection = Database.CreateConnection();
            if (mySqlConnection == null)
            {
                Logger.LogWarning("MySql UpdateUser Failed - Connection null");
                return;
            }

            try
            {
                string[] args = { "@Tablename", "@kills", "@deaths", "@playtimeSeconds", "@hdkills", "@csteamid" };
                string[] argsValues = { OasisPlugin.Instance.Configuration.Instance.DatabaseTableName, Stats.Kills.ToString(), Stats.Deaths.ToString(), Stats.PlaytimeSeconds.ToString(), Stats.HDKills.ToString(), cSteamID.ToString()};
                string cmd = "UPDATE `@Tablename` SET `kills`='@kills',`deaths`='@deaths',`playtimeSeconds`='@playtimeSeconds',`hdkills`='@hdkills' WHERE `csteamid`='@csteamid';";

                MySqlCommand command1 = mySqlConnection.CreateCommand();
                command1.CommandText = CompileArguments(cmd, args, argsValues);
                command1.ExecuteNonQuery();
                mySqlConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogWarning("MySql UpdateUser Failed");
                Logger.LogException(ex);
                mySqlConnection.Close();
            }
        }

        internal static bool AddUser(CSteamID csteamid)
        {
            MySqlConnection mySqlConnection = Database.CreateConnection();
            if (mySqlConnection == null)
            {
                Logger.LogWarning("MySql AddUser Failed - Connection null");
                return false;
            }
            try
            {

                string[] args = { "@Tablename", "@csteamid", "@username" };
                string[] argsValues = { OasisPlugin.Instance.Configuration.Instance.DatabaseTableName, csteamid.ToString(), UnturnedPlayer.FromCSteamID(csteamid).CharacterName};
                string cmd = "INSERT INTO `@Tablename` (`csteamid`,`kills`,`deaths`,`playtimeSeconds`,`hdkills`, `username`) VALUES(@csteamid,0,0,0,0, @username);";

                MySqlCommand command1 = mySqlConnection.CreateCommand();
                command1.CommandText = CompileArguments(cmd, args, argsValues);

                command1.ExecuteNonQuery();
                mySqlConnection.Close();
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogWarning("MySql AddUser Failed");
                Logger.LogException(ex);
                mySqlConnection.Close();
                return false;
            }
        }

        internal static List<PlayerRankComponent> RetrieveLeaders()
        {
            MySqlConnection mySqlConnection = Database.CreateConnection();
            if (mySqlConnection == null)
            {
                Logger.LogWarning("MySql RetrieveLeaders Failed - Connection null");
                return new List<PlayerRankComponent>();
            }
            try
            {

                List<PlayerRankComponent> list = new List<PlayerRankComponent>();

                string[] args2 = { "@Tablename" };
                string[] argsValues2 = { OasisPlugin.Instance.Configuration.Instance.DatabaseTableName };
                string cmd2 = "SELECT * FROM `@Tablename` ORDER BY `kills` DESC LIMIT 4;";
                MySqlCommand command2 = mySqlConnection.CreateCommand();
                command2.CommandText = CompileArguments(cmd2, args2, argsValues2);
                var reader2 = command2.ExecuteReader();
                while (reader2.Read())
                {
                    list.Add(new PlayerRankComponent()
                    {
                        Kills = reader2.GetInt32("kills"),
                        HDKills = reader2.GetInt32("hdkills"),
                        Deaths = reader2.GetInt32("deaths"),
                        PlaytimeSeconds = reader2.GetInt32("playtimeSeconds"),
                        PlayerID = (CSteamID)ulong.Parse(reader2.GetString("csteamid")),
                        Rank = (list.Count + 1),
                        Accuracy = reader2.GetInt32("kills") != 0 ? (((float)reader2.GetInt32("hdkills") / (float)reader2.GetInt32("kills")) * 100) : 0,
                        KDR = reader2.GetInt32("deaths") != 0 ? ((float)reader2.GetInt32("kills") / (float)reader2.GetInt32("deaths")) : reader2.GetInt32("kills"),
                        Username = reader2.GetString("username")
                    });
                }
                reader2.Close();
                mySqlConnection.Close();
                return list;
            }
            catch (Exception ex)
            {
                Logger.LogWarning("MySql RetrieveLeaders Failed");
                Logger.LogException(ex);
                mySqlConnection.Close();
                return new List<PlayerRankComponent>();
            }
        }
    }
}
