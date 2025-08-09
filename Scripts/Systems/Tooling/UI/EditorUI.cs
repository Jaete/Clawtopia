using Godot;

public partial class EditorUI : GodotObject
{
    public static void PopupAccept(string text)
    {
        AcceptDialog dialog = new()
        {
            DialogText = text
        };
        EditorInterface.Singleton.GetEditorMainScreen().AddChild(dialog);
        dialog.PopupCentered();
    }
}
