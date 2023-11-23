using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Core.Utils;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;


namespace OasisRanks
{
    public class OasisCommand : IRocketCommand
    {
        #region Declarations
        private Dictionary<CSteamID, DateTime> Cooldown = new Dictionary<CSteamID, DateTime>();
        public List<string> Permissions => new List<String>() { "OasisRanks.stats" };
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "stats";
        public string Syntax => "stats (player)";
        public string Help => "Příkaz pro statistiky hráče.";
        public List<string> Aliases => new List<string>();
        #endregion

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (!OasisPlugin.Instance.Configuration.Instance.Enabled)
            {
                UnturnedChat.Say(player, OasisPlugin.Instance.Translate("stats_not_enabled"), UnityEngine.Color.red);
                return;
            }
            if (!player.HasPermission("stats") && !player.IsAdmin)
            {
                UnturnedChat.Say(player, OasisPlugin.Instance.Translate("stats_no_permissions"), UnityEngine.Color.red);
                return;
            }
            if (command.Length == 0)
            {
                PlayerRankComponent CPlayer = Database.RetrieveUser(player.CSteamID);
                if (CPlayer.Rank == 0)
                {
                    UnturnedChat.Say(caller, OasisPlugin.Instance.Translate("stats_no_player"), UnityEngine.Color.red);
                    return;
                }

                UnturnedChat.Say(caller, OasisPlugin.Instance.Translate("stats_show_0", player.CharacterName, CPlayer.Rank, CPlayer.PlaytimeSeconds), UnityEngine.Color.green);
                UnturnedChat.Say(caller, OasisPlugin.Instance.Translate("stats_show_1", CPlayer.Kills, CPlayer.Deaths, CPlayer.Accuracy), UnityEngine.Color.green);
                UnturnedChat.Say(caller, OasisPlugin.Instance.Translate("stats_show_2", CPlayer.KDR, CPlayer.HDKills), UnityEngine.Color.green);
                return;
            }

            UnturnedPlayer PlayerToLookup = UnturnedPlayer.FromName(command[0].ToString());
            if (PlayerToLookup == null)
            {
                UnturnedChat.Say(caller, OasisPlugin.Instance.Translate("stats_no_player"), UnityEngine.Color.red);
                return;
            }

            PlayerRankComponent Player = Database.RetrieveUser(PlayerToLookup.CSteamID);
            if (Player.Rank == 0)
            {
                UnturnedChat.Say(caller, OasisPlugin.Instance.Translate("stats_no_player"), UnityEngine.Color.red);
                return;
            }

            UnturnedChat.Say(caller, OasisPlugin.Instance.Translate("stats_show_0", PlayerToLookup.CharacterName, Player.Rank, Player.PlaytimeSeconds), UnityEngine.Color.green);
            UnturnedChat.Say(caller, OasisPlugin.Instance.Translate("stats_show_1", Player.Kills, Player.Deaths, Player.Accuracy), UnityEngine.Color.green);
            UnturnedChat.Say(caller, OasisPlugin.Instance.Translate("stats_show_2", Player.KDR, Player.HDKills), UnityEngine.Color.green);

        }
    }
}