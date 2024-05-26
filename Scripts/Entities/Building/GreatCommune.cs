using Godot;
using System;

public partial class GreatCommune : Building {
    public override void _Ready() {
        initialize();
        this.is_pre_spawned = true;
        this.placed = true;
        this.region.BakeFinished += free_to_rebake;
        this.self_index = director.tower_count;
        rebake_add_building();
    }
}
