using System.ComponentModel.DataAnnotations;

namespace MultiPurposeBot.Database;
public class EightBallAnswer
{
    [Key]
    public long Id { get; set; }
    public string Text { get; set; }
    public string Color { get; set; }
}