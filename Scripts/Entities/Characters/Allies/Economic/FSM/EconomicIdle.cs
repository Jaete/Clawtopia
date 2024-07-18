using System.Runtime.CompilerServices;
using System.Threading;
using Godot;

public partial class EconomicIdle : AllyState
{

    public override void Enter(){
        ally.Velocity = Vector2.Zero;
        /* TODO
         Tocar animacao de idle quando houver
        */
    }

    public override void Update(double _delta){
        /*Nao faz nada, afinal e estado Idle*/
        //GD.Print("I'm idle"); // <- Apenas para DEBUG, sera removido depois
    }

    public override void Exit(){
    }

    public override void When_mouse_right_clicked(Vector2 coords){
        if (ally.currently_selected && is_current_state){
            interacted_with_building = simulation_mode.building_to_interact != null;
            if (interacted_with_building){
                Set_target_position(recalculate_coords(
                    ally.GlobalPosition, 
                    simulation_mode.building_to_interact.GlobalPosition));
                ally.interacted_building = simulation_mode.building_to_interact;
            }else{
                ally.interacted_building = null;
                Set_target_position(coords);
            }
            GD.Print(simulation_mode.tile_map);
            if(simulation_mode.tile_map.GetCellTileData(1, (Vector2I) coords) != null){
                GD.Print("INteragiu com agua");
            }
            Change_state("Move");
            
        }
    }
}
