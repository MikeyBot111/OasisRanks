using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using Steamworks;
using System;
using System.Collections.Generic;

namespace OasisRanks
{
    public class OasisCommandStats : IRocketCommand
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
                UnturnedChat.Say(player, OasisPlugin.Instance.Translate("not_enabled"), UnityEngine.Color.red);
                return;
            }
            if (!player.HasPermission("stats") && !player.IsAdmin)
            {
                UnturnedChat.Say(player, OasisPlugin.Instance.Translate("no_permissions"), UnityEngine.Color.red);
                return;
            }

            if (Cooldown.ContainsKey(player.CSteamID))
            {
                int timeLeft = Convert.ToInt32(System.Math.Abs((DateTime.Now - Cooldown[player.CSteamID]).TotalSeconds));
                if (timeLeft < OasisPlugin.Instance.Configuration.Instance.Cooldown)
                {
                    UnturnedChat.Say(player, OasisPlugin.Instance.Translate("cooldown", OasisPlugin.Instance.Configuration.Instance.Cooldown - timeLeft), UnityEngine.Color.red);
                    return;
                }
                Cooldown.Remove(player.CSteamID);
            }

            if (Cooldown.ContainsKey(player.CSteamID))
            {
                Cooldown[player.CSteamID] = DateTime.Now;
            }
            else
            {
                Cooldown.Add(player.CSteamID, DateTime.Now);
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
    public class OasisCommandRanks : IRocketCommand
    {
        #region Declarations
        private Dictionary<CSteamID, DateTime> Cooldown = new Dictionary<CSteamID, DateTime>();
        public List<string> Permissions => new List<String>() { "OasisRanks.ranks" };
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "ranks";
        public string Syntax => "ranks";
        public string Help => "Příkaz pro top 4 hráče.";
        public List<string> Aliases => new List<string>();
        #endregion

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (!OasisPlugin.Instance.Configuration.Instance.Enabled)
            {
                UnturnedChat.Say(player, OasisPlugin.Instance.Translate("not_enabled"), UnityEngine.Color.red);
                return;
            }
            if (!player.HasPermission("ranks") && !player.IsAdmin)
            {
                UnturnedChat.Say(player, OasisPlugin.Instance.Translate("no_permissions"), UnityEngine.Color.red);
                return;
            }
            if (Cooldown.ContainsKey(player.CSteamID))
            {
                int timeLeft = Convert.ToInt32(System.Math.Abs((DateTime.Now - Cooldown[player.CSteamID]).TotalSeconds));
                if (timeLeft < OasisPlugin.Instance.Configuration.Instance.Cooldown)
                {
                    UnturnedChat.Say(player, OasisPlugin.Instance.Translate("cooldown", OasisPlugin.Instance.Configuration.Instance.Cooldown - timeLeft), UnityEngine.Color.red);
                    return;
                }
                Cooldown.Remove(player.CSteamID);
            }

            if (Cooldown.ContainsKey(player.CSteamID))
            {
                Cooldown[player.CSteamID] = DateTime.Now;
            }
            else
            {
                Cooldown.Add(player.CSteamID, DateTime.Now);
            }

            List<PlayerRankComponent> list = Database.RetrieveLeaders();

            foreach (PlayerRankComponent rank in list)
            {
                UnturnedChat.Say(player, OasisPlugin.Instance.Translate("ranks_player", rank.Rank, rank.Username, rank.Kills, rank.Deaths, rank.KDR, rank.Accuracy), UnityEngine.Color.yellow);
            }

        }
    }
}