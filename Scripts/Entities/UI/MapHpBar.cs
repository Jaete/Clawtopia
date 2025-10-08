using Godot;
using System;
public partial class MapHpBar : TextureProgressBar
{
    private Building _building;

    public void SetBuilding(Building building)
    {
        _building = GetParent<Building>();

        if (_building?.Data != null)
        {
            if (_building.Data.BuildingHP.ContainsKey(_building.Type))
            {
                MaxValue = _building.Data.BuildingHP[_building.Type];
            }
            else
            {
                GD.PrintErr($"[MapHpBar] Nenhum HP definido para o prédio: {_building.Type}");
            }
        }
    }
    public override void _Process(double delta)
    {
        if (_building?.Data == null) return;

        if (_building.Data.CurHp < _building.Data.MaxHp)
        {
            Visible = true;

            if (_building.Data.CurHp > 0)
            {
                Value = _building.Data.CurHp;
            }
            else
            {
                Value = 0;
            }
        }
        else if (_building.Data.CurHp == _building.Data.MaxHp)
        {
            Visible = false;
        }
    }
}