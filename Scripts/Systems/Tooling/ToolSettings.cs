using Godot;

[GlobalClass, Tool]
public partial class ToolSettings : Resource
{
    [Export(PropertyHint.Dir)]
    public string ResourceDirectory { get; set; }
}
