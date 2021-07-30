using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;

namespace botnewbot.Commands
{
    public class Help : ModuleBase<SocketCommandContext>
    {
        [Command("명령어")]
        [Alias("도움말", "도움", "help")]
        public async Task help()
        {
            JObject json = JObject.Parse(File.ReadAllText("Help.json"));
            // List<EmbedFieldBuilder> categories = new List<EmbedFieldBuilder>();
            // EmbedBuilder embedBuilder = new EmbedBuilder();
            var options = new List<SelectMenuOptionBuilder>();
            foreach (var command in json)
            {
                string name = command.Key;
                string summary = command.Value["Summary"].ToString();
                string emoji = command.Value["Emoji"].ToString();
                // embedBuilder.AddField(name, summary);
                options.Add(new SelectMenuOptionBuilder().WithLabel(name).WithDescription(summary).WithEmote(new Emoji(emoji)).WithValue(name));
            }
            ComponentBuilder componentBuilder = new ComponentBuilder()
            .WithSelectMenu(new SelectMenuBuilder()
                .WithCustomId("helpComponent")
                .WithPlaceholder("도움말을 볼 명령어를 선택")
                .WithOptions(options));
            await ReplyAsync("도움말을 볼 주제를 선택하세요.", false, component: componentBuilder.Build());
        }
        public static async Task sendHelp(SocketMessageComponent component)
        {
            JObject json = JObject.Parse(File.ReadAllText("Help.json"));
            var value = component.Data.Values.First();
            JArray selectedCategory = json[value]["Commands"] as JArray;
            EmbedBuilder embedBuilder = new EmbedBuilder()
                .WithTitle(value + "에 관한 명령어들");
            foreach(var command in selectedCategory)
            {
                EmbedFieldBuilder newField = new EmbedFieldBuilder();
                newField.Name = command["Command"].ToString();
                newField.Value = $"```{command["Summary"]}```\n같은 명령어: `{string.Join(", ", (command["Alias"] as JArray).ToObject<string[]>())}`";
                embedBuilder.AddField(newField);
            }
            await component.RespondAsync(embed: embedBuilder.Build(), type: InteractionResponseType.UpdateMessage, ephemeral: true);
        }
    }
}