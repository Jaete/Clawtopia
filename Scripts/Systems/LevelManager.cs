using Godot;
using Godot.Collections;

public partial class LevelManager : Node2D
{
	[Signal]
	public delegate void ResourceDeliveredEventHandler(string resource, int quantity);
	
	[Signal]
	public delegate void ResourceExpendedEventHandler(string resource, int quantity);
	
	// VARIAVEIS DE QUANTIDADE DE RECURSO ATUAL
	[Export] public int CatnipQuantity = 0;
	[Export] public int SalmonQuantity = 0;
	[Export] public int SandQuantity = 0;
	
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
		ResourceDelivered += When_resource_delivered;
		ResourceExpended += WhenResourceExpended;
		CatnipLabel.Text = $"{CatnipQuantity}";
		SalmonLabel.Text = $"{SalmonQuantity}";
		SandLabel.Text = $"{SandQuantity}";
	}
	private void WhenResourceExpended(string resource, int quantity){
		switch (resource){
			case Constants.CATNIP:
				CatnipQuantity -= quantity;
				CatnipLabel.Text = $"{CatnipQuantity}";
				break;
			case Constants.SALMON:
				SalmonQuantity -= quantity;
				SalmonLabel.Text = $"{SalmonQuantity}";
				break;
			case Constants.SAND:
				SandQuantity -= quantity;
				SandLabel.Text = $"{SandQuantity}";
				break;
		}
	}

	public void When_resource_delivered(string resource, int quantity){
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
}
