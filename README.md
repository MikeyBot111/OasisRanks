# OasisRanks

> **⚠️ Archived – old project, no longer maintained.**
> Written in 2023 for the **Oasis** Unturned servers. It's kept here for reference only. It may not work with current versions of Unturned or RocketMod.

A RocketMod plugin for **Unturned** that tracks **player stats and rankings** in **MySQL**. The same table was used by the leaderboard on the [Oasis website](https://github.com/MikeyBot111/WebsiteOasis).

## What it does

- Records **kills, headshot kills, deaths and playtime** for each player
- Creates a player record on their first join
- Adds playtime when a player disconnects
- Works out K/D, accuracy (headshot %) and rank position
- Shows any player's stats in game, plus a top-players list
- The database table is created automatically on first load

## Commands & permissions

| Command | Permission | Description |
|---|---|---|
| `/stats [player]` | `OasisRanks.stats` | Show your own stats, or another player's |
| `/ranks` | `OasisRanks.ranks` | Show the top 4 players |

## Config

| Key | Default |
|---|---|
| `DatabaseAddress` | `localhost` |
| `DatabaseUsername` | `root` |
| `DatabasePassword` | *(empty)* |
| `DatabaseName` | `unturned` |
| `DatabaseTableName` | `OasisRanks` |
| `DatabasePort` | `3306` |
| `Cooldown` | `10` |

The repo only has placeholder defaults. Real database details are set in the server's plugin config.

## Tech

- C# / .NET Framework 4.8
- RocketMod, MySqlConnector (NuGet)
- `lib/` holds the reference assemblies needed to build

## Build

Restore NuGet packages, open `OasisRanks.sln` in Visual Studio and build it. Then copy the DLL and its dependencies into `Rocket/Plugins`.

## Part of the Oasis project

[OasisEssential](https://github.com/MikeyBot111/OasisEssential) · [OasisTeleport](https://github.com/MikeyBot111/OasisTeleport) · [OasisKits](https://github.com/MikeyBot111/OasisKits) · [OasisRanks](https://github.com/MikeyBot111/OasisRanks) · [OasisVault](https://github.com/MikeyBot111/OasisVault) · [OasisDiscord](https://github.com/MikeyBot111/OasisDiscord) · [OasisCustomCommands](https://github.com/MikeyBot111/OasisCustomCommands) · [WebsiteOasis](https://github.com/MikeyBot111/WebsiteOasis) · [oasis-server-discord](https://github.com/MikeyBot111/oasis-server-discord)
