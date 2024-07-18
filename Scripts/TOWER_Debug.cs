using Godot;
using System;

public partial class TOWER_Debug : Label
{
	private Building self;
	public override void _Ready()
	{
		CallDeferred("set_label_text");
	}

	public void set_label_text()
	{
		self = GetParent<Building>();
		Text = "Index: " + self.self_index;
	}
}
