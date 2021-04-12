using System;
using System.Net;
using Newtonsoft.Json;

namespace ScoreSaberDiscordBot
{
    public class Player
    {
        public string discordID;
        public string ssID;
    }
    
    public class PlayerInfos
    {
        public static string json;
        
        public static playerInfos GetInfos(string id)
        {
            WebClient webClient = new();
            json = webClient.DownloadString($"https://new.scoresaber.com/api/player/{id}/full");
            playerInfos Infos = new playerInfos();
            Infos = JsonConvert.DeserializeObject<playerInfos>(json);
            return Infos;
        }

        public static bool HasASSPage(string id)
        {
            WebClient webClient = new();
            json = webClient.DownloadString($"https://new.scoresaber.com/api/player/{id}/full");
            if (json.Length > 1) return true;
            else return false;
        }
        
    }

    public class playerInfos
    {
        public playerInfo PlayerInfo;
        public scoreStats ScoreStats;
    }
    
    public class playerInfo
    {
        public string playerName;
        public string pp;
        public string country;
        public string rank;
        public string countryRank;
        public string avatar;
        public string banned;
    }
    public class scoreStats
    {
        public string rankedPlayCount;
        public string averageRankedAccuracy;

    }
    
}