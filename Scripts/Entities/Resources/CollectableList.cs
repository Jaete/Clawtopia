using System.Linq;
using Godot;
using Godot.Collections;

[GlobalClass, Tool]
public partial class CollectableList : Resource
{
    [Signal] public delegate void CollectableListChangedEventHandler();

    private Array<Collectable> _collectables;

    [Export]
    public Array<Collectable> Resources
    {
        get => _collectables;
        set
        {
            _collectables = value;
            EmitSignal(SignalName.CollectableListChanged);
        }

    }

    public static Array<string> GetNames(Array<Collectable> resources)
    {
        if (resources == null)
        {
            return [];
        }

        var names = resources
            .Where(r => !string.IsNullOrWhiteSpace(r.Name) && !string.IsNullOrEmpty(r.Name))
            .Select(r => r.Name)
            .ToArray();

        return new Array<string>(names); ;
    }
}

