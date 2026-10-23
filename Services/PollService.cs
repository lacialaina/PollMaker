using System.Collections.Concurrent;
using PollMaker.Models;

namespace PollMaker.Services;

public class PollService
{
    private readonly ConcurrentDictionary<Guid, Poll> _polls = new();
    private readonly object _voteLock = new();

    // Components subscribe to this to learn when votes change
    public event Action<Guid>? PollChanged;

    public IReadOnlyList<Poll> GetAll() =>
        _polls.Values.OrderByDescending(p => p.CreatedUtc).ToList();

    public Poll? Get(Guid id) =>
        _polls.TryGetValue(id, out var poll) ? poll : null;

    public Poll? GetByManageId(Guid manageId) =>
        _polls.Values.FirstOrDefault(p => p.ManageId == manageId);

    public Poll Create(string question, IEnumerable<string> options)
    {
        var poll = new Poll { Question = question.Trim() };
        int i = 0;
        foreach (var text in options.Where(o => !string.IsNullOrWhiteSpace(o)))
            poll.Options.Add(new PollOption { Id = i++, Text = text.Trim() });

        _polls[poll.Id] = poll;
        return poll;
    }

    public bool Vote(Guid pollId, int optionId)
    {
        var poll = Get(pollId);
        if (poll is null) return false;

        lock (_voteLock) // two people voting at once shouldn't lose a vote
        {
            var option = poll.Options.FirstOrDefault(o => o.Id == optionId);
            if (option is null) return false;
            option.Votes++;
        }

        PollChanged?.Invoke(pollId);
        return true;
    }
}