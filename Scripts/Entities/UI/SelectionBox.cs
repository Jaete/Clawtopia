using Godot;

public partial class SelectionBox : ColorRect
{
    public static Color RectColor = new Color("4CB1E4");
    public RectangleShape2D SelectionShape;

    public override void _Draw(){
        DrawRect(SelectionShape.GetRect(), RectColor, false);
    }
}
