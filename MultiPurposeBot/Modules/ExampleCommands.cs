using Discord.Interactions;
using MultiPurposeBot.Services;

namespace MultiPurposeBot.Modules
{
    public class TestCommands : InteractionModuleBase<SocketInteractionContext>
    {
        public InteractionService Commands {get;set;}
        public CommandHandler _handler;

        public TestCommands(CommandHandler handler)
        {
            _handler = handler;
        }

        [SlashCommand("8ball", "Ask the magic 8 ball a question!")]
        public async Task EightBall(string question)
        {
            var replies = new List<string>();

            replies.Add("yes");
            replies.Add("no");
            replies.Add("maybe");
            replies.Add("hazzzyyy.....");

            var answer = replies[new Random().Next(replies.Count)];

            await RespondAsync(answer);
        }

    }
}