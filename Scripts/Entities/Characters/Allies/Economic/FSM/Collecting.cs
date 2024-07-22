using Godot;
using Godot.Collections;
using System;

public partial class Collecting : AllyState
{
	// TIMER PARA TICK DE RECURSO COLETADO
	public SceneTreeTimer resource_tick_timer;
	// IDENTIFICADOR DO RECURSO SENDO COLETADO ATUALMENTE
	public string currently_collecting = null;
	// CAPACIDADE MAXIMA DE RECURSO DA UNIDADE
	public int MAX_QUANTITY = 15;
	// QUANTIDADE ATUAL
	
	// CONSTANTE DE QUANTOS AUMENTA POR TICK
	public int QUANTITY_PER_TICK = 3;
	// TEMPO EM SEGUNDOS POR TICK
	public float TICK_TIME = 1.0f;
	public override void Enter(){
		// SEMPRE QUE ENTRAR NO ESTADO COLLECTING, MODIFICAR A VARIAVEL ACIMA
		currently_collecting = ally.interacted_resource;
		//INICIAR O TIMER
		resource_tick_timer = GetTree().CreateTimer(TICK_TIME); // tempo em segundos
		resource_tick_timer.Timeout += When_ticked; // CONECTANDO SIGNAL QUANDO O TEMPO ACABA 
		GD.Print("I'm collecting: ", currently_collecting);
	}

	public override void Update(double _delta) {
	}

	public override void Exit(){
        currently_collecting = null; // NA SAIDA DO ESTADO, MODIFICAR PARA NULL
	}

	public void When_ticked(){
		// VERIFICO SE A QUANTIDADE SOMADA NAO IRIA ULTRAPASSAR A QUANTIDADE MAXIMA
		if (ally.resource_current_quantity + QUANTITY_PER_TICK < MAX_QUANTITY){
			ally.resource_current_quantity += QUANTITY_PER_TICK;
		}else{
			ally.resource_current_quantity = MAX_QUANTITY;
		}
		if (ally.resource_current_quantity != MAX_QUANTITY){
			resource_tick_timer = GetTree().CreateTimer(TICK_TIME);
			resource_tick_timer.Timeout += When_ticked;
		}else{
			var target = Get_closest_resource_building(ally.GlobalPosition, currently_collecting);
			Set_target_position(target);
			ally.delivering = true;
			Change_state("Move");
		}
	}
}
