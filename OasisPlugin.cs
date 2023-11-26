using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using I18N.West;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using System;
using Steamworks;
using System.Linq;
using Rocket.Unturned.Events;
using SDG.Unturned;

namespace OasisRanks
{
    public class OasisPlugin : RocketPlugin<OasisConfiguration>
    {
        public static string version = "1.0.0.2";
        public static OasisPlugin Instance;

        public static Dictionary<CSteamID, DateTime> PlayerPlaytime = new Dictionary<CSteamID, DateTime>();

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList
                {
                    { "no_permissions", "[Oasis] Nemáš oprávnění na tento příkaz." },
                    { "not_enabled", "[Oasis] Tento příkaz není aktivovaný." },
                    { "cooldown", "[Oasis] Tento příkaz bude dostupný za {0}s." },

                    { "stats_no_player", "[Oasis] Hráč nenalezen." },
                    { "stats_show_0", "[Oasis] Hráč => {0}, Rank => {1}, Doba hraní => {2}s " },
                    { "stats_show_1", "[Oasis] Killů => {0}, Umrtí => {1}, Přesnost => {2}% " },
                    { "stats_show_2", "[Oasis] KDR => {0}, Headshot killy => {1} " },

                    { "ranks_player", "[Oasis] [{0}] {1} ,Killů => {2} ,Umrtí => {3}, KDR => {4}, Přesnost => {5}%" },
                };
            }
        }

        protected override void Load()
        {
            OasisPlugin.Instance = this;

            Logger.Log("OasisRanks, Version " + OasisPlugin.version);
            Logger.Log("Checking errors... ");
            if (Configuration.Instance.Enabled)
            {
                CP1250 cP1250 = new CP1250();
                Database.CheckSchema();
            }
            Logger.Log("No errors have been found!");

            U.Events.OnPlayerConnected += PlayerJoined;
            U.Events.OnPlayerDisconnected += PlayerDisconnect;
            UnturnedPlayerEvents.OnPlayerDeath += PlayerDied;
        }

        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= PlayerJoined;
            U.Events.OnPlayerDisconnected -= PlayerDisconnect;
            UnturnedPlayerEvents.OnPlayerDeath -= PlayerDied;
        }

        private void PlayerJoined(UnturnedPlayer player)
        {
            if (Configuration.Instance.Enabled)
            {
                PlayerRankComponent Exists = Database.RetrieveUser(player.CSteamID);
                if (Exists.Rank == 0)
                {
                    Database.AddUser(player.CSteamID);
                }
                PlayerPlaytime.Add(player.CSteamID, DateTime.Now);
            }
        }

        private void PlayerDisconnect(UnturnedPlayer player)
        {
            if (Configuration.Instance.Enabled)
            {
                PlayerRankComponent Stats = Database.RetrieveUser(player.CSteamID);
                KeyValuePair<CSteamID, DateTime> playtimeRaw = PlayerPlaytime.Where(k => k.Key == player.CSteamID).FirstOrDefault();
                Stats.PlaytimeSeconds += (int)(DateTime.Now - playtimeRaw.Value).TotalSeconds;

                Database.UpdateUser(player.CSteamID, Stats);
                PlayerPlaytime.Remove(player.CSteamID);
            }
        }

        private void PlayerDied(UnturnedPlayer player, EDeathCause cause, ELimb limb, Steamworks.CSteamID murderer)
        {
            if (Configuration.Instance.Enabled)
            {
                PlayerRankComponent StatsPlayer = Database.RetrieveUser(player.CSteamID);
                PlayerRankComponent StatsKiller = Database.RetrieveUser(murderer);

                StatsPlayer.Deaths += 1;
                if (murderer != player.CSteamID)
                {
                    if (limb == ELimb.SKULL)
                    {
                        StatsKiller.HDKills += 1;
                    }
                    StatsKiller.Kills += 1;
                    Database.UpdateUser(murderer, StatsKiller);
                }

                Database.UpdateUser(player.CSteamID, StatsPlayer);
            }
        }
    }
}

