using Godot;
using System;
public partial class BuildingHpBar : TextureProgressBar
{
    private Building _building;

    [Export] public Label hpLabel;

    public int timer = 200;

    public void SetBuilding(Building building)
    {
        _building = building;
        if (_building?.Data != null)
        {
            MaxValue = _building.Data.MaxHp;
        }
    }
    public override void _Process(double delta)
    {
        if (_building?.Data == null) return;

        if (_building.Data.CurHp > 0)
            Value = _building.Data.CurHp;
        else
            Value = 0;

        UpdateLabel();
    }

    private void UpdateLabel()
    {
        if (hpLabel == null || _building?.Data == null) return;

        hpLabel.Text = $"{_building.Data.CurHp} / {MaxValue}";
    }
}