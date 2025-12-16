using Godot;

public partial class ResourceMenu : BaseMenu
{
   	[Export] public Label resourceLabel;

    public void SetCollectPoint(CollectPoint point)
    {
        resourceLabel.Text = point.ResourceQuantity.ToString() + "/" + point.MaxResourceQuantity.ToString();
    }
}