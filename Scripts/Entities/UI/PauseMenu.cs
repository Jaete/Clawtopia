using Godot;

public partial class PauseMenu : Control
{
    public override void _Notification(int what)
    {
        if (what == NotificationWMMouseExit)
        {
            GetTree().Paused = true;
            Visible = true;
        }
        else if (what == NotificationWMMouseEnter)
        {
            GetTree().Paused = false;
            Visible = false;
        }
    }
}
