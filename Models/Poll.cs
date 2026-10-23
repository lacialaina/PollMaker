namespace PollMaker.Models;

public class Poll
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid ManageId { get; init; } = Guid.NewGuid();
    public string Question { get; set; } = "";
    public List<PollOption> Options { get; set; } = new();
    public DateTime CreatedUtc { get; init; } = DateTime.UtcNow;

    public int TotalVotes => Options.Sum(o => o.Votes);
}

public class PollOption
{
    public int Id { get; set; }
    public string Text { get; set; } = "";
    public int Votes { get; set; }
}