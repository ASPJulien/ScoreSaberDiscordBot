using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using ScoreSaberDiscordBot;

namespace ScoreSaberDiscordBot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        private string scoresaberLink = "https://new.scoresaber.com";
        private string bsppBoosterLink = "https://bs-pp-booster.abachelet.fr/?/Home/AjaxProfilePlaylist/Download-1/ProfileID-";

        [Command("link")]
        public async Task LinkAsync(string id)
        {
            bool canLink = true;
            ulong checkingParse;
            if (string.IsNullOrEmpty(id))
            {
                ReplyAsync("Please enter an scoresaber ID");
                canLink = false;
            }
            
            if (id.Length != 17 || !ulong.TryParse(id, out checkingParse) || !PlayerInfos.HasASSPage(id))
            {
                ReplyAsync("Please enter a correct scoresaber ID");
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
            if (!string.IsNullOrEmpty(_id))
            {
                if (string.IsNullOrEmpty(id)) id = _id;
                playerInfos infos = PlayerInfos.GetInfos(id);
                var acc = double.Parse(infos.ScoreStats.averageRankedAccuracy.Replace(".", ","));
                acc = Math.Round(acc, 2);
                var builder = new EmbedBuilder()
                    .WithTitle($"{infos.PlayerInfo.playerName}")
                    .WithUrl($"https://scoresaber.com/u/{id}")
                    .WithColor(Color.Blue)
                    .WithThumbnailUrl($"{scoresaberLink}{infos.PlayerInfo.avatar}")
                    .WithFooter("ScoreSaberStats 1.0.1 | Dev Version")
                    .AddField("Rank", $":earth_africa: #{infos.PlayerInfo.rank} | :flag_{infos.PlayerInfo.country.ToLower()}: #{infos.PlayerInfo.countryRank}")
                    .AddField("Ranked plays count", $":metal: {infos.ScoreStats.rankedPlayCount}")
                    .AddField("Performance points", $":clap: {infos.PlayerInfo.pp}", true)
                    .AddField("Ranked acc", $":sparkles: {acc}%", true);
            
                if (infos.PlayerInfo.banned != "0") builder.Color = Color.Red;
                var embed = builder.Build();
                await Context.Channel.SendMessageAsync(null, false, embed);
            }
            else
            {
                Context.Channel.SendMessageAsync(
                    "Sorry but your account isn't linked, please link it with the `!link` command");
            }
        }

        [Command("improve")]
        public async Task BSPPBoosterAsync()
        {
            string id = PlayersDB.GetPlayer(Context.User.Id.ToString());
            if (!string.IsNullOrEmpty(id))
            {
                WebClient webClient = new WebClient();
                webClient.DownloadFile(new Uri(bsppBoosterLink + id), $"bsppbooster-{id}.bplist");
                Context.Channel.SendFileAsync($"bsppbooster-{id}.bplist", "Here's your file :smile:");
            }
            else
                Context.Channel.SendMessageAsync(
                    "Sorry but your account isn't linked, please link it with the `!link` command");

        }

        [Command("unlink")]
        public async Task unlinkAsync()
        {
            try
            {
                if (!string.IsNullOrEmpty(PlayersDB.GetPlayer(Context.User.Id.ToString())))
                {
                    if (PlayersDB.RemovePlayer(Context.User.Id.ToString()))
                    {
                        Context.Channel.SendMessageAsync("Your account was successfully unlinked!");
                    }
                    else
                        Context.Channel.SendMessageAsync("Sorry but your account isn't linked");
                }

                else
                    Context.Channel.SendMessageAsync("Sorry but your account isn't linked");
            }
            catch (Exception e)
            {
                Context.Channel.SendMessageAsync("Sorry but your account isn't linked");
            }

        }
        
    }
}