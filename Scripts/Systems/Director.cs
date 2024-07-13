using Godot;
using Godot.Collections;

public partial class Director : Node2D {
    public Array<Ally> allies_to_process;
    public Array<Ally> allies_processed;
    public Array<Ally> selected_allies;
}
