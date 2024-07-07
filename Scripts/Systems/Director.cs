using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Director : Node2D {
    public Godot.Collections.Array<Ally> allies_to_process = new Godot.Collections.Array<Ally>();
    public Godot.Collections.Array<Ally> allies_processed = new Godot.Collections.Array<Ally>();
    public Godot.Collections.Array<Ally> selected_allies = new Godot.Collections.Array<Ally>();

}
