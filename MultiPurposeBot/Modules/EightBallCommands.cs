using Discord.Commands;
using Discord;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MultiPurposeBot.Database;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace MultiPurposeBot.Modules;

    public class EightBallCommands : ModuleBase
    {
        private readonly MultiPurposeDbContext _db;
        private List<string> _validColors = new();
        private readonly IConfiguration _config;

        public EightBallCommands(IServiceProvider services)
        {
            _db = services.GetRequiredService<MultiPurposeDbContext>();
            _config = services.GetRequiredService<IConfiguration>();

            _validColors.Add("green");
            _validColors.Add("red");
            _validColors.Add("blue");
        }

        [Command("add")]
        public async Task AddResponse(string answer, string color)
        {
            var sb = new StringBuilder();
            var embed = new EmbedBuilder();
        
            var user = Context.User;
        
            if (!_validColors.Contains(color.ToLower()))
            {
                sb.AppendLine($"**Sorry, [{user.Username}], you must specify a valid color.**");
                sb.AppendLine("Valid colors are:");
                sb.AppendLine();
                foreach (var validColor in _validColors)
                {
                    sb.AppendLine($"{validColor}");
                }
                embed.Color = new Color(255, 0, 0);
            }
            else
            {
                await _db.AddAsync(new EightBallAnswer
                {
                    Text = answer,
                    Color = color.ToLower()
                }
                );

                await _db.SaveChangesAsync();
                sb.AppendLine();
                sb.AppendLine("**Added answer:**");
                sb.AppendLine(answer);
                sb.AppendLine();
                sb.AppendLine("**With color:**");
                sb.AppendLine(color);
                embed.Color = new Color(0, 255, 0);
            }
            
            embed.Title = "Eight Ball Answer Addition";
            embed.Description = sb.ToString();
        
            await ReplyAsync(null, false, embed.Build());
        }

        [Command("list")]
        public async Task ListAnswers()
        {
            var sb = new StringBuilder();
            var embed = new EmbedBuilder();
        
            var user = Context.User;

            var answers = await _db.EightBallAnswer.ToListAsync();
            if (answers.Count > 0)
            {
                foreach (var answer in answers)
                {
                    sb.AppendLine($":small_blue_diamond: [{answer.Id}] **{answer.Text}**");
                }
            }
            else
            {
                sb.AppendLine("No answers found!");
            }

            // set embed
            embed.Title = "Eight Ball Answer List";
            embed.Description = sb.ToString();

            // send embed reply
            await ReplyAsync(null, false, embed.Build());
        }

        [Command("remove")]
        public async Task RemoveAnswer(int id)
        {
            var sb = new StringBuilder();
            var embed = new EmbedBuilder();

            // get user info from the Context
            var user = Context.User;

            var answers = await _db.EightBallAnswer.ToListAsync();
            var answerToRemove = answers.FirstOrDefault(a => a.Id == id);

            if (answerToRemove != null)
            {
                _db.Remove(answerToRemove);
                await _db.SaveChangesAsync();
                sb.AppendLine($"Removed answer -> [{answerToRemove.Text}]");
            }
            else
            {
                sb.AppendLine($"Did not find answer with id [**{id}**] in the database");
                sb.AppendLine($"Perhaps use the {_config["prefix"]}list command to list out answers");
            }

            // set embed
            embed.Title = "Eight Ball Answer List";
            embed.Description = sb.ToString();

            // send embed reply
            await ReplyAsync(null, false, embed.Build());
        }

        [Command("8ball")]
        [Alias("ask")]
        public async Task AskEightBall([Remainder] string args = null)
        {
            var sb = new StringBuilder();
        
            var embed = new EmbedBuilder();
        
            var replies = await _db.EightBallAnswer.ToListAsync();
        
            embed.Title = "Welcome to the 8-ball!";
        
            sb.AppendLine($"{Context.User.Username},");
            sb.AppendLine();
        
            if (args == null)
            {
                sb.AppendLine("Sorry, can't answer a question you didn't ask!");
            }
            else
            {
                var answer = replies[new Random().Next(replies.Count)];
            
                sb.AppendLine($"You asked: [**{args}**]...");
                sb.AppendLine();
                sb.AppendLine($"...your answer is [**{answer.Text}**]");

                switch (answer.Color)
                {
                    case "red":
                        {
                            embed.WithColor(255, 0, 0);
                            break;
                        }
                    case "blue":
                        {
                            embed.WithColor(0, 0, 255);
                            break;
                        }
                    case "green":
                        {
                            embed.WithColor(0, 255, 0);
                            break;
                        }
                }
            }

          
            embed.Description = sb.ToString();
        
            await ReplyAsync(null, false, embed.Build());
        }
    }
