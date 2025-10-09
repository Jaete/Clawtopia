using Godot;
using System.Collections.Generic;

namespace ClawtopiaCs.Scripts.Systems;

public class CounterList
{
    private Dictionary<string, Counter> _counters = new();

    public void AddCounter(string name, int initialValue = 0)
    {
        if (!_counters.ContainsKey(name))
            _counters[name] = new Counter(name, initialValue);
    }

    public Counter Get(string name)
    {
        return _counters.TryGetValue(name, out var counter) ? counter : null;
    }

    public int GetValue(string name)
    {
        return Get(name)?.Value ?? 0;
    }

    public void AddTo(string name, int amount)
    {
        var counter = Get(name);
        counter?.Add(amount);
    }

    public void RemoveFrom(string name, int value)
    {
        var counter = Get(name);
        counter?.Remove(value);
    }
}