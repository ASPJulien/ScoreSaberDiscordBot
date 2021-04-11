using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace ScoreSaberDiscordBot
{
    public class PlayersDB
    {
        static List<Player> players = new List<Player>();

        public static void AddPlayer(string disID, string scoID)
        {
            players.Add(new Player{discordID = disID, ssID = scoID});
            Console.WriteLine($"Player {disID} was added with scoresaber: {scoID}");
            GenerateDB();
        }
        
        public static void GenerateDB()
        {
            File.WriteAllText("playerDB.json", "");
            File.WriteAllText("playerDB.json", JsonConvert.SerializeObject(players));
        }

        public static void ReadDB()
        {
            int i = 0;
            string json = File.ReadAllText("playerDB.json");
            players = JsonConvert.DeserializeObject<List<Player>>(json);
            foreach (var vPlayer in players)
            {
                i++;
            }
            Console.WriteLine($"Finished to read all database, {i} players found!");
        }

        public static string GetPlayer(string disID)
        {
            
            foreach (var player in players)
            {
                if (disID == player.discordID)
                {
                    return player.ssID;
                }
            }
            return null;
        }
        
    }
}