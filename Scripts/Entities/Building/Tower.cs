using Godot;
using System;

public partial class Tower : Building {

    public override void _Ready() {
        initialize();
        is_pre_spawned = false;
        placed = false;
        region.BakeFinished += free_to_rebake;
        self_index = director.tower_count;
    }
}
