using System;
using Godot;

public partial class Cronometer : Node
{
    private float currentTime = 0f;
    private bool cronometerOn = false;

    public Label TimerLabel;
    public Control ResCount;
    public UI Ui;

    public override void _Ready()
    {
        cronometerOn = true;

        Ui = GetNode<UI>("/root/Game/UI");
        ResCount = Ui.GetNode<Control>("ResourcesCount");
        TimerLabel = ResCount.GetNode<Label>("Labels/TimerLabel");
    }

    public override void _Process(double delta)
    {
        if (cronometerOn)
        {
            currentTime += (float)delta;
            UpdateLabel();
        }
    }

    public void ResetCronometer()
    {
        currentTime = 0f;
    }

    private void UpdateLabel()
    {
        TimeSpan t = TimeSpan.FromSeconds(currentTime);
        TimerLabel.Text = t.ToString(@"mm\:ss");
    }
}