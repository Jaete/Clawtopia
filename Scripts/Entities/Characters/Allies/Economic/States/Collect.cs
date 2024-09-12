using Godot;

public partial class Collect : EconomicState
{
	// TIMER PARA TICK DE RECURSO COLETADO
	public SceneTreeTimer ResourceTickTimer;
	// IDENTIFICADOR DO RECURSO SENDO COLETADO ATUALMENTE
	public string CurrentlyCollecting;
	// CAPACIDADE MAXIMA DE RECURSO DA UNIDADE
	[Export] public int MaxQuantity = 15;
	// QUANTIDADE ATUAL
	
	// CONSTANTE DE QUANTOS AUMENTA POR TICK
	[Export] public int QuantityPerTick = 3;
	// TEMPO EM SEGUNDOS POR TICK
	[Export] public float TickTime = 1.0f;
	public override void Enter(){
		// SEMPRE QUE ENTRAR NO ESTADO COLLECTING, MODIFICAR A VARIAVEL ACIMA
		CurrentlyCollecting = Ally.InteractedResource;
		//INICIAR O TIMER
		ResourceTickTimer = GetTree().CreateTimer(TickTime); // tempo em segundos
		ResourceTickTimer.Timeout += CollectTimeTicked; // CONECTANDO SIGNAL QUANDO O TEMPO ACABA 
		GD.Print("I'm collecting: ", CurrentlyCollecting);
	}

	public override void Update(double delta) {
	}

	public override void Exit(){
		ResourceTickTimer.Timeout -= CollectTimeTicked;
		CurrentlyCollecting = null; // NA SAIDA DO ESTADO, MODIFICAR PARA NULL
	}

	public void CollectTimeTicked(){
		// VERIFICO SE A QUANTIDADE SOMADA NAO IRIA ULTRAPASSAR A QUANTIDADE MAXIMA
		if (Ally.ResourceCurrentQuantity + QuantityPerTick < MaxQuantity){
			Ally.ResourceCurrentQuantity += QuantityPerTick;
		}else{
			Ally.ResourceCurrentQuantity = MaxQuantity;
		}
		if (Ally.ResourceCurrentQuantity != MaxQuantity){
			ResourceTickTimer = GetTree().CreateTimer(TickTime);
			ResourceTickTimer.Timeout += CollectTimeTicked;
		}else{
			var target = GetClosestResourceBuilding(Ally.GlobalPosition, CurrentlyCollecting);
			Ally.Navigation.SetTargetPosition(target);
			Ally.Delivering = true;
			ChangeState("Move");
		}
	}

	public override void MouseRightClicked(Vector2 coords){
		if (!Ally.CurrentlySelected){ return; }
		ChooseNextTargetPosition(coords);
		ChangeState("Move");
	}
}
