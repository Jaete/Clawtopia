using Godot;
using System;

public partial class SelectionBox : ColorRect
{
    public static Color rect_color = new Color("4CB1E4");
    public RectangleShape2D selection_shape;

    public override void _Draw() {
        if(GetNodeOrNull(GetPath()) != null){
            DrawRect(selection_shape.GetRect(), rect_color, false);
        }
    }
}
