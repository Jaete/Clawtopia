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
        GD.Print("Setting building: ", _building?.Type);
        GD.Print("Has data: ", _building?.Data != null);
        GD.Print("Hp: ", ((object) _building?.Data?.BuildingHP[_building.Type] ?? "Not defined"));

        if (_building?.Data != null)
        {
            MaxValue = _building.Data.MaxHp;
        }
    }
    public override void _Process(double delta)
    {
        if (_building?.Data == null) return;


        if (_building.Data.CurHp > 0)
        {
            //Lógica para ver a barrinha descendo, pode apagar
                timer--;
                if (timer <= 0)
                {
                    _building.Data.CurHp--;
                    timer = 50;
                }
            ////
    
            Value = _building.Data.CurHp;
        }
        else
        {
            Value = 0;
        }

        UpdateLabel();
    }

    private void UpdateLabel()
    {
        if (hpLabel == null || _building?.Data == null) return;

        hpLabel.Text = $"{_building.Data.CurHp} / {MaxValue}";
    }
}