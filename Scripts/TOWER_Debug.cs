using Godot;

public partial class TowerDebug : Label
{
	private Building _self;
	public override void _Ready()
	{
		CallDeferred("set_label_text");
	}

	public void set_label_text()
	{
		_self = GetParent<Building>();
		Text = "Index: " + _self.SelfIndex;
	}
}
