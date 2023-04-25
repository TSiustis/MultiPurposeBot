using Discord.Commands;
using System.Text;

namespace MultiPurposeBot.Modules
{
    public class TestCommands : ModuleBase
    {
        [Command("hello")]
        public async Task HelloCommand()
        {
            var sb = new StringBuilder();
            
            var user = Context.User;
            
            sb.AppendLine($"You are -> [{user.Username}]");
            sb.AppendLine("I must now say, World!");
            
            await ReplyAsync(sb.ToString());
        }

    }
}