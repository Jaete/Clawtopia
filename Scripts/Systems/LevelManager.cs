using Godot;
using Godot.Collections;

public partial class LevelManager : Node2D
{
	[Signal]
	public delegate void ResourceDeliveredEventHandler(string resource, int quantity);
	[Signal]
	public delegate void ResourceExpendedEventHandler(BuildingData buildingData);
	[Signal]
	public delegate void NotEnoughResourcesEventHandler();
	
	// VARIAVEIS DE QUANTIDADE DE RECURSO ATUAL
	public int CatnipQuantity = 0;
	public int SalmonQuantity = 0;
	public int SandQuantity = 0;
	
	//NODES DE ESTRUTURA DE RECURSO DE SALMAO, CATNIP E AREIA PELO MAPA
	public Array<Building> SalmonBuildings = new();
	public Array<Building> CatnipBuildings = new();
	public Array<Building> SandBuildings = new();
	// AS CONSTRUCOES VAO INSERIR A SI MESMAS AQUI SEMPRE QUE INSTANCIADAS

	public UI Ui;
	public Control ResCount;
	public Label CatnipLabel;
	public Label SalmonLabel;
	public Label SandLabel;
	public override void _Ready(){
		Ui = GetNode<UI>("/root/Game/UI");
		ResCount = Ui.GetNode<Control>("ResourcesCount");
		CatnipLabel = ResCount.GetNode<Label>("CatnipLabel");
		SalmonLabel = ResCount.GetNode<Label>("SalmonLabel");
		SandLabel = ResCount.GetNode<Label>("SandLabel");
		ResourceDelivered += Delivered;
		ResourceExpended += Expended;
		NotEnoughResources += NotEnough;
		CatnipQuantity = Constants.STARTING_SAND_QUANTITY;
		SalmonQuantity = Constants.STARTING_SALMON_QUANTITY;
		SandQuantity = Constants.STARTING_SAND_QUANTITY;
		CatnipLabel.Text = $"{CatnipQuantity}";
		SalmonLabel.Text = $"{SalmonQuantity}";
		SandLabel.Text = $"{SandQuantity}";
	}
	private void NotEnough(){
		GD.Print("Not enough resources");
		
	}

	public void Delivered(string resource, int quantity){
		switch (resource){
			case Constants.CATNIP:
				CatnipQuantity += quantity;
				CatnipLabel.Text = $"{CatnipQuantity}";
				break;
			case Constants.SALMON:
				SalmonQuantity += quantity;
				SalmonLabel.Text = $"{SalmonQuantity}";
				break;
			case Constants.SAND:
				SandQuantity += quantity;
				SandLabel.Text = $"{SandQuantity}";
				break;
		}
	}

	public void Expended(BuildingData data){
		CatnipQuantity -= data.CatnipCost;
		SalmonQuantity -= data.SalmonCost;
		SandQuantity -= data.SandCost;
		CatnipLabel.Text = $"{CatnipQuantity}";
		SalmonLabel.Text = $"{SalmonQuantity}";
		SandLabel.Text = $"{SandQuantity}";
	}


}
