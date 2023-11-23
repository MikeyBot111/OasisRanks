using Rocket.API;
using Rocket.Unturned.Player;

namespace OasisRanks
{
    public class OasisConfiguration : IRocketPluginConfiguration
    {
        public string DatabaseAddress;
        public string DatabaseUsername;
        public string DatabasePassword;
        public string DatabaseName;
        public string DatabaseTableName;
        public int DatabasePort;
        public bool Enabled;

        public void LoadDefaults()
        {
            DatabaseAddress = "localhost";
            DatabaseUsername = "root";
            DatabasePassword = "";
            DatabaseName = "unturned";
            DatabaseTableName = "OasisRanks";
            DatabasePort = 3306;
            Enabled = true;
        }
    }


    public class PlayerRankComponent : UnturnedPlayerComponent
    {
        public int Kills = 0;
        public int HDKills = 0;
        public int Deaths = 0;
        public float Accuracy = 0;
        public int KDR = 0;
        public int Rank = 0;
        public int PlaytimeSeconds = 0;
    }
}
