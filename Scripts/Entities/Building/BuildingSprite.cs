using Godot;

[GlobalClass, Tool]
public partial class BuildingSprite : Sprite2D
{
    [Signal] public delegate void PropertyChangedEventHandler();

    [Export] private Building _building;

    public override void _Ready()
    {
        TextureChanged += OnPropertyChanged;
        PropertyChanged += OnPropertyChanged;
    }

    public void OnPropertyChanged() 
    {
        if (Engine.IsEditorHint() && BuildingData.ValidateBuilding(_building, MethodName.OnPropertyChanged))
        {
            CallDeferred(MethodName.SaveSprite);
        }
    }

    public void SaveSprite()
    {
        if (_building.IsRotated)
        {
            SetRotatedSprite();
        }
        else
        {
            SetSprite();
        }

        ResourceSaver.Singleton.Save(_building.Data);
    }

    private void SetSprite()
    {
        if (_building.IsPreSpawned)
        {
            _building.Data.Structure.PlacedTexture = Texture;
            _building.Data.Structure.PlacedOffset = Offset;
        }
        else
        {
            _building.Data.Structure.PreviewTexture = Texture;
            _building.Data.Offset = Offset;
        }
    }

    private void SetRotatedSprite()
    {
        if (_building.IsPreSpawned)
        {
            _building.Data.Structure.RotatedPlacedTexture = Texture;
            _building.Data.Structure.RotatedPlacedOffset = Offset;
        }
        else
        {
            _building.Data.Structure.RotatedPreviewTexture = Texture;
            _building.Data.RotatedOffset = Offset;
        }
    }

    public override bool _Set(StringName property, Variant value)
    {
        var result = base._Set(property, value);
        if (GetSignalConnectionList(SignalName.PropertyChanged).Count == 0)
        {
            PropertyChanged += OnPropertyChanged;
        }
        if (value.Obj != null)
        {
            EmitSignal(SignalName.PropertyChanged);
        }
        return result;
    }
}
