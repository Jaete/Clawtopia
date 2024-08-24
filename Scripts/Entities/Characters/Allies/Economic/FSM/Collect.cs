using Godot;
using Godot.Collections;
using System;

public partial class Collect : State
{
	// TIMER PARA TICK DE RECURSO COLETADO
	public SceneTreeTimer resource_tick_timer;
	// IDENTIFICADOR DO RECURSO SENDO COLETADO ATUALMENTE
	public string currently_collecting;
	// CAPACIDADE MAXIMA DE RECURSO DA UNIDADE
	[Export] public int MAX_QUANTITY = 15;
	// QUANTIDADE ATUAL
	
	// CONSTANTE DE QUANTOS AUMENTA POR TICK
	[Export] public float QUANTITY_PER_TICK = 3;
	// TEMPO EM SEGUNDOS POR TICK
	[Export] public float TICK_TIME = 1.0f;
	public override void Enter(){
		// SEMPRE QUE ENTRAR NO ESTADO COLLECTING, MODIFICAR A VARIAVEL ACIMA
		currently_collecting = self.interacted_resource;
		//INICIAR O TIMER
		resource_tick_timer = GetTree().CreateTimer(TICK_TIME); // tempo em segundos
		resource_tick_timer.Timeout += When_ticked; // CONECTANDO SIGNAL QUANDO O TEMPO ACABA 
		GD.Print("I'm collecting: ", currently_collecting);
	}

	public override void Update(double _delta) {
	}

	public override void Exit(){
		resource_tick_timer.Timeout -= When_ticked;
		currently_collecting = null; // NA SAIDA DO ESTADO, MODIFICAR PARA NULL
	}

	public void When_ticked(){
		// VERIFICO SE A QUANTIDADE SOMADA NAO IRIA ULTRAPASSAR A QUANTIDADE MAXIMA
		if (self.resource_current_quantity + QUANTITY_PER_TICK < MAX_QUANTITY){
			self.resource_current_quantity += QUANTITY_PER_TICK;
		}else{
			self.resource_current_quantity = MAX_QUANTITY;
		}
		if (self.resource_current_quantity != MAX_QUANTITY){
			resource_tick_timer = GetTree().CreateTimer(TICK_TIME);
			resource_tick_timer.Timeout += When_ticked;
		}else{
			var target = Get_closest_resource_building(self.GlobalPosition, currently_collecting);
			Set_target_position(target);
			self.delivering = true;
			Change_state("Move");
		}
	}

	public override void When_mouse_right_clicked(Vector2 coords){
		if (!self.currently_selected){ return; }
		Choose_next_target_position_ECONOMIC(coords);
		Change_state("Move");
	}
}
