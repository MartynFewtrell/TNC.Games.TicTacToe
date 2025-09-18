using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Tnc.Games.TicTacToe.Api.Background;

public class SelfPlayJobQueue
{
    private readonly ConcurrentDictionary<Guid, SelfPlayJob> _jobs = new();

    public SelfPlayJob Create(int requested)
    {
        var job = new SelfPlayJob { Requested = requested, Status = SelfPlayJobStatus.Pending };
        _jobs[job.Id] = job;
        return job;
    }

    public bool TryGet(Guid id, out SelfPlayJob? job) => _jobs.TryGetValue(id, out job);

    public IEnumerable<SelfPlayJob> List() => _jobs.Values;

    public void Update(SelfPlayJob job) => _jobs[job.Id] = job;
}
