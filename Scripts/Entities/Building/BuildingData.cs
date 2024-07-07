using Godot;
using System;

public partial class BuildingData : Resource
{
    private Vector2 GREAT_COMMUNE_OFFSET = new(-32, -64);
    private Vector2 GREAT_COMMUNE_SCALE = new(0.3f, 0.3f);
    private Vector2 GREAT_COMMUNE_INTERACTION_OFFSET = new(-25, -50);
    private Vector2 FIGHTERS_TOWER_OFFSET = new(0, -72);
    private Vector2 FIGHTERS_TOWER_SCALE = new(1, 1);
    private Vector2 FIGTHERS_TOWER_INTERACTION_OFFSET = new(0, -72);
    public ConcavePolygonShape2D obstacle_shape;
    public RectangleShape2D interaction_shape;
    public ConcavePolygonShape2D grid_shape;
    public Texture2D sprite_texture;
    public Vector2 sprite_offset;
    public Vector2 sprite_size;
    public Vector2 interaction_offset;
    
    [Export]
    public String type;
    [Export]
    public String name;
    [Export]
    public int HP;

    private int level;

    public void initialize() {
        switch(type) {
            case "Resource":
                break;
            case "Tower":
                level = 1;
                obstacle_shape = (ConcavePolygonShape2D)GD.Load("res://Resources/Buildings/Towers/Fighters/obstacle_shape.tres");
                interaction_shape = (RectangleShape2D)GD.Load("res://Resources/Buildings/Towers/Fighters/interaction_shape.tres");
                grid_shape = (ConcavePolygonShape2D)GD.Load("res://Resources/Buildings/Towers/Fighters/obstacle_shape.tres");
                switch (name) {
                    case "Fighters":
                        sprite_texture = (Texture2D)GD.Load("res://Assets/Buildings/torreA.png");
                        sprite_offset = FIGHTERS_TOWER_OFFSET;
                        sprite_size = FIGHTERS_TOWER_SCALE;
                        interaction_offset = FIGTHERS_TOWER_INTERACTION_OFFSET;
                        break;
                }
                break;
            case "GreatCommune":
                level = 1;
                sprite_texture = (Texture2D)GD.Load("res://Assets/Buildings/building_01_tavern.png");
                sprite_offset = GREAT_COMMUNE_OFFSET;
                sprite_size = GREAT_COMMUNE_SCALE;
                obstacle_shape = (ConcavePolygonShape2D)GD.Load("res://Resources/Buildings/GreatCommune/obstacle_shape.tres");
                interaction_shape = (RectangleShape2D)GD.Load("res://Resources/Buildings/GreatCommune/interaction_shape.tres");
                grid_shape = (ConcavePolygonShape2D)GD.Load("res://Resources/Buildings/GreatCommune/obstacle_shape.tres");
                interaction_offset = GREAT_COMMUNE_INTERACTION_OFFSET;
                break;
        }
    }
}
