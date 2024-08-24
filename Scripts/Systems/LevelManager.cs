using Godot;
using Godot.Collections;

public partial class LevelManager : Node2D
{
	[Signal]
	public delegate void ResourceDeliveredEventHandler(string resource, int quantity);
	
	// VARIAVEIS DE QUANTIDADE DE RECURSO ATUAL
	public int catnip_quantity = 0;
	public int salmon_quantity = 0;
	public int sand_quantity = 0;
	
	//NODES DE ESTRUTURA DE RECURSO DE SALMAO, CATNIP E AREIA PELO MAPA
	public Array<Building> salmon_buildings = new();
	public Array<Building> catnip_buildings = new();
	public Array<Building> sand_buildings = new();
	// AS CONSTRUCOES VAO INSERIR A SI MESMAS AQUI SEMPRE QUE INSTANCIADAS

	public UI ui;
	public Control res_count;
	public Label catnip_label;
	public Label salmon_label;
	public Label sand_label;
	public override void _Ready(){
		ui = GetNode<UI>("/root/Game/UI");
		res_count = ui.GetNode<Control>("ResourcesCount");
		catnip_label = res_count.GetNode<Label>("CatnipLabel");
		salmon_label = res_count.GetNode<Label>("SalmonLabel");
		sand_label = res_count.GetNode<Label>("SandLabel");
		ResourceDelivered += When_resource_delivered;
		catnip_label.Text = $"{catnip_quantity}";
		salmon_label.Text = $"{salmon_quantity}";
		sand_label.Text = $"{sand_quantity}";
	}

	public void When_resource_delivered(string resource, int quantity){
		switch (resource){
			case Constants.CATNIP:
				catnip_quantity += quantity;
				catnip_label.Text = $"{catnip_quantity}";
				break;
			case Constants.SALMON:
				salmon_quantity += quantity;
				salmon_label.Text = $"{salmon_quantity}";
				break;
			case Constants.SAND:
				sand_quantity += quantity;
				sand_label.Text = $"{sand_quantity}";
				break;
		}
	}
}
