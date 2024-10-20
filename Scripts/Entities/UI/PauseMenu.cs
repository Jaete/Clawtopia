using Godot;

public partial class PauseMenu : Control
{
    public override void _Notification(int what)
    {
        if (what == NotificationApplicationFocusOut)
        {
            GetTree().Paused = true;
            Visible = true;
        }
        else if (what == NotificationApplicationFocusIn)
        {
            GetTree().Paused = false;
            Visible = false;
        }
    }
}
