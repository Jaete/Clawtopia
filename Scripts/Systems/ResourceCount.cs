using Godot;
using System;

namespace ClawtopiaCs.Scripts.Systems;

public partial class ResourceCount : Control
{

    public static ResourceCount Singleton { get; private set; }

    private CounterList _counters;

    [Export] public Label catsLabel;
    [Export] public Label idleCatsLabel;

    public override void _Ready()
    {
        Singleton = this;

        _counters = new CounterList();

        _counters.AddCounter(CounterNames.ECONOMIC, 1);
        _counters.AddCounter(CounterNames.MILITARY, 0);
        _counters.AddCounter(CounterNames.ALL, 1);
        _counters.AddCounter(CounterNames.IDLE, 1);

        UpdateLabels();
    }

    public void OnUnitSpawned(string name, int amount)
    {
        _counters.AddTo(name, amount);
        _counters.AddTo(CounterNames.ALL, amount);

        UpdateLabels();
    }
    public void OnUnitDied(string name, int amount)
    {
        _counters.RemoveFrom(name, amount);
        _counters.RemoveFrom(CounterNames.ALL, amount);

        UpdateLabels();
    }

    public void OnIdleUnit(int amount)
    {
        _counters.AddTo(CounterNames.IDLE, amount);

        UpdateLabels();
    }

    public void OnWorkUnit(int amount)
    {
        _counters.RemoveFrom(CounterNames.IDLE, amount);

        UpdateLabels();
    }

    public int GetCount(string counterName) => _counters.GetValue(counterName);

    public void UpdateLabels()
    {
        catsLabel.Text = GetCount(CounterNames.ALL).ToString();
        idleCatsLabel.Text = GetCount(CounterNames.IDLE).ToString();
    }
    
}