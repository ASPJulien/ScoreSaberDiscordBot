using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using ScoreSaberDiscordBot;

namespace ScoreSaberDiscordBot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        private string scoresaberLink = "https://new.scoresaber.com";

        [Command("link")]
        public async Task LinkAsync(string id)
        {
            bool canLink = true;
            if (string.IsNullOrEmpty(id))
            {
                ReplyAsync("Please enter an scoresaber ID");
                canLink = false;
            }
            try
             {
                if (!string.IsNullOrEmpty(PlayersDB.GetPlayer(Context.User.Id.ToString())))
                {
                    ReplyAsync("A scoresaber ID is already linked to your account,\nplease reach your server administrator");
                    canLink = false;
                }
            }
            finally
            {
                if (canLink)
                {
                    PlayersDB.AddPlayer(Context.User.Id.ToString(), id);
                    ReplyAsync("Your account was succesfully linked :clap:");
                }
            }
        }
    

        [Command("me")]
        public async Task MeAsync(string id = null)
        {
            string _id = PlayersDB.GetPlayer(Context.User.Id.ToString());
            if (!string.IsNullOrEmpty(_id)) id = _id;
            playerInfos infos = PlayerInfos.GetInfos(id);
            var acc = double.Parse(infos.ScoreStats.averageRankedAccuracy.Replace(".", ","));
            acc = Math.Round(acc, 2);
            var builder = new EmbedBuilder()
                .WithTitle($"{infos.PlayerInfo.playerName}")
                .WithColor(Color.Blue)
                .WithThumbnailUrl($"{scoresaberLink}{infos.PlayerInfo.avatar}")
                .AddField("Rank", $":earth_africa: #{infos.PlayerInfo.rank} | :flag_{infos.PlayerInfo.country.ToLower()}: #{infos.PlayerInfo.countryRank}")
                .AddField("Ranked plays count", $":metal: {infos.ScoreStats.rankedPlayCount}")
                .AddField("Performance points", $":clap: {infos.PlayerInfo.pp}", true)
                .AddField("Ranked acc", $":sparkles: {acc}%", true);
            if (infos.PlayerInfo.banned != "0") builder.Color = Color.Red;
            var embed = builder.Build();
            await Context.Channel.SendMessageAsync(null, false, embed);
        }
        
        
    }
}