using Godot;
using Godot.Collections;

[GlobalClass, Tool]
public partial class BuildingList : Resource
{

    [Signal] public delegate void BuildingListChangedEventHandler();

    private Array<BuildingData> _buildingData;

    [Export]
    public Array<BuildingData> List {
        get => _buildingData; 
        set
        {
            _buildingData = value;
            EmitSignal(SignalName.BuildingListChanged);
        }
            
    }
}
